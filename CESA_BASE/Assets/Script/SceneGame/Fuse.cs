using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fuse : MonoBehaviour
{
    public enum FuseType
    {
        Normal,
        //UI,
        Start,
        Rotate,
        MoveX,
        MoveNX,
        MoveY,
        MoveNY,
        MoveZ,
        MoveNZ,
    }

    [SerializeField]
    private FuseType m_type = FuseType.Normal;
    private float m_burnTime = 0.0f;

    // 燃えているか
    private bool m_isBurn = false;
    private bool m_isUI = false;

    private Vector3 m_endPos = Vector3.zero;
    private Vector3 m_defaultPos = Vector3.zero;
    private Vector3 m_defaultRot = Vector3.zero;

    // 回転
    private Vector3 m_rotatePoint = Vector3.zero;   // 回転の中心
    private Vector3 m_rotateAxis = Vector3.zero;    // 回転軸
    private float m_fuseAngle = 0.0f;
    private bool m_isRotate = false;

    // 移動
    private Vector3 m_moveX = new Vector3(1f, 0, 0);
    private Vector3 m_moveNX = new Vector3(-1f, 0, 0);
    private Vector3 m_moveY = new Vector3(0, 1f, 0);
    private Vector3 m_moveNY = new Vector3(0, -1f, 0);
    private Vector3 m_moveZ = new Vector3(0, 0, 1f);
    private Vector3 m_moveNZ = new Vector3(0, 0, -1f);
    private float m_speed = 2f;
    private Vector3 m_movPos;   // 移動位置
    private Vector3 m_prevPos;  // 元の位置
    private bool m_isMoved = false;     // 移動済み
    private bool m_isMoving = false;    // 移動中

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

    private void Awake()
    {
    }

    private void Start()
    {
        m_defaultRot = transform.localEulerAngles;
        m_burnTime = AdjustParameter.Fuse_Constant.BURN_MAX_TIME;  // 燃え尽きるまでの時間
        m_wetTime = AdjustParameter.Fuse_Constant.WET_MAX_TIME;  // 水が乾くまでの時間
        switch (m_type)
        {
            case FuseType.Start:
                {
                    gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                    m_isBurn = true;
                    break;
                }
            case FuseType.Normal:
                {
                    gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.white);

                    m_isBurn = false;
                    break;
                }
        }
        // 元の位置を保存
        m_prevPos = transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        if (!m_isUI && m_isBurn)
        {
            m_burnTime -= Time.deltaTime * GameMgr.Instance.GameSpeed;
            if (m_burnTime <= 0.0f)
            {
                // 燃え尽きた
                m_isBurn = false;
                //m_type = FuseType.Fuse;
                gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
                // ここにゲーム管理の関数呼び出し
                GameMgr.Instance.BurnOutFuse(this);
            }
        }
        else if (m_isUI)
        {

            if (m_endPos != Vector3.zero)
            {
                transform.localPosition -= new Vector3(0.0f, 0.2f, 0.0f);
                if (transform.localPosition.y <= m_endPos.y)
                {
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
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x,
                m_defaultRot.y - Camera.main.transform.localEulerAngles.y, transform.localEulerAngles.z);
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

    // 
    private void OnTriggerStay(Collider other)
    {
        // UIかつ燃えていないものなら(濡れていたらも追加
        if (m_isUI || !m_isBurn || m_isWet)
            return;
             
        // 導火線との判定
        if (Utility.TagSeparate.getParentTagName(other.transform.tag) == NameDefine.TagName.Fuse)
        {
            Fuse _fuse = other.gameObject.GetComponent<Fuse>();
            // 相手が燃えているもしくは燃え尽きた後なら処理を飛ばす(相手が濡れていたらも追加
            if (_fuse.m_isBurn || _fuse.m_burnTime <= 0.0f || _fuse.m_isUI || _fuse.m_isWet)
                return;
            // 
            if (m_burnTime <= AdjustParameter.Fuse_Constant.SPREAD_TIME)
            {
                _fuse.m_isBurn = true;
                _fuse.GetComponent<Renderer>().material.SetColor("_Color", Color.red);

                GameMgr.Instance.BurnCount += 1;
            }
            else
            {
                _fuse.GetComponent<Renderer>().material.SetColor("_Color", Color.green);

            }
        }
        else if (Utility.TagSeparate.getParentTagName(other.transform.tag) == NameDefine.TagName.Gimmick)
        {
            GameGimmick _gimmick = other.gameObject.GetComponent<GameGimmick>();

            // 水は燃えないので
            if (_gimmick.Type == GameGimmick.GimmickType.Water)
                return;

            _gimmick.GetComponent<Renderer>().material.SetColor("_Color", Color.red);

            if(_gimmick.Type == GameGimmick.GimmickType.SubMission)
            {
                // スコアの処理とか？
            }

            // 当たったものがゴールなら
            if (_gimmick.Type == GameGimmick.GimmickType.Goal)
            {
                // クリア演出開始
                GameMgr.Instance.FireGoal(_gimmick);
            }
        }
    }

    public void OnGimmick()
    {
        switch (m_type)
        {
            case FuseType.Rotate:
                {
                    m_rotatePoint = transform.position;
                    m_rotateAxis = new Vector3(0, -1, 0);
                    if(!m_isRotate)
                        StartCoroutine(RotateFuse());
                    break;
                }
            case FuseType.MoveX:
                {
                    if (transform.position == m_prevPos)
                    {
                        m_isMoved = false;
                    }
                    else
                    {
                        m_isMoved = true;
                    }

                    if (m_isMoved)
                    {
                        m_movPos = m_prevPos;
                        StartCoroutine(MoveFuse(m_prevPos, m_moveNX));
                    }
                    else
                    {
                        m_movPos = m_prevPos + m_moveX;
                        StartCoroutine(MoveFuse(m_movPos, m_moveX));
                    }
                    break;
                }
            case FuseType.MoveNX:
                {
                    if (transform.position == m_prevPos)
                    {
                        m_isMoved = false;
                    }
                    else
                    {
                        m_isMoved = true;
                    }

                    if (m_isMoved)
                    {
                        m_movPos = m_prevPos;
                        StartCoroutine(MoveFuse(m_movPos, m_moveX));

                    }
                    else
                    {
                        m_movPos = m_prevPos + m_moveNX;
                        StartCoroutine(MoveFuse(m_movPos, m_moveNX));
                    }
                    break;
                }
            case FuseType.MoveY:
                {
                    if (transform.position == m_prevPos)
                    {
                        m_isMoved = false;
                    }
                    else
                    {
                        m_isMoved = true;
                    }

                    if (m_isMoved)
                    {
                        m_movPos = m_prevPos;
                        StartCoroutine(MoveFuse(m_movPos, m_moveNY));
                    }
                    else
                    {
                        m_movPos = m_prevPos + m_moveY;
                        StartCoroutine(MoveFuse(m_movPos, m_moveY));
                    }
                    break;
                }
            case FuseType.MoveNY:
                {
                    if (transform.position == m_prevPos)
                    {
                        m_isMoved = false;
                    }
                    else
                    {
                        m_isMoved = true;
                    }

                    if (m_isMoved)
                    {
                        m_movPos = m_prevPos;
                        StartCoroutine(MoveFuse(m_movPos, m_moveY));
                    }
                    else
                    {
                        m_movPos = m_prevPos + m_moveNY;
                        StartCoroutine(MoveFuse(m_movPos, m_moveNY));
                    }
                    break;
                }
            case FuseType.MoveZ:
                {
                    if (transform.position == m_prevPos)
                    {
                        m_isMoved = false;
                    }
                    else
                    {
                        m_isMoved = true;
                    }

                    if (m_isMoved)
                    {
                        m_movPos = m_prevPos;
                        StartCoroutine(MoveFuse(m_movPos, m_moveNZ));
                    }
                    else
                    {
                        m_movPos = m_prevPos + m_moveZ;
                        StartCoroutine(MoveFuse(m_movPos, m_moveZ));
                    }
                    break;
                }
            case FuseType.MoveNZ:
                {
                    if (transform.position == m_prevPos)
                    {
                        m_isMoved = false;
                    }
                    else
                    {
                        m_isMoved = true;
                    }

                    if (m_isMoved)
                    {
                        m_movPos = m_prevPos;
                        StartCoroutine(MoveFuse(m_movPos, m_moveZ));
                    }
                    else
                    {
                        m_movPos = m_prevPos + m_moveNZ;
                        StartCoroutine(MoveFuse(m_movPos, m_moveNZ));
                    }
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    // Fuse回転
    public IEnumerator RotateFuse()
    {
        //回転中のフラグを立てる
        m_isRotate = true;

        //回転処理
        float sumAngle = 0f; //angleの合計を保存
        while (sumAngle < 90f)
        {
            m_fuseAngle = 5f; //ここを変えると回転速度が変わる
            sumAngle += m_fuseAngle;

            // 90度以上回転しないように値を制限
            if (sumAngle > 90f)
            {
                m_fuseAngle -= sumAngle - 90f;
            }
            transform.RotateAround(m_rotatePoint, m_rotateAxis, m_fuseAngle);

            yield return null;
        }
        //回転中のフラグを倒す
        m_isRotate = false;
        yield return null;
    }

    // Fuse移動
    public IEnumerator MoveFuse(Vector3 target, Vector3 direction)
    {
        m_isMoving = true;

        RaycastHit hit = new RaycastHit();
        float dist = 1f;

        if (!(Physics.Raycast(transform.position, direction, out hit, dist)))
        {
            float sum = 0f;

            while (sum < 1f)
            {
                sum += m_speed * Time.deltaTime;

                transform.position = Vector3.MoveTowards(transform.position, target, m_speed * Time.deltaTime);
                yield return null;
            }
        }

        m_isMoving = false;

        yield break;
    }

    // Fuse濡れる
    public void FuseWet()
    {
        m_isWet = true;
        m_wetTime = AdjustParameter.Fuse_Constant.WET_MAX_TIME; 

        gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.blue); // debug

        if (m_isBurn)
        {
            m_isBurn = false;
            m_burnTime = 0.0f;
            GameMgr.Instance.BurnOutFuse(this);
        }
    }

}
