using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectFuse : MonoBehaviour
{
    [SerializeField]
    private Texture2D m_fuseTex = null;

    private Transform m_childModel = null;
    private Renderer m_childRenderer = null;
    private Transform m_childTarget = null;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
