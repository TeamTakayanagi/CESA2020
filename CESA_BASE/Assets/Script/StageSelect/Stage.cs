using System.Collections;
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

    }

    private void Update()
    {
        if (m_srep == 0)
        {
            if (int.Parse(SelectMgr.Instance.SaveData.data[m_stageNum][1]) > 0)
            {
                StartCoroutine("FireWorks");
            }

            m_srep++;
        }
    }

    private IEnumerator FireWorks()
    {
        while (true)
        {
            if (transform.childCount == 0)
                m_effekt = EffectManager.Instance.EffectCreate(Effekseer.EffekseerEmitter.EffectType.fireworks_core, transform.position, Quaternion.identity, transform);

            yield return new WaitForSeconds(5 + Random.Range(0, 3));
        }
    }
}
