﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectFuse : MonoBehaviour
{
    const float childScale = 0.1f;

    [SerializeField]
    private Texture2D m_fuseTex = null;

    private Vector3 m_targetDistance = Vector3.zero;

    private Transform m_childModel = null;
    private Renderer m_childRenderer = null;
    private Transform m_childTarget = null;
    private HashSet<GameObject> m_collObj = new HashSet<GameObject>();

    private bool m_isBurn = false;

    public bool Burn
    {
        get
        {
            return m_isBurn;
        }
        set
        {
            m_isBurn = value;
        }
    }

    private void Awake()
    {
        // 燃焼していないことをシェーダーにセット
        m_childModel = transform.GetChild(0);
        m_childTarget = transform.GetChild(1);
        m_childRenderer = m_childModel.GetComponent<Renderer>();
        m_childRenderer.material.SetTexture("_MainTex", m_fuseTex);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_targetDistance = Vector3.right / 2.0f;
        //// 導火線の燃えてきた方向にシェーダー用のオブジェクトを移動
        //Renderer modelRender = m_childModel.GetComponent<Renderer>();
        //m_childTarget.position = transform.position + m_targetDistance;
        //modelRender.material.SetVector("_Target", m_childTarget.position);
        //modelRender.material.SetVector("_Center", m_childModel.position);
    }

    // Update is called once per frame
    void Update()
    {
        // 色変更用オブジェクトが中心にいないなら
        if (m_isBurn && m_childTarget.localPosition != Vector3.zero)
        {
            float burnRate = Time.deltaTime / AdjustParameter.Fuse_Constant.BURN_MAX_TIME;
            m_childTarget.localScale += new Vector3(burnRate, burnRate, burnRate);

            // 移動
            m_childTarget.position -= (m_targetDistance / Mathf.Abs(Vector3.Dot(Vector3.one, m_targetDistance))) * burnRate * 0.5f;

            // 導火線と同じ大きさになったら
            if (m_childTarget.localScale.x >= 1.0f)
            {
                m_childTarget.localScale = Vector3.one;
                m_childTarget.position = transform.position;
            }

            // 色を変えるオブジェクトの座標
            m_childRenderer.material.SetVector("_Target", m_childTarget.position);
            // 燃やす範囲（0:その場だけ ～　1:全域）
            m_childRenderer.material.SetFloat("_Ration", Vector3.Dot(m_childTarget.localScale, Vector3.one) / 3);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 導火線との判定
        if (Utility.TagSeparate.getParentTagName(other.transform.tag) == NameDefine.TagName.Fuse)
        {
            SelectFuse _fuse = other.gameObject.GetComponent<SelectFuse>();

            // 相手が燃えているもしくは燃え尽きた後なら処理を飛ばす
            if (!_fuse || _fuse.m_isBurn)
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

}
