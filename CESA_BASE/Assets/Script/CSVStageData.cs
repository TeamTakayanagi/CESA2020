using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CSVStageData : MonoBehaviour
{
    private List<int[]> m_stageData = new List<int[]>();

    private string m_saveDataPass = "/TextData/SaveData.csv";

    public List<int[]> StageData
    {
        get
        {
            return m_stageData;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool LoadSaveData()
    {
        Debug.Log(Application.dataPath + m_saveDataPass);
        StreamReader _strReader = new StreamReader(Application.dataPath + m_saveDataPass);
        
        while (_strReader.Peek() != -1)
        {
            // 1行持ってくる
            string _line = _strReader.ReadLine();
            // ','をトラッシュ
            string[] _str = _line.Split(',');
            // メモリ確保
            int[] _stageNum = new int[2];
            int.TryParse(_str[0], out _stageNum[0]);
            int.TryParse(_str[1], out _stageNum[1]);

            m_stageData.Add(_stageNum);
        }

        return true;
    }
}
