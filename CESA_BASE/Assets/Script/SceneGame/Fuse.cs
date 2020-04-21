using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fuse : MonoBehaviour
{
    public enum FuseType
    {
        Fuse,
        Start, 
        Goal,
        UI
    }

    [SerializeField]
    private FuseType m_type = FuseType.Fuse;
    private float m_burnTime = 0.0f;

    // 燃えているか
    private bool m_isBurn = false;
    private Vector3 m_endPos = Vector3.zero;
    private Vector3 m_defaultPos = Vector3.zero;
    private Vector3 m_defaultRot = Vector3.zero;

    public Vector3 EndPos
    {
        get
        {
            return m_endPos;
        }
        set
        {
            m_endPos = value;
            m_defaultPos =  transform.parent.position + value;
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
    public Vector3 DefaultRot
    {
        get
        {
            return m_defaultRot;
        }
    }

    private void Start()
    {
        m_defaultRot = transform.localEulerAngles;
        m_burnTime = ConstDefine.ConstParameter.BURN_MAX_TIME;  // 燃え尽きるまでの時間

        switch (m_type)
        {
            case FuseType.Start:
                {
                    gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                    m_isBurn = true;
                    break;
                }
            case FuseType.Fuse:
                {
                    gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
                    m_isBurn = false;
                    break;
                }
            case FuseType.Goal:
                {
                    gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
                    m_isBurn = false;
                    break;
                }
            case FuseType.UI:
                {
                    gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
                    m_isBurn = false;
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_type != FuseType.UI && m_isBurn)
        {
           // m_burnTime -= Time.deltaTime * GameMgr.Instance.GameSpeed;
            if (m_burnTime <= 0.0f)
            {
                // 燃え尽きた
                m_isBurn = false;
                m_type = FuseType.Fuse;
                gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
                // ここにゲーム管理の関数呼び出し
                GameMgr.Instance.BurnOutFuse(this);
            }
        }
        else if(m_type == FuseType.UI)
        {
            if (m_endPos != Vector3.zero)
            {
                transform.localPosition -= new Vector3(0.0f, 0.2f, 0.0f);
                if(transform.localPosition.y <= m_endPos.y)
                {
                    transform.localPosition = m_endPos;
                    m_endPos = Vector3.zero;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (m_type == FuseType.UI)
        {
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, - m_defaultRot.y - Camera.main.transform.localEulerAngles.y, transform.localEulerAngles.z);
        }
    }


    // 
    private void OnTriggerStay(Collider other)
    {
        // エディタ上なら
#if UNITY_EDITOR
        // UIかつ燃えていないものなら
        if (m_type == FuseType.UI || !m_isBurn)
            return;

        // 導火線との判定
        if (other.transform.tag == ConstDefine.TagName.Fuse)
        {
            Fuse _fuse = other.gameObject.GetComponent<Fuse>();
            // 相手が燃えているもしくは燃え尽きた後なら処理を飛ばす
            if (_fuse.m_isBurn || _fuse.m_burnTime <= 0.0f || _fuse.m_type == FuseType.UI)
                return;
            // 
            if (m_burnTime <= ConstDefine.ConstParameter.SPREAD_TIME)
            {
                _fuse.m_isBurn = true;
                _fuse.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                GameMgr.Instance.BurnCount += 1;
                // 当たったものがゴールなら
                if (_fuse.m_type == FuseType.Goal)
                {
                    // クリア演出開始
                }
            }
            else
            {
                _fuse.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
            }
        }
#else
        // UIかつ燃えていないものなら
        if (m_type == FuseType.UI || m_burnTime <= 1.0f)
            return;

        // 導火線との判定
        if (other.transform.tag == ConstDefine.TagName.Fuse)
        {
            // 
            if ()
            {
                Fuse _fuse = other.gameObject.GetComponent<Fuse>();
                // 相手が燃えているなら処理を飛ばす
                if (_fuse.m_isBurn || _fuse.m_type == FuseType.UI)
                    return;

                _fuse.m_isBurn = true;
                _fuse.GetComponent<Renderer>().material.SetColor("_Color", Color.red);

                // 当たったものがゴールなら
                if (_fuse.m_type == FuseType.Goal)
                {
                    SceneManager.LoadScene(ConstDefine.Scene.Clear);
                }
            }
        }
#endif
    }
}
