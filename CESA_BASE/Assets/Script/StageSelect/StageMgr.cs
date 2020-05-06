using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMgr : SingletonMonoBehaviour<StageMgr>
{
    const float MOVE_RESIST = 0.9f;

    private enum StageMgrState
    {
        LoadCsv = 0,
        ShaderSwitch,
        Max
    }

    //private CSVStageData m_csvStageData = null;
    private Utility.CSVFile.CSVData m_SaveData = new Utility.CSVFile.CSVData();
    private Renderer[] m_childRender = null;
    private Vector3 m_touchStartPos;
    private Vector3 m_touchEndPos;
    private Vector3 m_direction;

    private int m_step = 0;

    // Start is called before the first frame update
    void Start()
    {
        //m_csvStageData = SelectMgr.Instance.GetComponent<CSVStageData>();
        m_childRender = GetComponentsInChildren<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Binaryファイル読込
        if (m_step == (int)StageMgrState.LoadCsv)
        {
            // セーブデータを読み込む
            m_SaveData = Utility.CSVFile.LoadBin("SaveData");
            if (m_SaveData != null)
            {
                m_step++;
            }
        }

        // ステージの色を変える
        if (m_step == (int)StageMgrState.ShaderSwitch)
        {
            for (int i = 0; i < m_childRender.Length; i++)
            {
                m_childRender[i].material.SetFloat("_texNum", float.Parse(m_SaveData.data[i]));
            }

            m_step++;
        }

        if (TitleMgr.Instance.Step < 7) 
            return;

        if (Camera.main.GetComponent<MainCamera>().Zoom == 0)
        {
            Scroll();
            Camera.main.transform.position += m_direction;
        }
    }

    private void FixedUpdate()
    {
        m_direction *= MOVE_RESIST;
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
