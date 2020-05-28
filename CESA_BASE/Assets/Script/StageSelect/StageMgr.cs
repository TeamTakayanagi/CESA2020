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

    //private CSVStageData m_csvStageData = null;
    private Utility.CSVFile.BinData m_SaveData = new Utility.CSVFile.BinData();
    private Renderer[] m_childRender = null;
    private StageMgrState m_step = 0;

    void Start()
    {
        m_childRender = GetComponentsInChildren<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (!Utility.CSVFile.InitSaveData("SaveData"))
                {
                    Utility.CSVFile.InitSaveData("SaveData");
                }

                m_step = (int)StageMgrState.LoadCsv;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                if (!Utility.CSVFile.SaveBin("SaveData", 0, (int.Parse(m_SaveData.data[0][1]) + 1) % 3))
                {
                    Debug.Log("false");
                }

                m_step = (int)StageMgrState.LoadCsv;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (!Utility.CSVFile.SaveBin("SaveData", 1, (int.Parse(m_SaveData.data[1][1]) + 1) % 3))
                {
                    Debug.Log("false");
                }

                m_step = (int)StageMgrState.LoadCsv;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (!Utility.CSVFile.SaveBin("SaveData", 2, (int.Parse(m_SaveData.data[2][1]) + 1) % 3))
                {
                    Debug.Log("false");
                }

                m_step = (int)StageMgrState.LoadCsv;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (!Utility.CSVFile.SaveBin("SaveData", 3, (int.Parse(m_SaveData.data[3][1]) + 1) % 3))
                {
                    Debug.Log("false");
                }

                m_step = (int)StageMgrState.LoadCsv;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                if (!Utility.CSVFile.SaveBin("SaveData", 4, (int.Parse(m_SaveData.data[4][1]) + 1) % 3))
                {
                    Debug.Log("false");
                }

                m_step = (int)StageMgrState.LoadCsv;
            }
        }
#endif

        // Binaryファイル読込
        if (m_step == StageMgrState.LoadCsv)
        {
            //// セーブデータを読み込む
            //m_SaveData = Utility.CSVFile.LoadBin("SaveData");
            //if (m_SaveData != null)
            //{
            //    m_step++;
            //}
        }

        // ステージの色を変える
        else if (m_step == StageMgrState.ShaderSwitch)
        {
            for (int i = 0; i < m_childRender.Length; i++)
            {
                //m_childRender[i].material.SetFloat("_texNum", float.Parse(m_SaveData.data[i][1]));
            }

            m_step++;
        }
    }
}
