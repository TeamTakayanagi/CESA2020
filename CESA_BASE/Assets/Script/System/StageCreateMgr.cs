using System.Collections.Generic;
using UnityEngine;
using System;

public class StageCreateMgr : SingletonMonoBehaviour<StageCreateMgr>
{
    /// <summary>
    /// 添え字の配列の決め方
    /// </summary>
    public enum SuffixType
    {
        Zero,               // すべてゼロ
        Turn,               // 順番
        Random,             // 完全なランダム
        Duplication,        // 重複なしランダム
    }

    [SerializeField]
    private GameObject m_uiColider = null;
    [SerializeField]
    private GameObject m_rotMark = null;
    [SerializeField]
    private GameObject m_moveMark = null;
    [SerializeField]
    private List<Fuse> m_fuseList = new List<Fuse>();
    [SerializeField]
    private List<GameGimmick> m_gimmkList = new List<GameGimmick>();

    private void Start()
    {
        // 昇順に並び替え
        m_gimmkList.Sort((a, b) => a.Type - b.Type);
        var comp = new Comparison<Fuse>(CompColider);
        m_fuseList.Sort(comp);
    }

    /// <summary>
    /// 比較用
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private int CompColider(Fuse a, Fuse b)
    {
        BoxCollider[] boxA, boxB;
        boxA = a.GetComponents<BoxCollider>();
        boxB = b.GetComponents<BoxCollider>();

        if (boxA.Length != boxB.Length)
            return boxB.Length - boxA.Length;

        float sumA = 0, sumB = 0, difference;
        for(int i = 0; i < boxA.Length; ++i)
        {
            sumA += Vector3.Dot(boxA[i].size, Vector3.one);
            sumB += Vector3.Dot(boxB[i].size, Vector3.one);
        }

        difference = sumB - sumA;
        return (int)(difference / Mathf.Abs(sumB - sumA));
    }

