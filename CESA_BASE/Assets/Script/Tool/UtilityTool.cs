using System.IO;
using System.Text;
using UnityEngine;
using System.Collections.Generic;

namespace Utility
{
    /// <summary>
    /// タグを取得するクラス 
    /// </summary>
    public static class TagSeparate
    {
        public static string getParentTagName(string name)
        {
            int pos = name.IndexOf("/");

            if (0 < pos)
                return name.Substring(0, pos);
            else
                return name;
        }

        public static string getChildTagName(string name)
        {
            int pos = name.IndexOf("/");

            if (0 < pos)
                return name.Substring(pos + 1);
            else
                return name;
        }
    }

    /// <summary>
    /// CSVファイルの操作クラス
    /// </summary>
    public static class CSVFile
    {
        private const string CSV_PATH = "/TextData/";
        public class CSVData
        {
            public Vector3Int size;
            public List<string> data;
        }

        public static int PosToIndex(Vector3 pos, int stageSizeX, int stageSizeY)
        {
            return stageSizeY * stageSizeX * (int)pos.z +
                stageSizeX * (int)(stageSizeY - pos.y - 1) + (int)pos.x;
        }
        public static Vector3 IndexToPos(int idx, int stageSizeX, int stageSizeY, int stageSizeZ)
        {
            Vector3 half = new Vector3(stageSizeX / 2, stageSizeY / 2, stageSizeZ / 2);
            return (new Vector3(idx % stageSizeX, stageSizeY - (idx / stageSizeY) % 5 - 1, Mathf.Floor(idx / (stageSizeX * stageSizeY))) - half);
        }

        // CSV読み込み
        public static CSVData LoadCsv(string stageNum)
        {
            StreamReader reader = new StreamReader(Application.dataPath + CSV_PATH + stageNum + ".csv");
            CSVData info = new CSVData();
            info.data = new List<string>();

            string line = reader.ReadLine();
            info.size.x = line.Split(',').Length;
            int counter = 0;
            while (reader.Peek() != -1) // reader.Peaekが-1になるまで
            {
                counter = 0;
                while (line != "!n")
                {
                    string[] data = line.Split(',');
                    info.data.AddRange(data);
                    line = reader.ReadLine();
                    counter++;
                }
                line = reader.ReadLine();
                info.size.z++;
            }

            info.size.y = counter;
            return info;
        }

        /// <summary>
        /// CSV書き込み
        /// </summary>
        /// <param name="dataList"></param>
        /// <param name="fileName"></param>
        public static void WriteCsv(List<string> dataList, string fileName, int dataSizeX, int dataSizeY)
        {
            StreamWriter sw = new StreamWriter(Application.dataPath + CSV_PATH + fileName + ".csv", false, Encoding.GetEncoding("Shift_JIS"));
            int _stageSizeX = inputFieldInt.GetInputFieldInt(inputFieldInt.FieldType.stageSizeX);
            int _stageSizeY = inputFieldInt.GetInputFieldInt(inputFieldInt.FieldType.stageSizeY);

            string strData = "";
            for (int i = 0; i < dataList.Count; ++i)
            {
                strData += string.Join(",", dataList[i]);

                if ((i + 1) % _stageSizeX == 0)
                {
                    sw.WriteLine(strData);
                    strData = "";
                }
                else
                    strData += ',';

                if ((i + 1) % (_stageSizeX * _stageSizeY) == 0)
                    sw.WriteLine("!n");
            }

            sw.Close();
        }
    }

    /// <summary>
    /// 重複なしのランダム値取得
    /// </summary>
    public static class RandomDuplication
    {
        private static Dictionary<int, List<int>> m_randomDict = new Dictionary<int, List<int>>();

        /// <summary>
        /// 重複しないランダム変数取得(最小値以上・最大値未満)
        /// </summary>
        /// <param name="instanceID">一意な値（Object.instanceID）</param>
        /// <param name="min">最小値</param>
        /// <param name="max">最大値</param>
        /// <returns></returns>
        public static int GetRandomDuplication(int instanceID, int min, int max)
        {
            // 
            if (!m_randomDict.ContainsKey(instanceID))
                m_randomDict.Add(instanceID, new List<int>());

            if (!m_randomDict.ContainsKey(instanceID) || m_randomDict[instanceID].Count <= 0)
            {
                for (int i = min; i < max; ++i)
                    m_randomDict[instanceID].Add(i);
            }

            int idx = Random.Range(0, m_randomDict[instanceID].Count);
            int num = m_randomDict[instanceID][idx];
            m_randomDict[instanceID].Remove(num);
            return num;
        }

        /// <summary>
        /// 重複情報をすべてクリア
        /// </summary>
        public static void DuplicationReset()
        {
            m_randomDict.Clear();
        }
    }
}