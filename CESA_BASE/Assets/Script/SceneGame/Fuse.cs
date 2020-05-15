using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fuse : MonoBehaviour
{
    public enum FuseType
    {
        Normal,
        Start,
        Rotate,
        MoveLeft,
        MoveRight,
        MoveDown,
        MoveUp,
        MoveBack,
        MoveForward,
    }
    //public enum FuseChild
    //{
    //    Model,
    //    Target,
    //}
    public enum FuseState
    {
        None,       // 場に置かれている
        UI,         // UIになっている
        Burn,       // 燃えている
        Out,        // 燃え尽きた後
        Wet         // 濡れている
    }

    private Transform m_childModel = null;
    private Transform m_childTarget = null;

    [SerializeField]
    private FuseType m_type = FuseType.Normal;
    [SerializeField]
    private Texture2D m_fuseTex = null;

    private readonly Vector3 OUTPOS = new Vector3(-50, -50, -50);       // 導火線を生成できない位置
    private FuseState m_state = FuseState.None;
    // 燃えているか
    private Vector3 m_targetDistance = Vector3.zero;
    private HashSet<GameObject> m_collisionObj = new HashSet<GameObject>();
    private float m_countTime = 0.0f;

    // UI用
    private Vector3 m_endPos = Vector3.zero;
    private Vector3 m_defaultPos = Vector3.zero;
    private Vector3 m_defaultRot = Vector3.zero;

    private bool m_isRotate = false;

    public Vector3 EndPos
    {
        get
        {
            return m_endPos;
        }
        set
        {
            m_endPos = value;
            m_defaultPos = transform.parent.position + value;
        }
    }
    public Vector3 DefaultPos
    {
        get
        {
            return m_defaultPos;
        }
        set
        {
            m_defaultPos = value;
        }
    }
    public FuseType Type
    {
        get
        {
            return m_type;
        }
        set
        {
            m_type = value;
        }
    }
    public FuseState State
    {
        get
        {
            return m_state;
        }
        set
        {
            m_state = value;
        }
    }
    public Vector3 DefaultRot
    {
        get
        {
            return m_defaultRot;
        }
    }
    public Vector3 TargetDistance
    {
        get
        {
            return m_targetDistance;
        }       
        set
        {
            m_targetDistance = value;
        }
    }
    public Transform ChildModel
    {
        get
        {
            return m_childModel;
        }
    }
    public Transform ChildTarget
    {
        get
        {
            return m_childTarget;
        }
        set
        {
            m_childTarget = value;
        }
    }



    private void Awake()
    {
        // 水が乾くまでの時間
        m_countTime = AdjustParameter.Fuse_Constant.WET_MAX_TIME;
    }

    private void Start()
    {
        // 燃焼していないことをシェーダーにセット
        m_childModel = transform.GetChild(0);
        m_childTarget = transform.GetChild(1);

        // 導火線本体の中心座標を設定
        m_childModel.GetComponent<Renderer>().material.SetVector("_Center", m_childModel.position);
        if (m_type == FuseType.Start)
        {
            m_state = FuseState.Burn;
            m_targetDistance = Vector3.down / 2.0f;

            // 導火線の燃えてきた方向にシェーダー用のオブジェクトを移動
            m_childTarget.position = transform.position + TargetDistance;
            m_childModel.GetComponent<Renderer>().material.SetVector("_Center", m_childModel.position);
        }
        else
        {
            // 色を変えるオブジェクトの座標
            m_childModel.GetComponent<Renderer>().material.SetVector("_Target", OUTPOS);
            // 燃やす範囲（0:その場だけ ～　1:全域）
            m_childModel.GetComponent<Renderer>().material.SetFloat("_Ration", 0);
            m_childModel.GetComponent<Renderer>().material.SetTexture("_MainTex", m_fuseTex);
        }

        m_defaultRot = transform.localEulerAngles;

       // 元の位置を保存
       if(m_state != FuseState.UI)
            m_defaultPos = transform.position;
    }

    void Update()
    {
        switch(m_state)
        {
            case FuseState.UI:
                {
                    // 移動官僚していないなら
                    if (m_endPos != Vector3.zero)
                    {
                        // 下へ落下
                        transform.localPosition -= new Vector3(0.0f, AdjustParameter.UI_OBJECT_Constant.MOVE_VALUE_Y, 0.0f);
                        // 範囲処理
                        if (transform.localPosition.y <= m_endPos.y)
                        {
                            // 移動完了
                            transform.localPosition = m_endPos;
                            m_endPos = Vector3.zero;
                        }
                    }
                    break;
                }
            case FuseState.Burn:
                {
                    // 色変更用オブジェクトが中心にいないなら
                    if (m_childTarget.localPosition != Vector3.zero)
                    {
                        float scaleDot = Time.deltaTime / AdjustParameter.Fuse_Constant.BURN_MAX_TIME;
                        m_childTarget.localScale += new Vector3(scaleDot, scaleDot, scaleDot);

                        // 移動
                        m_childTarget.position -= (m_targetDistance / Mathf.Abs(Vector3.Dot(Vector3.one, m_targetDistance))) * Time.deltaTime / AdjustParameter.Fuse_Constant.BURN_MAX_TIME * 0.5f;

                        // 導火線と同じ大きさになったら
                        if (m_childTarget.localScale.x >= 1.0f)
                        {
                            m_childTarget.localScale = Vector3.one;
                            m_childTarget.position = transform.position;

                            // 当たっている導火線に引火
                            foreach (GameObject _obj in m_collisionObj)
                            {
                                if (Utility.TagSeparate.getParentTagName(_obj.transform.tag) == NameDefine.TagName.Fuse)
                                {
                                    Fuse _fuse = _obj.GetComponent<Fuse>();
                                    _fuse.State = FuseState.Burn;
                                    GameMgr.Instance.BurnCount += 1;

                                    _fuse.TargetDistance = new Vector3(
                                        (transform.position.x - _fuse.transform.position.x) * 0.5f,
                                        (transform.position.y - _fuse.transform.position.y) * 0.5f,
                                        (transform.position.z - _fuse.transform.position.z) * 0.5f);

                                    // 導火線の燃えてきた方向にシェーダー用のオブジェクトを移動
                                    _fuse.ChildTarget.position =
                                        _fuse.transform.position + _fuse.TargetDistance;

                                    // 導火線本体の中心座標を設定
                                    Transform childModel = _fuse.ChildModel;
                                    childModel.GetComponent<Renderer>().material.SetVector("_Center", childModel.position);
                                }
                                else if (Utility.TagSeparate.getParentTagName(_obj.transform.tag) == NameDefine.TagName.Gimmick)
                                {
                                    GameGimmick _gimmick = _obj.gameObject.GetComponent<GameGimmick>();
                                    _gimmick.GetComponent<Renderer>().material.SetColor("_Color", Color.red);

                                    // 当たったものがゴールなら
                                    if (_gimmick.Type == GameGimmick.GimmickType.Goal && !_gimmick.GimmickStart)
                                    {
                                        // 燃えたゴールを登録
                                        GameMgr.Instance.FireGoal(_gimmick);
                                        // 燃えているオブジェクトを加算
                                        GameMgr.Instance.BurnCount += 1;
                                        _gimmick.GimmickStart = true;
                                    }
                                }
                            }

                            // 燃え尽きたことをマネージャーに通知
                            GameMgr.Instance.BurnOutFuse();
                            m_state = FuseState.Out;
                            m_countTime = AdjustParameter.Fuse_Constant.OUT_MAX_TIME;
                        }

                        // 色を変えるオブジェクトの座標
                        m_childModel.GetComponent<Renderer>().material.SetVector("_Target", m_childTarget.position);
                        // 燃やす範囲（0:その場だけ ～　1:全域）
                        m_childModel.GetComponent<Renderer>().material.SetFloat("_Ration", (m_childTarget.localScale.x + m_childTarget.localScale.y + m_childTarget.localScale.z) / 3);
                    }
                    break;
                }
            case FuseState.Wet:
                {
                    gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.blue); // debug

                    m_countTime -= Time.deltaTime/* * GameMgr.Instance.GameSpeed*/;
                    if (m_countTime <= 0.0f)
                        m_state = FuseState.None;
                    break;
                }
            case FuseState.Out:
                {
                    m_countTime -= Time.deltaTime;
                    // 一定時間たったら消す
                    if(m_countTime <= 0)
                    {
                        GameMgr.Instance.DestroyFuse(this);
                        Destroy(gameObject);
                    }
                    break;
                }
            default:
                break;
        }
    }

    private void FixedUpdate()
    {
        if (m_state == FuseState.UI)
        {
            transform.localEulerAngles = new Vector3(m_defaultRot.x,
                m_defaultRot.y - Camera.main.transform.localEulerAngles.y, m_defaultRot.z);
        }
    }

    /// <summary>
    /// ポストエフェクトをかけるためのレイヤーの制御
    /// </summary>
    /// <param name="isSet">ポストエフェクトを開始か終わりか</param>
    public void SelectUIFuse(bool isSet)
    {
        for (int i = 0; i < transform.childCount - 1; ++i)
        {
            GameObject _obj = transform.GetChild(i).gameObject;
            if (isSet)
                _obj.layer = NameDefine.Layer.PostEffect;
            else
                _obj.layer = NameDefine.Layer.Ignore;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // 燃えていない導火線のみ判定をとる
        if (m_state != FuseState.Burn)
            return;
             
        // 導火線との判定
        if (Utility.TagSeparate.getParentTagName(other.transform.tag) == NameDefine.TagName.Fuse)
        {
            Fuse _fuse = other.gameObject.GetComponent<Fuse>();

            // 相手が燃えているもしくは燃え尽きた後なら処理を飛ばす
            if (!_fuse || _fuse.State != FuseState.None)
                return;

            m_collisionObj.Add(_fuse.gameObject);
        }
        else if (Utility.TagSeparate.getParentTagName(other.transform.tag) == NameDefine.TagName.Gimmick)
        {
            GameGimmick _gimmick = other.gameObject.GetComponent<GameGimmick>();

            // 水は燃えないので
            if (_gimmick.Type == GameGimmick.GimmickType.Water)
                return;

            m_collisionObj.Add(_gimmick.gameObject);
        }
    }

    public void OnGimmick()
    {
        switch (m_type)
        {
            case FuseType.Rotate:
                {
                    if(!m_isRotate)
                        StartCoroutine(RotateFuse());
                    break;
                }
            case FuseType.MoveLeft:
                {
                    GimmickMove(Vector3.left);
                    break;
                }
            case FuseType.MoveRight:
                {
                    GimmickMove(Vector3.right);
                    break;
                }
            case FuseType.MoveDown:
                {
                    GimmickMove(Vector3.down);
                    break;
                }
            case FuseType.MoveUp:
                {
                    GimmickMove(Vector3.up);
                    break;
                }
            case FuseType.MoveBack:
                {
                    GimmickMove(Vector3.back);
                    break;
                }
            case FuseType.MoveForward:
                {
                    GimmickMove(Vector3.forward);
                    break;
                }
            default:
                    break;
        }
    }

    private void GimmickMove(Vector3 direct)
    {
        Vector3 movePos;   // 移動位置
        bool isMove = transform.position == m_defaultPos;

        if (!isMove)
        {
            movePos = m_defaultPos;
            StartCoroutine(MoveFuse(movePos, direct));
        }
        else
        {
            movePos = m_defaultPos - direct;
            StartCoroutine(MoveFuse(movePos, -Vector3.back));
        }
    }

    /// <summary>
    /// 導火線の回転
    /// </summary>
    public IEnumerator RotateFuse()
    {
        //回転中のフラグを立てる
        m_isRotate = true;

        //回転処理
        float sumAngle = 0.0f; //angleの合計を保存
        while (sumAngle < 90.0f)
        {
            float fuseAngle = AdjustParameter.Fuse_Constant.ROT_VALUE; //ここを変えると回転速度が変わる
            sumAngle += fuseAngle;

            // 90度以上回転しないように値を制限
            if (sumAngle > 90.0f)
                fuseAngle -= sumAngle - 90f;

            transform.RotateAround(transform.position, Vector3.down, fuseAngle);

            yield return null;
        }

        //回転中のフラグを倒す
        m_isRotate = false;
        yield return null;
    }

    /// <summary>
    /// 導火線の移動
    /// </summary>
    /// <param name="target">目的地</param>
    /// <param name="direction">向き</param>
    public IEnumerator MoveFuse(Vector3 target, Vector3 direction)
    {
        RaycastHit hit = new RaycastHit();
        float dist = 1f;

        if (!(Physics.Raycast(transform.position, direction, out hit, dist)))
        {
            float sum = 0f;

            while (sum < 1f)
            {
                sum += AdjustParameter.Fuse_Constant.MOVE_VALUE * Time.deltaTime;

                transform.position = Vector3.MoveTowards(transform.position, target, AdjustParameter.Fuse_Constant.MOVE_VALUE * Time.deltaTime);
                yield return null;
            }
        }

        yield break;
    }

    /// <summary>
    /// 導火線が濡れた時の処理
    /// </summary>
    public void FuseWet()
    {
        if (m_state == FuseState.Out || m_state == FuseState.UI)
            return;

        m_state = FuseState.Wet;
        m_countTime = AdjustParameter.Fuse_Constant.WET_MAX_TIME; 

        if (m_state == FuseState.Burn)
        {
            // 燃える演出
            m_childTarget.localScale = Vector3.one;
            m_childTarget.position = transform.position;
            
            // 色を変えるオブジェクトの座標
            m_childModel.GetComponent<Renderer>().material.SetVector("_Target", m_childTarget.position);
            // 燃やす範囲（0:その場だけ ～　1:全域）
            m_childModel.GetComponent<Renderer>().material.
                SetFloat("_Ration", (m_childTarget.localScale.x + m_childTarget.localScale.y + m_childTarget.localScale.z) / 3);

            GameMgr.Instance.BurnOutFuse();
        }
    }
}
