using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage : MonoBehaviour
{
    Effekseer.EffekseerEmitter m_effekt = null;
    private Material m_myMaterial = null;

    private int m_stageNum = 0;                 // 自身のステージ番号
    private int m_clearState = 0;               // クリア状況
    private HashSet<GameObject> m_collObj = new HashSet<GameObject>();

    //---
    private GameObject m_stageText = null;
    private float posY;
    private float amplitude = 0.1f;
    public bool bZoom = false;

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
        set
        {
            m_clearState = value;
        }
    }

    private void Start()
    {
        // クリア状態の格納
        int _saveData = int.Parse(SelectMgr.SaveData.data[m_stageNum - 1]);

        m_myMaterial = transform.GetComponent<Renderer>().material;
        m_myMaterial.SetFloat("_mono", _saveData);

        if (m_clearState > 0)
        {
            StartCoroutine("FireWorks");
            GetComponent<Renderer>().material.SetFloat("_mono", 1);
        }
        else
        {
            GetComponent<Renderer>().material.SetFloat("_mono", 0);
        }

        //---
        m_stageText = transform.GetChild(0).gameObject;
        m_stageText.transform.localPosition = new Vector3(0f, 1f, 0f);
        posY = 1.0f;
    }

    private void Update()
    {
    }

    // テキストをふわふわさせる
    public IEnumerator MoveText()
    {
        m_stageText.transform.localPosition =
             new Vector3(m_stageText.transform.localPosition.x,
              posY + Mathf.PingPong(Time.time / 6, 0.1f),
               m_stageText.transform.localPosition.z);
        yield return null;
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
