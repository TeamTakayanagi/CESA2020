﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage : MonoBehaviour
{
    Effekseer.EffekseerEmitter m_effekt = null;

    private int m_srep = 0;
    private int m_stageNum = 0;

    public int StageNum
    {
        get
        {
            return m_stageNum;
        }
        set
        {
            m_stageNum = value;
        }
    }

    private void Start()
    {
        if (m_srep == 0)
        {
            if (int.Parse(SelectMgr.Instance.SaveData.data[m_stageNum - 1][1]) > 0)
            {
                StartCoroutine("FireWorks");
            }
            m_srep++;
        }
    }

    private void Update()
    {

    }

    private IEnumerator FireWorks()
    {
        while (true)
        {
            if (transform.childCount == 0)
            {
                m_effekt = EffectManager.Instance.EffectCreate(Effekseer.EffekseerEmitter.EffectType.fireworks_core, transform.position, 
                    new Vector3(transform.position.x, AdjustParameter.Production_Constant.END_FIRE_POS_Y / 10, transform.position.z),
                    Vector3.one / 10, Quaternion.identity);
            }
            yield return new WaitForSeconds(5 + Random.Range(0, 3));
        }
    }
}
