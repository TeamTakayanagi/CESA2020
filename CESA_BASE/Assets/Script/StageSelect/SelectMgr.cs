using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectMgr : SingletonMonoBehaviour<SelectMgr>
{
    private MainCamera m_camera = null;
    private GameObject m_uiArrow = null;
    private GameObject m_uiStartBack = null;

    private List<Stage> m_stages = new List<Stage>();
    private Stage m_zoomObj = null;

    public Stage ZoomObj
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
        m_camera.Type = MainCamera.CameraType.SwipeMove;
        m_camera.Control = true;

        m_stages.AddRange(StageMgr.Instance.GetComponentsInChildren<Stage>());

        // ステージ番号順にソート
        m_stages.Sort((a, b) => a.StageNum - b.StageNum);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_camera.Type == MainCamera.CameraType.ZoomIn)
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

    public void ClickArrow(int direct)
    {
        m_zoomObj = m_stages[Mathf.Clamp(m_zoomObj.StageNum + direct, 0, m_stages.Count - 1)];
        m_camera.ZoomIn(m_zoomObj.transform.position);
    }

    public void GameStart()
    {
        if (m_zoomObj.GetComponent<Renderer>().material.GetFloat("_texNum") > 0)
        {
            // シーン遷移開始
            SceneManager.LoadScene("SampleSceneSugi");
        }
    }
}
