﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Number : MonoBehaviour
{
    private const int NUMBER = 10;
    private const int COUNT = 60 * 2;
    [SerializeField]
    private Sprite[] texture2D = new Sprite[NUMBER];
    [SerializeField]
    private int m_texCount = 3;
    private int m_countDown = COUNT;

    private Image m_image = null;

    public int TexCount
    {
        get
        {
            return m_texCount;
        }
    }
    // Start is called before the first frame update
    void Awake()
    {
        m_image = GetComponent<Image>();
        m_image.sprite = texture2D[m_texCount];
    }

    // Update is called once per frame
    void Update()
    {
        m_countDown--;
        if (m_countDown <= 0)
        {
            m_texCount--;
            if (m_texCount >= 0)
            {
                m_image.sprite = texture2D[m_texCount];
                DestroyImmediate(this);
            }
            m_countDown = COUNT;
        }
    }
}
