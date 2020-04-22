using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class CSVScript : MonoBehaviour
{
    // 仮のステージ
    string[,,] m_stage = new string[,,] {
    {{ "A0", "A0", "A0", "A0", },
     { "A0", "--", "--", "A0", },
     { "A0", "A0", "--", "A0", },},

    {{ "A0", "A0", "A0", "A0", },
     { "A0", "--", "--", "A0", },
     { "A0", "--", "--", "A0", },},

    {{ "A0", "--", "--", "A0", },
     { "A0", "--", "--", "A0", },
     { "A0", "--", "--", "A0", },},
    };

    TextAsset csvFile;
    public List<List<string[]>> stageLList = new List<List<string[]>>();

    //string CsvPass = "Assets/Resources/StageData.csv";
    private string m_csvPath = "/TextData/StageData";
    string str;         // ステージデータ格納用

    private int m_stageNum = 0;

    public List<List<string[]>> Stage
    {
        get
        {
            return stageLList;
        }
    }

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // CSV読み込み
    public bool LoadCsv()
    {
        //csvFile = Resources.Load(@"StageData") as TextAsset;
        StreamReader reader = new StreamReader(Application.dataPath + m_csvPath + m_stageNum + ".csv");

        int _roop = 0;
        string line = reader.ReadLine();
        while (reader.Peek() != -1) // reader.Peaekが-1になるまで
        {
            stageLList.Add(new List<string[]>());
            while (line != "!n")
            {
                stageLList[_roop].Add(line.Split(','));
                line = reader.ReadLine();
            }
            line = reader.ReadLine();
            _roop++;

        }

        return true;
    }


    // CSV書き込み
    public bool WriteCsv(List<List<string[]>> _stage, string _stageName, int _sizeY)
    {
        for (int z = 0; z < _stage.Count; z++)
        {
            stageLList.Add(new List<string[]>());
            for (int y = 0; y < _stage[z].Count; y++)
            {
                stageLList[z].Add(new string[_sizeY]);
                for (int x = 0; x < _stage[z][y].Length; x++)
                {
                    stageLList[z][y][x] = _stage[z][y][x]; ;
                }
            }
        }

        StreamWriter sw = new StreamWriter(Application.dataPath + m_csvPath + _stageName + ".csv", false, Encoding.GetEncoding("Shift_JIS"));

        for (int z = 0; z < _stage.Count; z++)
        {
            for (int y = 0; y < _stage[z].Count; y++)
            {
                for (int x = 0; x < _stage[z][y].Length; x++)
                {
                    str = string.Join(",", stageLList[z][y]);
                    str += ',';
                }

                sw.WriteLine(str);
            }

            sw.WriteLine("!n");
        }

        sw.Close();

        return true;
    }

}
