﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage : MonoBehaviour
{
    Effekseer.EffekseerEmitter m_effekt = null;
    private Material m_myMaterial = null;

    private int m_srep = 0;
    private int m_stageNum = 0;
    private HashSet<GameObject> m_collObj = new HashSet<GameObject>();

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
        // クリア状態の格納
        int _saveData = int.Parse(SelectMgr.SaveData.data[m_stageNum - 1]);

        m_myMaterial = transform.GetComponent<Renderer>().material;
        m_myMaterial.SetFloat("_mono", _saveData);

        if (int.Parse(SelectMgr.SaveData.data[m_stageNum - 1]) > 0)
        {
            StartCoroutine("FireWorks");
            GetComponent<Renderer>().material.SetFloat("_mono", 1);
        }
        else
        {
            GetComponent<Renderer>().material.SetFloat("_mono", 0);
        }
    }

    private void Update()
    {
    }

    private IEnumerator FireWorks()
    {
        float _launchTiming = (Random.Range(0, 600) + Time.deltaTime * 30) / 60;
        yield return new WaitForSeconds(ProcessedtParameter.LaunchTiming.INIT + _launchTiming);
        while (true)
        {
            if (transform.childCount == 0)
            {
                m_effekt = EffectManager.Instance.EffectCreate(Effekseer.EffekseerEmitter.EffectType.fireworks_core, transform.position, 
                    new Vector3(transform.position.x, transform.position.y + AdjustParameter.Production_Constant.END_FIRE_POS_Y / 10, transform.position.z),
                    Vector3.one / 10, Quaternion.identity);
            }
            _launchTiming = (Random.Range(0, 600) + Time.deltaTime * 30) / 60;

            yield return new WaitForSeconds(ProcessedtParameter.LaunchTiming.NEXT + _launchTiming);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 導火線との判定
        if (Utility.TagSeparate.getParentTagName(other.transform.tag) == NameDefine.TagName.Fuse)
        {
            SelectFuse _fuse = other.gameObject.GetComponent<SelectFuse>();

            // 相手が燃えているもしくは燃え尽きた後なら処理を飛ばす
            if (!_fuse || _fuse.Burn)
                return;

            m_collObj.Add(_fuse.gameObject);
        }
    }
}
