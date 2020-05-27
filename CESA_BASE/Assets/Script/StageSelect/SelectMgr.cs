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
        for(int i = 0; i < m_stages.Count; ++i)
        {
            m_stages[i].StageNum = i;
        }

        m_uiArrow.SetActive(false);
        m_uiStartBack.SetActive(false);

        // ステージ番号順にソート
        m_stages.Sort((a, b) => a.StageNum - b.StageNum);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_camera.Type == MainCamera.CameraType.SwipeMove)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit _hit = new RaycastHit();
                Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(_ray, out _hit))
                {
                    // ステージとの判定
                    if (_hit.transform.tag == NameDefine.TagName.Stage)
                    {
                        for (int i = 0; i < m_stages.Count; ++i)
                        {
                            Stage _stage = m_stages[i];
                            if (_hit.transform != _stage.transform)
                                continue;

                            m_zoomObj = _stage;
                            m_camera.StartZoomIn(_stage.transform.position);

                            m_uiArrow.SetActive(true);
                            m_uiStartBack.SetActive(true);
                        }
                    }

                    // 背景オブジェクトとの判定
                    if (_hit.transform.root.GetComponent<BGObjs>())
                    {
                        BGObjs _bgObjects = BGObjs.Instance.GetComponent<BGObjs>();
                        for (int i = 0; i < _bgObjects.transform.childCount; i++)
                        {
                            for (int j = 0; j < _bgObjects.transform.GetChild(i).childCount; j++)
                            {
                                if (_hit.transform == _bgObjects.transform.GetChild(i).GetChild(j))
                                {
                                    _hit.transform.GetComponent<ClickedObject>().OnClick();
                                }
                            }
                        }
                    }
                }
            }
        }

    }

    /// <summary>
    /// 矢印をクリック
    /// </summary>
    /// <param name="direct">右（1）左（-1）</param>
    public void ClickArrow(int direct)
    {
        m_zoomObj = m_stages[Mathf.Clamp(m_zoomObj.StageNum + direct, 0, m_stages.Count - 1)];
        m_camera.StartZoomIn(m_zoomObj.transform.position);
    }

    public void ZoomOut()
    {
        m_uiArrow.SetActive(false);
        m_uiStartBack.SetActive(false);
        m_camera.StartZoomOut();
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
