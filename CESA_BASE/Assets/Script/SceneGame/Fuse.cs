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
        // UIかつ燃えていないものなら
        if (m_isUI || !m_isBurn)
            return;

        // 導火線との判定
        if (Utility.TagSeparate.getParentTagName(other.transform.tag) == NameDefine.TagName.Fuse)
        {
            Fuse _fuse = other.gameObject.GetComponent<Fuse>();
            // 相手が燃えているもしくは燃え尽きた後なら処理を飛ばす
            if (_fuse.m_isBurn || _fuse.m_burnTime <= 0.0f || _fuse.m_isUI)
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

            // 当たったものがゴールなら
            if (_gimmick.Type == GameGimmick.GimmickType.Goal)
            {
                // クリア演出開始
                GameMgr.Instance.FireGoal(_gimmick);
            }
        }
    }
}
