using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMgr : SingletonMonoBehaviour<StageMgr>
{
    private enum StageMgrState
    {
        LoadCsv = 0,
        ShaderSwitch,
        Max
    }

    private CSVStageData m_csvStageData = null;

    private Renderer[] m_childRender = null;

    private MainCamera m_camera = null;

    private Vector3 m_touchStartPos;
    private Vector3 m_touchEndPos;
    private Vector3 m_direction;

    private int m_step = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_csvStageData = SceneMgr.Instance.GetComponent<CSVStageData>();
        m_childRender = GetComponentsInChildren<Renderer>();
        m_camera = Camera.main.GetComponent<MainCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        // CSV読み込み
        if (m_step == (int)StageMgrState.LoadCsv)
        {
            if (m_csvStageData.LoadSaveData())
            {
                m_step++;
            }
        }

        // ステージの色を変える
        if (m_step == (int)StageMgrState.ShaderSwitch)
        {
            for (int i = 0; i < m_childRender.Length; i++)
            {
                m_childRender[i].material.SetFloat("_texNum", m_csvStageData.StageData[i][1]);
                //transform.GetChild(i).GetComponent<Renderer>().material.SetFloat("texNum", m_csvStageData.StageData[i][1]);
                //gameObject.GetComponentsInChildren<Renderer>()[m_csvStageData.StageData[i][0]].material.SetInt("TexNum", m_csvStageData.StageData[i][1]);
            }

            m_step++;
        }

        if (TitleMgr.Instance.Step < 7) return;

        if (m_camera.Zoom == 0)
        {
            Scroll();
            Camera.main.transform.position += m_direction;
        }
    }

    private void FixedUpdate()
    {
        m_direction *= 0.9f;
    }

    void Scroll()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_touchStartPos = new Vector3(Input.mousePosition.x / 500, 0, Input.mousePosition.y / 500);
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 _touchiNow = new Vector3(Input.mousePosition.x / 500, 0, Input.mousePosition.y / 500);

            {
                m_touchEndPos = _touchiNow;
                m_direction = m_touchStartPos - m_touchEndPos;
            }
        }
    }

}
