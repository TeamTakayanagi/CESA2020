using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutsideCanvas : MonoBehaviour
{
    private Camera m_camera = null;
    private RectTransform m_myRectTrans = null;

    private Rect m_myRect = new Rect(0, 0, 1, 1);
    private bool m_isVisible = true;        // キャンバス内でtrue

    public bool isVisible
    {
        get
        {
            return m_isVisible;
        }
    }

    void Start()
    {
        m_camera = Camera.main;
        m_myRectTrans = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        m_isVisible = m_myRect.Contains(m_camera.WorldToViewportPoint(m_myRectTrans.transform.position + (Vector3)(m_myRectTrans.rect.min * m_myRectTrans.lossyScale.x))) ||
                   m_myRect.Contains(m_camera.WorldToViewportPoint(m_myRectTrans.transform.position + (Vector3)(m_myRectTrans.rect.max * m_myRectTrans.lossyScale.x)));
    }
}
