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

    string CsvPass = "Assets/Resources/StageData.csv";
    string str;         // ステージデータ格納用

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
        csvFile = Resources.Load(@"StageData") as TextAsset;
        StringReader reader = new StringReader(csvFile.text);

        while (reader.Peek() != -1) // reader.Peaekが-1になるまで
        {
            string line = reader.ReadLine();
            for (int z = 0; z < 3; z++)
            {
                stageLList.Add(new List<string[]>());
                for (int y = 0; y < 3; y++)
                {
                    stageLList[z].Add(line.Split(','));
                    line = reader.ReadLine();
                }
                line = reader.ReadLine();
            }
        }

        // csvDatas[行][列]を指定して値を自由に取り出せる
        for (int z = 0; z < 3; z++)
        {
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    Debug.Log(stageLList[y][z][x]);
                }
            }
        }

        return true;
    }

    // CSV書き込み
    public bool WriteCsv()
    {
        for (int z = 0; z < 3; z++)
        {
            stageLList.Add(new List<string[]>());
            for (int y = 0; y < 3; y++)
            {
                stageLList[z].Add(new string[4]);
                for (int x = 0; x < 4; x++)
                {
                    stageLList[z][y][x] = m_stage[z, y, x];
                }
            }
        }

        StreamWriter sw = new StreamWriter(@CsvPass, false, Encoding.GetEncoding("Shift_JIS"));

        for (int z = 0; z < 3; z++)
        {
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 4; x++)
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
