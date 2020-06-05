using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : ClickedObject
{
    private float m_redian = 0;

    private int m_animeNum = 0;
    private int m_animeTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_animeTime++;
        if (m_animeTime > ProcessedtParameter.ClickObj.Tree.ANIME_DURATION)
        {
            m_animeNum = Random.Range(0, 5);
            if (m_animeNum == 0)
            {
                StartCoroutine("SwaysTree");
            }
            m_animeTime = 0;
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
        yield return null;
    }
}
