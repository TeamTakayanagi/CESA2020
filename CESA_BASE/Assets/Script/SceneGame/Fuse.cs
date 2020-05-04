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
    }
    public enum FuseChild
    {
        Model,
        Target,
    }

    [SerializeField]
    private FuseType m_type = FuseType.Normal;

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
        // 燃焼していないことをシェーダーにセット
        Transform fuseModel = transform.GetChild((int)FuseChild.Model);
        Transform target = transform.GetChild((int)FuseChild.Target);
        // 導火線本体の中心座標を設定
        fuseModel.GetComponent<Renderer>().material.SetVector("_Center", fuseModel.position);
        if (m_type == FuseType.Start)
        {
            m_isBurn = true;

            m_targetDistance = new Vector3(0.0f, -0.5f, 0.0f);

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
        }
    }

    private void Start()
    {
        m_defaultRot = transform.localEulerAngles;
    }

    // Update is called once per frame
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
                target.position -= m_targetDistance * Time.deltaTime / AdjustParameter.Fuse_Constant.BURN_MAX_TIME * 0.5f

                // 中心を通り過ぎたら
                if (target.localScale.x > 1.0f)
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
                transform.localPosition -= new Vector3(0.0f, AdjustParameter.UI_Fuse_Constant.MOVE_VALUE_Y, 0.0f);
                // 範囲処理
                if (transform.localPosition.y <= m_endPos.y)
                {
                    // 移動完了
                    transform.localPosition = m_endPos;
                    m_endPos = Vector3.zero;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (m_isUI)
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x,
                m_defaultRot.y - Camera.main.transform.localEulerAngles.y, transform.localEulerAngles.z);
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
        // UIかつ燃えていないものなら
        if (m_isUI || !m_isBurn)
            return;

        // 導火線との判定
        if (Utility.TagSeparate.getParentTagName(other.transform.tag) == NameDefine.TagName.Fuse)
        {
            Fuse _fuse = other.gameObject.GetComponent<Fuse>();

            // 相手が燃えているもしくは燃え尽きた後なら処理を飛ばす
            if (!_fuse || _fuse.m_isBurn || _fuse.m_isUI)
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
}
