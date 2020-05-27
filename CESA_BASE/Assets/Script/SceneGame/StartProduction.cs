﻿using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartProduction : MonoBehaviour
{
    public enum Production
    {
        wait,
        move, 
        end
    }

    GameObject m_fireworks = null;
    GameObject m_fuse = null;
    GameObject m_fire = null;
    Vector3 m_defaultPos;
    Production m_state = Production.wait;

    public Production State
    {
        set
        {
            m_state = value;
        }
        get
        {
            return m_state;
        }
    }

    void Awake()
    {
        m_state = Production.wait;
    }
    // Start is called before the first frame update
    void Start()
    {
        // 演出用の子オブジェクト取得
        m_fuse = transform.GetChild(0).gameObject;
        m_fireworks = transform.GetChild(1).gameObject;
        m_fire = transform.GetChild(2).gameObject;

        m_defaultPos = m_fire.transform.localPosition;
        m_fuse.GetComponent<Image>().material.SetFloat("_OutTime", 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_state != Production.move)
            return;

        m_fire.transform.localPosition += new Vector3(AdjustParameter.Production_Constant.START_TIME, 0.0f, 0.0f) * Time.deltaTime;
        if (m_fire.transform.localPosition.x >= m_fireworks.transform.localPosition.x)
            m_state = Production.end;

        m_fuse.GetComponent<Image>().material.SetFloat("_OutTime",
            Mathf.Clamp01((m_fire.transform.localPosition - m_defaultPos).x / (m_fireworks.transform.localPosition - m_defaultPos).x));
    }
}
