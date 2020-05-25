﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGimmick : MonoBehaviour
{
    public enum GimmickType
    {
        Goal,
        Water,
        Wall
    }

    [SerializeField]
    private GimmickType m_type = GimmickType.Goal;
    private bool m_isUI = false;

    // 水
    [SerializeField]
    private float m_gimmickValue = 0.0f;      // 水の長さ
    private bool m_isGimmickStart = false;

    public GimmickType Type
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
    public bool GimmickStart
    {
        get
        {
            return m_isGimmickStart;
        }
        set
        {
            m_isGimmickStart = value;
        }
    }
    public float Value
    {
        get
        {
            return m_gimmickValue;
        }
        set
        {
            m_gimmickValue = value;
        }
    }

    void Start()
    {
        if (m_type == GimmickType.Water)
            m_isGimmickStart = true;
        else
            m_isGimmickStart = false;
        m_gimmickValue = 0.0f;
    }

    void Update()
    {
        if (!m_isGimmickStart)
            return;

        if (m_type == GimmickType.Water)
            StartCoroutine(GimmickWater());

        else if(m_type == GimmickType.Goal)
        {
            m_gimmickValue += Time.deltaTime * GameMgr.Instance.GameSpeed;
            if(m_gimmickValue >= AdjustParameter.Fuse_Constant.BURN_MAX_TIME)
            {
                m_gimmickValue = AdjustParameter.Fuse_Constant.BURN_MAX_TIME;
                GameMgr.Instance.BurnCount -= 1;
                GameMgr.Instance.FireGoal(this, true);
                m_isGimmickStart = false;
            }
        }
    }

    public IEnumerator GimmickWater()
    {
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(transform.position, Vector3.forward, out hit, m_gimmickValue))
        {
            if (hit.collider.gameObject.CompareTag("Fuse"))
            {
                hit.collider.gameObject.GetComponent<Fuse>().FuseWet();
            }
        }

        yield break;
    }
}
