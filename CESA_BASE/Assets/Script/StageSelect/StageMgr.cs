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
        // Binaryファイル読込
        if (m_step == StageMgrState.LoadCsv)
        {
            // セーブデータを読み込む
            m_SaveData = Utility.CSVFile.LoadBin("SaveData");
            if (m_SaveData != null)
            {
                m_step++;
            }
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
