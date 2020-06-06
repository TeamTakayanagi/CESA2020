using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectFuse : FuseBase
{
    private SelectFuse m_nextFuse = null;

    public SelectFuse NextFuse
    {
        set
        {
            m_nextFuse = value;
        }
    }

    private void Awake()
    {
        // 燃焼していないことをシェーダーにセット
        m_childModel = transform.GetChild(0);
        m_childTarget = transform.GetChild(1);
        m_childRenderer = m_childModel.GetComponent<Renderer>();
        m_childRenderer.material.SetTexture("_MainTex", m_fuseTex);
        m_targetDistance = Vector3.zero;
        m_state = FuseState.None;
    }

    // Update is called once per frame
    void Update()
    {
        // 色変更用オブジェクトが中心にいないなら
        if (m_state == FuseState.Burn && m_childTarget.localPosition != Vector3.zero)
        {
            float burnRate = Time.deltaTime / AdjustParameter.Fuse_Constant.BURN_MAX_TIME;
            m_childTarget.localScale += new Vector3(burnRate, burnRate, burnRate);

            // 移動
            m_childTarget.position -= (m_targetDistance / Mathf.Abs(Vector3.Dot(Vector3.one, m_targetDistance))) * burnRate * 0.5f;

            // 導火線と同じ大きさになったら
            if (m_childTarget.localScale.x >= 1.0f)
            {
                m_childTarget.position = transform.position;
                m_childTarget.localScale = Vector3.one;
                m_state = FuseState.Out;

                // 次の導火線に引火
                if (m_nextFuse)
                {
                    SelectFuse _fuse = m_nextFuse;
                    _fuse.State = FuseState.Burn;

                    _fuse.m_targetDistance = new Vector3(
                        transform.position.x - _fuse.transform.position.x,
                        transform.position.y - _fuse.transform.position.y,
                        transform.position.z - _fuse.transform.position.z) * 0.5f;

                    // 導火線の燃えてきた方向にシェーダー用のオブジェクトを移動
                    _fuse.m_childTarget.position =
                        _fuse.transform.position + _fuse.m_targetDistance;
                    Spark.Instantiate(_fuse.transform.position + _fuse.m_targetDistance, _fuse.m_targetDistance * -2.0f, _fuse, 0);

                    // 導火線本体の中心座標を設定
                    Transform childModel = _fuse.m_childModel;
                    Renderer childRendere = _fuse.m_childRenderer;
                    childRendere.material.SetVector("_Center", childModel.position);
                }
            }

            // 色を変えるオブジェクトの座標
            m_childRenderer.material.SetVector("_Target", m_childTarget.position);
            // 燃やす範囲（0:その場だけ ～　1:全域）
            m_childRenderer.material.SetFloat("_Ration", Vector3.Dot(m_childTarget.localScale, Vector3.one) / 3);
        }
    }

    /// <summary>
    /// クリア演出
    /// </summary>
    public void BurnStart()
    {
        // 導火線の燃えてきた方向にシェーダー用のオブジェクトを移動
        Renderer modelRender = m_childModel.GetComponent<Renderer>();
        m_childTarget.position = transform.position + m_targetDistance;
        modelRender.material.SetVector("_Target", m_childTarget.position);
        modelRender.material.SetVector("_Center", m_childModel.position);
        Spark.Instantiate(transform.position + m_targetDistance, m_targetDistance * -2.0f, this, 0);
        m_state = FuseState.Burn;
    }

    public void BurnOut()
    {
        // 導火線の燃えてきた方向にシェーダー用のオブジェクトを移動
        Renderer modelRender = m_childModel.GetComponent<Renderer>();
        m_childTarget.position = transform.position;
        m_childTarget.localScale = Vector3.one;
        modelRender.material.SetVector("_Target", m_childTarget.position);
        modelRender.material.SetVector("_Center", m_childModel.position);
        m_childRenderer.material.SetFloat("_Ration", 1.0f);
        m_state = FuseState.Out;
    }

    public void SetTarget(Vector3 backPos)
    {
        Vector3 distance = transform.position - backPos;
        m_targetDistance = - Utility.MyMath.GetMaxDirectSign(distance) / 2.0f;
    }
}