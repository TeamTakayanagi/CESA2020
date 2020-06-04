using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fuse : MonoBehaviour
{
    const float childScale = 0.01f;
    public enum FuseType
    {
        Normal,
        Start,
        Rotate,
        MoveLeft,
        MoveRight,
        MoveUp,
        MoveDown,
        MoveBack,
        MoveForward,
    }

    public enum FuseState
    {
        None,       // 場に置かれている
        UI,         // UIになっている
        Burn,       // 燃えている
        Out,        // 燃え尽きた後
        Wet         // 濡れている
    }

    private readonly Vector3 OUTPOS = new Vector3(-50, -50, -50);           // 導火線を生成できない位置

    [SerializeField]
    private FuseType m_type = FuseType.Normal;
    [SerializeField]
    private Texture2D m_fuseTex = null;

    private FuseState m_state = FuseState.None;
    private Transform m_childModel = null;
    private Renderer m_childRenderer = null;
    private Transform m_childTarget = null;

    private float m_countTime = 0.0f;                                       // 汎用カウント変数
    private Vector3 m_targetDistance = Vector3.zero;
    private HashSet<GameObject> m_collObj = new HashSet<GameObject>();
    private List<Spark> m_haveEffect = new List<Spark>();                   // この導火線についているエフェクト   

    // UI用
    private Vector3 m_endPos = Vector3.zero;
    private Vector3 m_defaultPos = Vector3.zero;
    private Vector3 m_defaultRot = Vector3.zero;

    private bool m_isRotate = false;

    public float CountTime
    {
        get
        {
            return m_countTime;
        }
        set
        {
            m_countTime = value;
        }
    }
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
    public Transform ChildModel
    {
        get
        {
            return m_childModel;
        }
    }
    public Renderer ChildRendrer
    {
        get
        {
            return m_childRenderer;
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
    public List<BoxCollider> HaveEffect(Spark spark)
    {
        List<Spark> sparkList = m_haveEffect;
        //sparkList.Remove(spark);
        List<BoxCollider> collList = new List<BoxCollider>();
        for(int i = 0; i < sparkList.Count; ++i)
        {
            collList.AddRange(sparkList[i].GetComponents<BoxCollider>());
        }
        return collList;
    }
    /// <summary>
    /// 導火線に着火
    /// </summary>
    public void GameStart()
    {
        m_state = FuseState.Burn;
        Spark spark = Spark.Instantiate(transform.position + m_targetDistance, m_targetDistance * -2.0f, this, m_haveEffect.Count);
        Renderer modelRender = m_childModel.GetComponent<Renderer>();
        // 導火線の燃えてきた方向にシェーダー用のオブジェクトを移動
        m_childTarget.position = transform.position + TargetDistance;
        modelRender.material.SetVector("_Target", m_childTarget.position);
        modelRender.material.SetVector("_Center", m_childModel.position);
        m_haveEffect.Add(spark);
    }

    private void Awake()
    {
        // 水が乾くまでの時間
        m_countTime = AdjustParameter.Fuse_Constant.WET_MAX_TIME;

        // 燃焼していないことをシェーダーにセット
        m_childModel = transform.GetChild(0);
        m_childTarget = transform.GetChild(1);
        m_childRenderer = m_childModel.GetComponent<Renderer>();
        m_childRenderer.material.SetTexture("_MainTex", m_fuseTex);
    }

    private void Start()
    {
        m_defaultRot = transform.localEulerAngles;

        Renderer modelRender = m_childModel.GetComponent<Renderer>();
        // 導火線本体の中心座標を設定
        modelRender.material.SetVector("_Center", m_childModel.position);
        m_childTarget.localScale = new Vector3(childScale, childScale, childScale);

        if (m_type == FuseType.Start)
        {
            Vector3 entry = Vector3.zero;
            BoxCollider[] coliders = GetComponents<BoxCollider>();
            for (int i = 0; i < coliders.Length; ++i)
            {
                // 長いコライダー
                if (coliders[i].center == Vector3.zero)
                {
                    // 
                    Vector3 direct = transform.rotation * coliders[i].size;
                    float max = Mathf.Max(direct.x, direct.y, direct.z);
                    direct = direct - new Vector3(max, max, max);
                    direct = new Vector3(Mathf.Clamp01(Mathf.Floor(direct.x + 1)),
                        Mathf.Clamp01(Mathf.Floor(direct.y + 1)), Mathf.Clamp01(Mathf.Floor(direct.z + 1)));

                    // 
                    entry = Vector3.Dot(transform.position, direct) * direct;
                    if(entry != Vector3.zero)
                        break;
                }
                // 短いコライダー
                else
                {
                    Vector3 center = coliders[i].center;
                    Vector3 absolute = new Vector3(Mathf.Clamp01(Mathf.Floor(center.x + 1)),
                        Mathf.Clamp01(Mathf.Floor(center.y + 1)), Mathf.Clamp01(Mathf.Floor(center.z + 1)));
                    if (Vector3.Dot(transform.position, absolute) * Vector3.Dot(center, absolute) > 0)
                    {
                        entry = Vector3.Dot(center, absolute) * absolute;
                        break;
                    }
                }
            }
            m_state = FuseState.None;
            m_targetDistance = new Vector3(Mathf.Clamp(entry.x, -1, 1),
                                            Mathf.Clamp(entry.y, -1, 1),
                                            Mathf.Clamp(entry.z, -1, 1)) / 2.0f;
        }
        else
        {
            // 色を変えるオブジェクトの座標
            modelRender.material.SetVector("_Target", OUTPOS);
            // 燃やす範囲（0:その場だけ ～　1:全域）
            modelRender.material.SetFloat("_Ration", 0);
        }

        // 元の位置を保存
        if (m_state != FuseState.UI)
        m_defaultPos = transform.position;
    }

    void Update()
    {
        switch(m_state)
        {
            case FuseState.UI:
                {
                    // 移動完了していないなら
                    if (m_endPos != Vector3.zero)
                    {
                        // 下へ落下
                        transform.localPosition -= new Vector3(0.0f, AdjustParameter.UI_Object_Constant.MOVE_VALUE_Y, 0.0f);
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
                        float burnRate = Time.deltaTime * GameMgr.Instance.GameSpeed / AdjustParameter.Fuse_Constant.BURN_MAX_TIME;
                        m_childTarget.localScale += new Vector3(burnRate, burnRate, burnRate);

                        // 移動
                        m_childTarget.position -= (m_targetDistance / Mathf.Abs(Vector3.Dot(Vector3.one, m_targetDistance))) * burnRate * 0.5f;

                        // 導火線と同じ大きさになったら
                        if (m_childTarget.localScale.x >= 1.0f)
                        {
                            m_childTarget.localScale = Vector3.one;
                            m_childTarget.position = transform.position;

                            // 当たっている導火線に引火
                            foreach (GameObject _obj in m_collObj)
                            {
                                if (Utility.TagSeparate.getParentTagName(_obj.transform.tag) == NameDefine.TagName.Fuse)
                                {
                                    Fuse _fuse = _obj.GetComponent<Fuse>();
                                    _fuse.State = FuseState.Burn;
                                    GameMgr.Instance.BurnCount += 1;

                                    _fuse.TargetDistance = new Vector3(
                                        transform.position.x - _fuse.transform.position.x,
                                        transform.position.y - _fuse.transform.position.y,
                                        transform.position.z - _fuse.transform.position.z) * 0.5f;

                                    // 導火線の燃えてきた方向にシェーダー用のオブジェクトを移動
                                    _fuse.ChildTarget.position =
                                        _fuse.transform.position + _fuse.TargetDistance;

                                    // エフェクトも同じ場所に生成
                                    Spark spark = Spark.Instantiate(_fuse.transform.position + _fuse.TargetDistance,
                                        _fuse.TargetDistance * - 2.0f, _fuse, _fuse.m_haveEffect.Count);
                                    _fuse.m_haveEffect.Add(spark);

                                    // 導火線本体の中心座標を設定
                                    Transform childModel = _fuse.ChildModel;
                                    Renderer childRendere = _fuse.ChildRendrer;
                                    childRendere.material.SetVector("_Center", childModel.position);
                                }
                                else if (Utility.TagSeparate.getParentTagName(_obj.transform.tag) == NameDefine.TagName.Gimmick)
                                {
                                    GameGimmick _gimmick = _obj.gameObject.GetComponent<GameGimmick>();

                                    // 当たったものがゴールなら
                                    if (_gimmick.Type == GameGimmick.GimmickType.Goal && !_gimmick.GimmickStart)
                                    {
                                        // 燃えたゴールを登録
                                        GameMgr.Instance.FireGoal(false);
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
                        m_childRenderer.material.SetVector("_Target", m_childTarget.position);
                        // 燃やす範囲（0:その場だけ ～　1:全域）
                        m_childRenderer.material.SetFloat("_Ration", Vector3.Dot(m_childTarget.localScale, Vector3.one) / 3 * childScale * AdjustParameter.Fuse_Constant.BURN_MAX_TIME);
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
                    m_childRenderer.material.SetFloat("_OutTime", m_countTime / AdjustParameter.Fuse_Constant.OUT_MAX_TIME);
                    // 一定時間たったら消す
                    if (m_countTime <= 0)
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

            m_collObj.Add(_fuse.gameObject);
        }
        else if (Utility.TagSeparate.getParentTagName(other.transform.tag) == NameDefine.TagName.Gimmick)
        {
            GameGimmick _gimmick = other.gameObject.GetComponent<GameGimmick>();

            // 水は燃えないので
            if (_gimmick.Type == GameGimmick.GimmickType.Water)
                return;

            m_collObj.Add(_gimmick.gameObject);
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
            StartCoroutine(MoveFuse(movePos, -direct));
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
        float dist = 0.5f;

        Debug.DrawRay(transform.position, direction, Color.red, dist);
        if (!Physics.Raycast(transform.position + direction / 2, direction, out hit, dist))
        {
            float sum = 0.0f;
            while (sum < 1.0f)
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
            m_childRenderer.material.SetVector("_Target", m_childTarget.position);
            // 燃やす範囲（0:その場だけ ～　1:全域）
            m_childRenderer.material.
                SetFloat("_Ration", (m_childTarget.localScale.x + m_childTarget.localScale.y + m_childTarget.localScale.z) / 3);

            GameMgr.Instance.BurnOutFuse();
        }
    }
}
