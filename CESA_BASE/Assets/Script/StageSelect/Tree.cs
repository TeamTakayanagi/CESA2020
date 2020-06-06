using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : ClickedObject
{
    private float m_redian = 0;

    private int m_countTime = 0;

    void Start()
    {
        
    }

    void Update()
    {
        m_countTime++;
        if (m_countTime > ProcessedtParameter.ClickObj.Tree.ANIME_DURATION)
        {
            if (Random.Range(0, 50) == 0)
            {
                //EffectManager.Instance.EffectCreate(Effekseer.EffekseerEmitter.EffectType.Leafe, transform.position + new Vector3(0.0f, transform.localScale.y / 2.0f, 0.0f), Quaternion.identity);
            }
            m_countTime = 0;
        }
    }

    public override void OnClick()
    {   
        StartCoroutine("SwaysTree");
    }

    private IEnumerator SwaysTree()
    {
        while (m_redian < ProcessedtParameter.ClickObj.Tree.MAX_REDIAN)
        {
            m_redian += Time.deltaTime * ProcessedtParameter.ClickObj.Tree.SWAYS_SPEED;
            transform.localEulerAngles = Vector3.forward * ProcessedtParameter.ClickObj.Tree.SWAYS_ANGLE * Mathf.Sin(m_redian);
            yield return null;
        }

        transform.rotation = Quaternion.identity;
        m_redian = 0;
        StopCoroutine("SwaysTree");
        //EffectManager.Instance.EffectCreate(Effekseer.EffekseerEmitter.EffectType.Leafe,
       //     transform.position + new Vector3(0.0f, transform.localScale.y / 2.0f, 0.0f), Quaternion.identity);
        yield return null;
    }
}