    /// <summary>
    /// ステージのオブジェト生成
    /// </summary>
    /// <param name="parent">ステージの親オブジェクト</param>
    /// <param name="csvData">CSVのデータ</param>
    public List<GameObject> CreateStage(Transform parent, Utility.CSVFile.CSVData csvData)
    {
        List<GameObject> _createList = new List<GameObject>();

        for(int i = 0; i < csvData.data.Count; ++i)
        {
            if (csvData.data[i].Length != ProcessedtParameter.CSV_Constant.STAGE_DATA_COUNT)
            {
                string tagName = csvData.data[i].Substring(ProcessedtParameter.CSV_Constant.ADDINFO_WORD_COUNT, ProcessedtParameter.CSV_Constant.OBJECT_WORD_COUNT);

                // 配置されていないなら
                if (tagName == "--")
                    continue;

                Fuse _fuse = null;
                for (int j = 0; j < m_fuseList.Count; ++j)
                {
                    // タグの一部が一致しているなら
                    if (Utility.TagSeparate.getChildTagName(m_fuseList[j].tag) != tagName)
                        continue;

                    Vector3 pos = Utility.CSVFile.IndexToPos(i, csvData.size.x, csvData.size.y, csvData.size.z);
                    _fuse = Instantiate(m_fuseList[j], pos, Quaternion.identity);
                    _fuse.transform.parent = parent;
                    _fuse.Type = (Fuse.FuseType)int.Parse(csvData.data[i].Substring(0, ProcessedtParameter.CSV_Constant.ADDINFO_WORD_COUNT));
                    _fuse.transform.localEulerAngles = new Vector3(
                        float.Parse(csvData.data[i].Substring(ProcessedtParameter.CSV_Constant.OBJECT_ROT_COUNT, 1)),
                        float.Parse(csvData.data[i].Substring(ProcessedtParameter.CSV_Constant.OBJECT_ROT_COUNT + 1, 1)),
                        float.Parse(csvData.data[i].Substring(ProcessedtParameter.CSV_Constant.OBJECT_ROT_COUNT + 2, 1))) * 90.0f;

                    _createList.Add(_fuse.gameObject);
                    break;
                }

                // 導火線を生成していないなら
                if (!_fuse)
                {
                    GameGimmick _gimmick = null;
                    for (int j = 0; j < m_gimmkList.Count; ++j)
                    {
                        string st = Utility.TagSeparate.getChildTagName(m_gimmkList[j].tag).Substring(0, 2);
                        // タグの一部が一致しているなら
                        if (Utility.TagSeparate.getChildTagName(m_gimmkList[j].tag).Substring(0,
                            ProcessedtParameter.CSV_Constant.OBJECT_WORD_COUNT)
                            != tagName)
                            continue;

                        Vector3 pos = Utility.CSVFile.IndexToPos(i, csvData.size.x, csvData.size.y, csvData.size.z);
                        _gimmick = Instantiate(m_gimmkList[j], pos, Quaternion.identity);
                        _gimmick.transform.parent = parent;
                        _gimmick.Type = (GameGimmick.GimmickType)j;
                        _gimmick.transform.localEulerAngles = new Vector3(
                                float.Parse(csvData.data[i].Substring(ProcessedtParameter.CSV_Constant.OBJECT_ROT_COUNT, 1)),
                                float.Parse(csvData.data[i].Substring(ProcessedtParameter.CSV_Constant.OBJECT_ROT_COUNT + 1, 1)),
                                float.Parse(csvData.data[i].Substring(ProcessedtParameter.CSV_Constant.OBJECT_ROT_COUNT + 2, 1))) * 90.0f;

                        _createList.Add(_gimmick.gameObject);
                        break;
                    }
                }
            }
            else
            {
                string objName = csvData.data[i].Substring(0, ProcessedtParameter.CSV_Constant.TYPE_WORD_COUNT);
                string tagName = csvData.data[i].Substring(ProcessedtParameter.CSV_Constant.TYPE_WORD_COUNT + ProcessedtParameter.CSV_Constant.ADDINFO_WORD_COUNT, ProcessedtParameter.CSV_Constant.OBJECT_WORD_COUNT);

                // 配置されていないなら
                if (objName == "f")
                {
                    Fuse _fuse = null;
                    for (int j = 0; j < m_fuseList.Count; ++j)
                    {
                        // タグの一部が一致しているなら
                        if (Utility.TagSeparate.getChildTagName(m_fuseList[j].tag) != tagName)
                            continue;

                        Vector3 pos = Utility.CSVFile.IndexToPos(i, csvData.size.x, csvData.size.y, csvData.size.z);
                        _fuse = Instantiate(m_fuseList[j], parent);
                        _fuse.transform.position = pos;
                        _fuse.Type = (Fuse.FuseType)int.Parse(csvData.data[i].Substring(ProcessedtParameter.CSV_Constant.TYPE_WORD_COUNT, ProcessedtParameter.CSV_Constant.ADDINFO_WORD_COUNT));
                        _fuse.transform.localEulerAngles = new Vector3(
                            float.Parse(csvData.data[i].Substring(ProcessedtParameter.CSV_Constant.TYPE_WORD_COUNT + ProcessedtParameter.CSV_Constant.ADDINFO_WORD_COUNT + ProcessedtParameter.CSV_Constant.OBJECT_WORD_COUNT, 1)),
                            float.Parse(csvData.data[i].Substring(ProcessedtParameter.CSV_Constant.TYPE_WORD_COUNT + ProcessedtParameter.CSV_Constant.ADDINFO_WORD_COUNT + ProcessedtParameter.CSV_Constant.OBJECT_WORD_COUNT + 1, 1)),
                            float.Parse(csvData.data[i].Substring(ProcessedtParameter.CSV_Constant.TYPE_WORD_COUNT + ProcessedtParameter.CSV_Constant.ADDINFO_WORD_COUNT + ProcessedtParameter.CSV_Constant.OBJECT_WORD_COUNT + 2, 1))) * 90.0f;
                        

                        if(_fuse.Type >= Fuse.FuseType.MoveLeft && _fuse.Type <= Fuse.FuseType.MoveForward)
                        {
                            Quaternion rot = Quaternion.identity;
                            switch (_fuse.Type)
                            {
                                case Fuse.FuseType.MoveRight:
                                    rot = Quaternion.Euler(0.0f, 180.0f, 0.0f);
                                    break;
                                case Fuse.FuseType.MoveUp:
                                    rot = Quaternion.Euler(0.0f, 0.0f, 90.0f);
                                    break;
                                case Fuse.FuseType.MoveDown:
                                    rot = Quaternion.Euler(180.0f, 0.0f, 90.0f);
                                    break;
                                case Fuse.FuseType.MoveForward:
                                    rot = Quaternion.Euler(0.0f, 90.0f, 0.0f);

                                    break;
                                case Fuse.FuseType.MoveBack:
                                    rot = Quaternion.Euler(0.0f, 270.0f, 0.0f);
                                    break;
                                default:
                                    break;
                            }
                            GameObject _colider = Instantiate(m_moveMark, _fuse.transform);
                            _colider.transform.position = _fuse.transform.position;
                            _colider.transform.rotation = rot;
                        }
                        else if(_fuse.Type == Fuse.FuseType.Rotate)
                        {
                            GameObject _colider = Instantiate(m_rotMark, _fuse.transform);
                            _colider.transform.position = _fuse.transform.position;
                            _colider.transform.rotation = Quaternion.identity;
                        }

                        _createList.Add(_fuse.gameObject);
                        break;
                    }
                }
                else if (objName == "g")
                {
                    GameGimmick _gimmick = null;
                    for (int j = 0; j < m_gimmkList.Count; ++j)
                    {
                        // タグの一部が一致しているなら
                        if (Utility.TagSeparate.getChildTagName(m_gimmkList[j].tag).Substring(0, ProcessedtParameter.CSV_Constant.OBJECT_WORD_COUNT)
                            != tagName)
                            continue;

                        Vector3 pos = Utility.CSVFile.IndexToPos(i, csvData.size.x, csvData.size.y, csvData.size.z);
                        _gimmick = Instantiate(m_gimmkList[j], parent);
                        _gimmick.transform.position = pos;
                        _gimmick.Type = (GameGimmick.GimmickType)j;
                        _gimmick.Value = float.Parse(csvData.data[i].Substring(ProcessedtParameter.CSV_Constant.TYPE_WORD_COUNT, ProcessedtParameter.CSV_Constant.ADDINFO_WORD_COUNT));
                        _gimmick.transform.localEulerAngles = new Vector3(
                            float.Parse(csvData.data[i].Substring(ProcessedtParameter.CSV_Constant.TYPE_WORD_COUNT + ProcessedtParameter.CSV_Constant.ADDINFO_WORD_COUNT + ProcessedtParameter.CSV_Constant.OBJECT_WORD_COUNT, 1)),
                            float.Parse(csvData.data[i].Substring(ProcessedtParameter.CSV_Constant.TYPE_WORD_COUNT + ProcessedtParameter.CSV_Constant.ADDINFO_WORD_COUNT + ProcessedtParameter.CSV_Constant.OBJECT_WORD_COUNT + 1, 1)),
                            float.Parse(csvData.data[i].Substring(ProcessedtParameter.CSV_Constant.TYPE_WORD_COUNT + ProcessedtParameter.CSV_Constant.ADDINFO_WORD_COUNT + ProcessedtParameter.CSV_Constant.OBJECT_WORD_COUNT + 2, 1))) * 90.0f;

                        _createList.Add(_gimmick.gameObject);
                        break;
                    }
                }
            }
        }

        return _createList;
    }

