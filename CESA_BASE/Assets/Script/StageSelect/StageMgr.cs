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
    private Utility.CSVFile.SaveData m_SaveData = new Utility.CSVFile.SaveData();
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
        // テスト---------------------
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!Utility.CSVFile.InitSaveData("SaveData"))
            {
                
            }

            m_step = (int)StageMgrState.LoadCsv;
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            if (!Utility.CSVFile.Save("SaveData", 0, (int.Parse(m_SaveData.data[0][1]) + 1) % 3))
            {
                Debug.Log("false");
            }

            m_step = (int)StageMgrState.LoadCsv;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (!Utility.CSVFile.Save("SaveData", 1, (int.Parse(m_SaveData.data[1][1]) + 1) % 3))
            {
                Debug.Log("false");
            }

            m_step = (int)StageMgrState.LoadCsv;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (!Utility.CSVFile.Save("SaveData", 2, (int.Parse(m_SaveData.data[2][1]) + 1) % 3))
            {
                Debug.Log("false");
            }

            m_step = (int)StageMgrState.LoadCsv;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (!Utility.CSVFile.Save("SaveData", 3, (int.Parse(m_SaveData.data[3][1]) + 1) % 3))
            {
                Debug.Log("false");
            }

            m_step = (int)StageMgrState.LoadCsv;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (!Utility.CSVFile.Save("SaveData", 4, (int.Parse(m_SaveData.data[4][1]) + 1) % 3))
            {
                Debug.Log("false");
            }

            m_step = (int)StageMgrState.LoadCsv;
        }
        //----------------------------
        //return;

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
                m_childRender[i].material.SetFloat("_texNum", float.Parse(m_SaveData.data[i][1]));
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
