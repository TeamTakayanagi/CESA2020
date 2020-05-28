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

    }
}