    /// <summary>
    /// UI部分の導火線の初期生成
    /// </summary>
    /// <param name="amount">生成量</param>
    /// <param name="parent">導火線の親オブジェクト</param>
    /// <param name="index">添え字の配列の決め方</param>
    public void CreateUIFuse(int amount, Transform parent, SuffixType index, SuffixType rot)
    {
        int[] indexList = GetSuffixList(index, amount, m_fuseList.Count, parent.GetInstanceID());
        int UIFuseCount = 0;
        if (GameMgr.Instance)
            UIFuseCount = GameMgr.Instance.UIFuseCount;
        // 
        for (int i = UIFuseCount; i < amount; ++i)
        {
            Fuse _fuse = Instantiate(m_fuseList[indexList[i - UIFuseCount]], parent);
            _fuse.Type = Fuse.FuseType.Normal;
            _fuse.State = Fuse.FuseState.UI;
            _fuse.transform.localPosition = new Vector3((i % 2) * AdjustParameter.UI_Object_Constant.INTERVAL_X - 1.0f,
                1.0f + (i / 2) * AdjustParameter.UI_Object_Constant.INTERVAL_Y,
                AdjustParameter.UI_Object_Constant.DEFAULT_POS_Z);
            _fuse.EndPos = Vector3.zero;
            _fuse.DefaultPos = _fuse.transform.parent.position +_fuse.transform.localPosition;

            if (rot != SuffixType.Zero)
                _fuse.transform.localEulerAngles = new Vector3(90.0f * UnityEngine.Random.Range(0, 4), 90.0f * UnityEngine.Random.Range(0, 4), 90.0f * UnityEngine.Random.Range(0, 4));

            // UI専用のコライダーを子供に
            Instantiate(m_uiColider, _fuse.transform);
        }
    }

