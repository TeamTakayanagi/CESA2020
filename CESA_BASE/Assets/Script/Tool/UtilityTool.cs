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
        private const string CSV_PATH = "/ExternalFile/TextData/";
        private const string BIN_PATH = "/ExternalFile/Binary/";

        public class CSVData
        {
            public Vector3Int size;
            public List<string> data;
        }

        public class BinData
        {
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
            return (new Vector3(idx % stageSizeX, stageSizeY - (idx / stageSizeX) % stageSizeY - 1, Mathf.Floor(idx / (stageSizeX * stageSizeY))) - half);
        }

        /// <summary>
        /// CSV読み込み
        /// </summary>
        /// <param name="textName"></param>
        /// <returns></returns>
        public static CSVData LoadCsv(string textName)
        {
            StreamReader reader = new StreamReader(Application.dataPath + CSV_PATH + textName + ".csv");
            CSVData info = new CSVData();
            info.data = new List<string>();

            string line = reader.ReadLine();
            info.size.x = line.Split(',').Length;
            int counter = 0;
            while (reader.Peek() != -1) // reader.Peaekが-1になるまで
            {
                counter = 0;
                while (line != "!n" && reader.Peek() != -1)
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
        public static void WriteCsv(List<string> dataList, string fileName)
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

        /// <summary>
        /// Binaryファイル読込
        /// </summary>
        /// <param name="_fileName"></param>
        /// <returns></returns>
        public static BinData LoadBin(string _fileName, int stageNum)
        {
            BinaryReader _reader = null;
            BinData _saveData = new BinData();
            _saveData.data = new List<string>();

            try
            {
                try
                {
                    _reader = new BinaryReader(new FileStream(Application.dataPath + BIN_PATH + _fileName + ".bin", FileMode.Open));

                }
                catch
                {
                    Debug.LogWarning("LoadBinエラー");
                    return null;
                }
                string _test = _reader.ReadString();
                string[] _str = _test.Split(',');
                _saveData.data.AddRange(_str);
            }
            finally
            {
                _reader.Close();
            }

            return _saveData;
        }

        public static bool SaveBinAt(string _fileName, int _stageNum, int _clearState)
        {
            BinaryWriter _writer = null;
            try
            {
                _writer = new BinaryWriter(new FileStream(Application.dataPath + BIN_PATH + _fileName + ".bin", FileMode.Create));

                SelectMgr.SaveData.data[_stageNum - 1] = _clearState.ToString();

                string _str = SelectMgr.SaveData.data[0];
                for (int i = 1; i < SelectMgr.SaveData.data.Count; i++)
                {
                    _str += "," + SelectMgr.SaveData.data[i];
                }
                _writer.Write(_str);
            }
            catch
            {
                return false;
            }
            finally
            {
                _writer.Close();
            }

            return true;
        }

        /// <summary>
        /// セーブデータの初期化
        /// </summary>
        /// <param name="_fileName"></param>
        public static bool InitSaveData(string _fileName)
        {
            BinaryWriter _writer = null;
            try
            {
                _writer = new BinaryWriter(new FileStream(Application.dataPath + BIN_PATH + _fileName + ".bin", FileMode.Create));

                string _initData = "1";
                for (int i = 1; i < GameObject.FindGameObjectWithTag(NameDefine.TagName.StageParent).transform.childCount; i++)
                {
                    _initData += ",0";
                }
                _writer.Write(_initData);
            }
            catch
            {
                Debug.LogWarning("ファイルが見つかりません。");
                return false;
            }
            finally
            {
                _writer.Close();
            }

            return true;
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

    public static class MyMath
    {
        /// <summary>
        /// XYZの中で一番大きい値の要素を大きさ1にしてそれ以外を0の値で返す
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Vector3 GetMaxDirect(Vector3 vec)
        {
            Vector3 absolute;
            // 距離を求める
            // 絶対値にしたものを入れる
            absolute = new Vector3(Mathf.Abs(vec.x), Mathf.Abs(vec.y), Mathf.Abs(vec.z));
            // XYZの絶対値の最大値を求める
            float max = Mathf.Max(absolute.x, absolute.y, absolute.z);
            // 一番大きい要素は１、そのほか2つは0を入れる
            absolute -= new Vector3(max, max, max);
            absolute = new Vector3(Mathf.Clamp01(Mathf.Floor(absolute.x + 1)),
                Mathf.Clamp01(Mathf.Floor(absolute.y + 1)), Mathf.Clamp01(Mathf.Floor(absolute.z + 1)));
            return absolute;
        }
        public static Vector3 GetMaxDirectSign(Vector3 vec)
        {
            Vector3 absolute;
            // 距離を求める
            // 絶対値にしたものを入れる
            absolute = new Vector3(Mathf.Abs(vec.x), Mathf.Abs(vec.y), Mathf.Abs(vec.z));
            // XYZの絶対値の最大値を求める
            float max = Mathf.Max(absolute.x, absolute.y, absolute.z);
            // 一番大きい要素は１、そのほか2つは0を入れる
            absolute -= new Vector3(max, max, max);
            absolute = new Vector3(Mathf.Clamp01(Mathf.Floor(absolute.x + 1)),
                Mathf.Clamp01(Mathf.Floor(absolute.y + 1)), Mathf.Clamp01(Mathf.Floor(absolute.z + 1)));
            return absolute * Mathf.Sign(Vector3.Dot(vec, absolute));
        }
    }
}