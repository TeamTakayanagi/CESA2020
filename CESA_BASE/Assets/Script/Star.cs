using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Star : MonoBehaviour
{
    private Image m_myImage = null;
    private Sprite m_mySprite = null;
    private Vector3 m_myScale = Vector3.zero;

    private bool m_updateFlg = false;

    // Start is called before the first frame update
    void Start()
    {
        m_myImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        if (m_updateFlg)
        {
            m_myImage.sprite = m_mySprite;
            transform.localScale = m_myScale;
            m_updateFlg = false;
        }
    }

    public void SetParam(Sprite _sprite, Vector3 _scale)
    {
        m_mySprite = _sprite;
        m_myScale = _scale;
        m_updateFlg = true;
    }


}
