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

    private EffectManager m_effectMgrClass = null;
    private List<Fuse> m_hitFuse = new List<Fuse>();

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
    public bool Burn
    {
        get {
            return m_isBurn;
        }
    }
    public float BurnTime
    {
        get {
            return m_burnTime;
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
        
        m_effectMgrClass = GameObject.FindObjectOfType<EffectManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_isUI && m_isBurn)
        {
            //m_burnTime -= Time.deltaTime * GameMgr.Instance.GameSpeed;
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
            transform.localRotation = Quaternion.AngleAxis(-30.0f, new Vector3(Mathf.Sin(m_defaultRot.z) * Mathf.Sin(m_defaultRot.y), 0.0f, 0.0f))
                                    * Quaternion.AngleAxis(m_defaultRot.y - Camera.main.transform.localEulerAngles.y,
                                                           new Vector3(0.0f, Mathf.Cos(m_defaultRot.z) * Mathf.Cos(m_defaultRot.x) * Mathf.Cos(m_defaultRot.y), 0.0f))
                                    * Quaternion.AngleAxis(0, Vector3.forward);

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
            if (_fuse.m_burnTime <= 0.0f || _fuse.m_isUI)
                return;

            // ここから下の処理は少し強引かもしれない
            // 相手が燃えていてもいなくても行う
            bool _sparkProcess = true;
            // 当たったことのある導火線なら火花の処理を飛ばす
            for (int _hitFuseCount = 0; _hitFuseCount < m_hitFuse.Count; _hitFuseCount++)
            {
                if (_fuse == m_hitFuse[_hitFuseCount])
                    _sparkProcess = false;
            }
            // 相手が半分以上燃えていないなら
            if (m_burnTime <= AdjustParameter.Fuse_Constant.SPREAD_TIME &&
                _fuse.m_burnTime > AdjustParameter.Fuse_Constant.BURN_MAX_TIME / 2 && _sparkProcess)
            {
                BoxCollider[] _boxCollider = other.GetComponents<BoxCollider>();
                for (int _boxColliderCnt = 0; _boxColliderCnt < _boxCollider.Length; _boxColliderCnt++)
                {
                    if (other.bounds.size == _boxCollider[_boxColliderCnt].bounds.size)
                    {
                        m_effectMgrClass.ChangeFuseClass(this.gameObject.GetComponent<Fuse>(), _fuse, _boxCollider[_boxColliderCnt]);
                        break;
                    }
                }
                m_hitFuse.Add(_fuse);
            }

            // 相手が燃えているときは処理を飛ばす
            if (_fuse.m_isBurn)
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