    /// <summary>
    /// UI部分の導火線追加生成（1個）
    /// </summary>
    /// <param name="parent">導火線の親オブジェクト</param>
    /// <param name="index">添え字の配列の決め方</param>
    /// <param name="fuseRean">UIの導火線の総数（X：右　Y：左）</param>
    /// <param name="indexNum">添え字指定（デフォルト引数：-1）</param>
    public void AddCreateUIFuse(Transform parent, SuffixType index, Vector2Int fuseRean, int indexNum = -1)
    {
        int[] indexList;
        if(indexNum < 0 || indexNum >= m_fuseList.Count)
            indexList = GetSuffixList(index, 1, m_fuseList.Count, parent.GetInstanceID());
        else
            indexList = new int[1] { indexNum };

        int fuseAmount = fuseRean.x + fuseRean.y;
        // 出す場所
        int place;
        if (fuseRean.x <= fuseRean.y)
            place = -1;
        else
            place = 1;

        Fuse _fuse = Instantiate(m_fuseList[indexList[0]], parent);
        _fuse.Type = Fuse.FuseType.Normal;
        _fuse.State = Fuse.FuseState.UI;
        _fuse.EndPos = new Vector3(place,
            1.0f + ((fuseAmount - (Mathf.Abs(fuseRean.x - fuseRean.y) / 2)) / 2) * AdjustParameter.UI_Object_Constant.INTERVAL_Y,
            AdjustParameter.UI_Object_Constant.DEFAULT_POS_Z);
        _fuse.transform.localPosition = new Vector3(place, AdjustParameter.UI_Object_Constant.DEFAULT_POS_Y,
            AdjustParameter.UI_Object_Constant.DEFAULT_POS_Z);
        _fuse.transform.localEulerAngles = new Vector3(90.0f * UnityEngine.Random.Range(0, 4), 90.0f * UnityEngine.Random.Range(0, 4), 90.0f * UnityEngine.Random.Range(0, 4));

        if (GameMgr.Instance)
            GameMgr.Instance.UIFuse = _fuse;    // リストの末尾に追加

        // UI専用のコライダーを子供に
        Instantiate(m_uiColider, _fuse.transform);
    }

    /// <summary>
    /// ギミックのUI表示（エディタ用）
    /// </summary>
    /// <param name="parent">UIオブジェクトの親</param>
    public void CreateUIGimmck(Transform parent)
    {
        int[] indexList = GetSuffixList(SuffixType.Turn, m_gimmkList.Count, m_gimmkList.Count, parent.GetInstanceID());
        // 
        for (int i = 0; i < m_gimmkList.Count; ++i)
        {
            GameGimmick _gimmick = Instantiate(m_gimmkList[indexList[i]], parent);
            _gimmick.UI = true;
            _gimmick.transform.localPosition = new Vector3((i % 2) * AdjustParameter.UI_Object_Constant.INTERVAL_X - 1.0f,
                1.0f + (i / 2) * AdjustParameter.UI_Object_Constant.INTERVAL_Y, AdjustParameter.UI_Object_Constant.DEFAULT_POS_Z);
        }
    }

    /// <summary>
    /// 添え字の配列生成
    /// </summary>
    /// <param name="type">添え字の配列の決め方</param>
    /// <param name="elementCount">戻り値の要素数</param>
    /// <param name="objListCount">戻り値の配列を使うオブジェクトの要素数</param>
    /// <param name="instanceID">一意な値(Object.instanceID)</param>
    /// <returns>オブジェクトの添え字の配列</returns>
    private int[] GetSuffixList(SuffixType type, int elementCount, int objListCount, int instanceID)
    {
        int[] indexList = new int[elementCount];
        switch(type)
        {
            case SuffixType.Zero:
                for (int i = 0; i < elementCount; ++i)
                    indexList[i] = 0;
                break;
            case SuffixType.Turn:
                for (int i = 0; i < elementCount; ++i)
                    indexList[i] = i;
                break;
            case SuffixType.Random:
                for (int i = 0; i < elementCount; ++i)
                    indexList[i] = UnityEngine.Random.Range(0, objListCount);
                break;
            case SuffixType.Duplication:
                for (int i = 0; i < elementCount; ++i)
                    indexList[i] = Utility.RandomDuplication.GetRandomDuplication(instanceID, 0, objListCount);
                break;
        }
        return indexList;
    }
}
