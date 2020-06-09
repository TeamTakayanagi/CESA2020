using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage : MonoBehaviour
{
    private const float TEXT_POS_Y = 1.0f;

    private Material m_myMaterial = null;

    private int m_stageNum = 0;                 // 自身のステージ番号
    private int m_clearState = 0;               // クリア状況
    private GameObject m_stageText = null;

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
    public int ClearState
    {
        get
        {
            return m_clearState;
        }
        set
        {
            m_clearState = value;
        }
    }

    private void Start()
    {
        m_myMaterial = transform.GetComponent<Renderer>().material;
        m_myMaterial.SetFloat("_mono", m_clearState);
        if (m_clearState < 0)
        {
            m_clearState *= -1;
            StartCoroutine("Clear");
            m_myMaterial.SetFloat("_texType", m_clearState - 1);
        }
        else if (m_clearState > 0)
        {
            StartCoroutine("FireWorks");
            m_myMaterial.SetFloat("_mono", 1);
            m_myMaterial.SetFloat("_texType", m_clearState - 1);
        }
        else
            m_myMaterial.SetFloat("_mono", 0);

        m_stageText = transform.GetChild(0).gameObject;
        m_stageText.GetComponent<TextMesh>().text += m_stageNum.ToString();
        m_stageText.transform.localPosition = new Vector3(0f, 1f, 0f);
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
            EffectManager.Instance.EffectCreate(Effekseer.EffekseerEmitter.EffectType.fireworks_core, transform.position,
                new Vector3(transform.position.x, transform.position.y + AdjustParameter.Production_Constant.END_FIRE_POS_Y / 10, transform.position.z),
                Vector3.one / 10, Quaternion.identity);
            _launchTiming = (Random.Range(0, 600) + Time.deltaTime * 30) / 60;

            yield return new WaitForSeconds(ProcessedtParameter.LaunchTiming.NEXT + _launchTiming);
        }
    }

    // テキストをふわふわさせる
    private IEnumerator MoveText()
    {
        while (true)
        {
            m_stageText.transform.localPosition =
                 new Vector3(m_stageText.transform.localPosition.x,
                  TEXT_POS_Y + Mathf.PingPong(Time.time / 6, 0.1f),
                   m_stageText.transform.localPosition.z);
            yield return null;
        }
    }

    public void MoveCoroutine(bool isStart)
    {
        if(isStart)
            StartCoroutine("MoveText");
        else
            StopCoroutine("MoveText");
    }

    public void OffText()
    {
        m_stageText.SetActive(false);
    }

    /// <summary>
    /// 最新のクリアステージを徐々に色を変える
    /// </summary>
    /// <returns></returns>
    private IEnumerator Clear()
    {
        float alpha = 0.0f;
        while (alpha < 1.0f)
        {
            m_myMaterial.SetFloat("_mono", alpha);
            alpha += Time.deltaTime / 5.0f;
            yield return null;
        }

        StartCoroutine("FireWorks");
        yield break;
    }
}