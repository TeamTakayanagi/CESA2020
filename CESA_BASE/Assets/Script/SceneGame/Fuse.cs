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
    public enum FuseChild
    {
        Model,
        Target,
    }

    [SerializeField]
    private FuseType m_type = FuseType.Normal;
    [SerializeField]
    private Texture2D m_fuseTex = null;

    private readonly Vector3 OUTPOS = new Vector3(-50, -50, -50);       // 導火線を生成できない位置
    
    // 燃えているか
    private bool m_isBurn = false;
    private Vector3 m_targetDistance = Vector3.zero;
    private HashSet<GameObject> m_collisionObj = new HashSet<GameObject>();

    // UI用
    private bool m_isUI = false;
    private Vector3 m_endPos = Vector3.zero;
    private Vector3 m_defaultPos = Vector3.zero;
    private Vector3 m_defaultRot = Vector3.zero;

    private bool m_isRotate = false;

    // 水
    private bool m_isWet;
    private float m_wetTime = 0.0f;

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
    public bool UI
    {
        get
        {
            return m_isUI;
        }
        set
        {
            m_isUI = value;
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

    private void Awake()
    {
        // 水が乾くまでの時間
        m_wetTime = AdjustParameter.Fuse_Constant.WET_MAX_TIME;  
    }

    private void Start()
    {
        // 燃焼していないことをシェーダーにセット
        Transform fuseModel = transform.GetChild((int)FuseChild.Model);
        Transform target = transform.GetChild((int)FuseChild.Target);

        // 導火線本体の中心座標を設定
        fuseModel.GetComponent<Renderer>().material.SetVector("_Center", fuseModel.position);
        if (m_type == FuseType.Start)
        {
            m_isBurn = true;

            m_targetDistance = Vector3.down / 2.0f;

            // 導火線の燃えてきた方向にシェーダー用のオブジェクトを移動
            transform.GetChild((int)FuseChild.Target).position = transform.position + TargetDistance;

            // 導火線本体の中心座標を設定
            Transform childModel = transform.GetChild((int)FuseChild.Model);
            fuseModel.GetComponent<Renderer>().material.SetVector("_Center", childModel.position);
        }
        else
        {
            m_isBurn = false;

            // 色を変えるオブジェクトの座標
            fuseModel.GetComponent<Renderer>().material.SetVector("_Target", OUTPOS);
            // 燃やす範囲（0:その場だけ ～　1:全域）
            fuseModel.GetComponent<Renderer>().material.SetFloat("_Ration", 0);
            fuseModel.GetComponent<Renderer>().material.SetTexture("_MainTex", m_fuseTex);
        }

        m_defaultRot = transform.localEulerAngles;

       // 元の位置を保存
       if(!m_isUI)
            m_defaultPos = transform.position;
    }

    void Update()
    {
        // 燃えてるなら
        if (!m_isUI && m_isBurn)
        {
            // 燃える演出
            Transform fuseModel = transform.GetChild((int)FuseChild.Model);
            Transform target = transform.GetChild((int)FuseChild.Target);

            // 色変更用オブジェクトが中心にいないなら
            if (target.localPosition != Vector3.zero)
            {
                float scaleDot = Mathf.Abs(Vector3.Dot(Vector3.one,
                    m_targetDistance * Time.deltaTime / AdjustParameter.Fuse_Constant.BURN_MAX_TIME));
                target.localScale += new Vector3(scaleDot, scaleDot, scaleDot);

                // 移動
                target.position -= m_targetDistance * Time.deltaTime / AdjustParameter.Fuse_Constant.BURN_MAX_TIME * 0.5f;

                // 導火線と同じ大きさになったら
                if (target.localScale.x >= 1.0f)
                {
                    target.localScale = Vector3.one;
                    target.position = transform.position;

                    // 当たっている導火線に引火
                    foreach (GameObject _obj in m_collisionObj)
                    {
                        if (Utility.TagSeparate.getParentTagName(_obj.transform.tag) == NameDefine.TagName.Fuse)
                        {
                            Fuse _fuse = _obj.GetComponent<Fuse>();
                            _fuse.m_isBurn = true;
                            GameMgr.Instance.BurnCount += 1;

                            _fuse.TargetDistance = new Vector3(
                                (transform.position.x - _fuse.transform.position.x) * 0.5f,
                                (transform.position.y - _fuse.transform.position.y) * 0.5f,
                                (transform.position.z - _fuse.transform.position.z) * 0.5f);

                            // 導火線の燃えてきた方向にシェーダー用のオブジェクトを移動
                            _fuse.transform.GetChild((int)FuseChild.Target).position =
                                _fuse.transform.position + _fuse.TargetDistance;

                            // 導火線本体の中心座標を設定
                            Transform childModel = _fuse.transform.GetChild((int)FuseChild.Model);
                            childModel.GetComponent<Renderer>().material.SetVector("_Center", childModel.position);
                        }
                        else if (Utility.TagSeparate.getParentTagName(_obj.transform.tag) == NameDefine.TagName.Gimmick)
                        {
                            GameGimmick _gimmick = _obj.gameObject.GetComponent<GameGimmick>();
                            _gimmick.GetComponent<Renderer>().material.SetColor("_Color", Color.red);

                            // 当たったものがゴールなら
                            if (_gimmick.Type == GameGimmick.GimmickType.Goal)
                                // クリア演出開始
                                GameMgr.Instance.FireGoal(_gimmick);
                        }
                    }

                    // 燃え尽きたことをマネージャーに通知
                    GameMgr.Instance.BurnOutFuse(this);
                }

                // 色を変えるオブジェクトの座標
                fuseModel.GetComponent<Renderer>().material.SetVector("_Target", target.position);
                // 燃やす範囲（0:その場だけ ～　1:全域）
                fuseModel.GetComponent<Renderer>().material.SetFloat("_Ration", (target.localScale.x + target.localScale.y + target.localScale.z) / 3);
            }
        }
        // UIなら
        else if (m_isUI)
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
        }
        // 濡れているとき乾いていく処理
        if(m_isWet)
        {
            gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.blue); // debug

            m_wetTime -= Time.deltaTime * GameMgr.Instance.GameSpeed;
            if(m_wetTime <= 0.0f)
            {
                m_isWet = false;
            }
        }
        else if (!m_isWet && !m_isBurn)
        {
            gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.white); // debug
        }
    }

    private void FixedUpdate()
    {
        if (m_isUI)
        {
            transform.localEulerAngles = new Vector3(m_defaultRot.x,
                m_defaultRot.y - Camera.main.transform.localEulerAngles.y, m_defaultRot.z);
        }
    }

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
        // UIかつ燃えていないものなら(濡れていたらも追加
        if (m_isUI || !m_isBurn || m_isWet)
            return;
             
        // 導火線との判定
        if (Utility.TagSeparate.getParentTagName(other.transform.tag) == NameDefine.TagName.Fuse)
        {
            Fuse _fuse = other.gameObject.GetComponent<Fuse>();

            // 相手が燃えているもしくは燃え尽きた後なら処理を飛ばす
            if (!_fuse || _fuse.m_isBurn || _fuse.m_isUI || _fuse.m_isWet)
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

    // Fuse回転
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

            transform.RotateAround(transform.position, new Vector3(0, -1, 0), fuseAngle);

            yield return null;
        }

        //回転中のフラグを倒す
        m_isRotate = false;
        yield return null;
    }

    // Fuse移動
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

    // Fuse濡れる
    public void FuseWet()
    {
        m_isWet = true;
        m_wetTime = AdjustParameter.Fuse_Constant.WET_MAX_TIME; 

        if (m_isBurn)
        {
            m_isBurn = false;

            // 燃える演出
            Transform fuseModel = transform.GetChild((int)FuseChild.Model);
            Transform target = transform.GetChild((int)FuseChild.Target);
            target.localScale = Vector3.one;
            target.position = transform.position;
            
            // 色を変えるオブジェクトの座標
            fuseModel.GetComponent<Renderer>().material.SetVector("_Target", target.position);
            // 燃やす範囲（0:その場だけ ～　1:全域）
            fuseModel.GetComponent<Renderer>().material.SetFloat("_Ration", (target.localScale.x + target.localScale.y + target.localScale.z) / 3);

            GameMgr.Instance.BurnOutFuse(this);
        }
    }
}
