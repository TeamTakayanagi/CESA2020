using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectMgr : SingletonMonoBehaviour<SelectMgr>
{
    private MainCamera m_camera = null;
    private GameObject m_uiArrow = null;
    private GameObject m_uiStartBack = null;

    private Stage[] m_stages = null;
    private GameObject m_zoomObj = null;

    public GameObject ZoomObj
    {
        set
        {
            m_zoomObj = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_uiArrow = transform.GetChild(0).gameObject;
        m_uiStartBack = transform.GetChild(1).gameObject;
        m_camera = Camera.main.GetComponent<MainCamera>();
        m_stages = StageMgr.Instance.GetComponentsInChildren<Stage>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_camera.Zoom == 1)
        {

            if (!m_uiArrow.activeSelf)
                m_uiArrow.SetActive(true);
            if (!m_uiStartBack.activeSelf)
                m_uiStartBack.SetActive(true);

        }
        else
        {
            if (m_uiArrow.activeSelf)
                m_uiArrow.SetActive(false);
            if (m_uiStartBack.activeSelf)
                m_uiStartBack.SetActive(false);
        }
    }

    public void ClickArrow(bool _leftArrow)
    {
        if (_leftArrow)
        {
            for (int i = 0; i < m_stages.Length; i++)
            {
                if (m_stages[i].name == m_zoomObj.name)
                {
                    if (i == 0)
                    {
                        //m_camera.ZoomIn(m_stages[m_stages.Length-1].transform.position);
                        //m_zoomObj = m_stages[m_stages.Length-1].gameObject;
                    }
                    else
                    {
                        m_camera.ZoomIn(m_stages[i - 1].transform.position);
                        m_zoomObj = m_stages[i - 1].gameObject;
                    }
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < m_stages.Length; i++)
            {
                if (m_stages[i].name == m_zoomObj.name)
                {
                    if (i == m_stages.Length - 1)
                    {
                        //m_camera.ZoomIn(m_stages[0].transform.position);
                        //m_zoomObj = m_stages[0].gameObject;
                    }
                    else
                    {
                        m_camera.ZoomIn(m_stages[i + 1].transform.position);
                        m_zoomObj = m_stages[i + 1].gameObject;
                    }
                    break;
                }
            }
        }
    }

    public void GameStart()
    {
        //GameObject.FindGameObjectWithTag("Fade").GetComponent<FadeMgr>().StartFade("Alpha");
        if (m_zoomObj.GetComponent<Renderer>().material.GetFloat("_texNum") > 0)
        {
            // シーン遷移開始
            SceneManager.LoadScene("SampleSceneSugi");
        }
    }
}
