using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class CSVStageData : MonoBehaviour
{
    private string m_initStageDataText = "1,1\n2,1\n3,1\n4,1\n5,0\n6,0\n7,0\n8,0\n9,0\n10,0\n11,0\n12,0\n13,0\n14,0\n15,0\n16,0\n17,0\n18,0\n19,0\n20,0";

    private List<int[]> m_stageData = new List<int[]>();

    private string m_saveDataPath = "/TextData/SaveData.csv";

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
        if (!File.Exists(Application.dataPath + m_saveDataPath))
        {
            StreamWriter sw = new StreamWriter(Application.dataPath + m_saveDataPath, false, Encoding.GetEncoding("Shift_JIS"));

            sw.Write(m_initStageDataText);
            sw.Close();
        }

        StreamReader _strReader = new StreamReader(Application.dataPath + m_saveDataPath);
        
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
