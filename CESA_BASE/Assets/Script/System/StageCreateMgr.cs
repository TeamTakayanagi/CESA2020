using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private List<Fuse> m_fuseList = new List<Fuse>();
    [SerializeField]
    private List<GameGimmick> m_gimmkList = new List<GameGimmick>();
    //[SerializeField]
    //private List<GameObject> m_fieldList = new List<GameObject>();

    private void Start()
    {
        m_gimmkList.Sort((a, b) => b.Type - a.Type);
        m_fuseList.Sort((a, b) => string.Compare(a.name, b.name));
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
            Fuse _fuse = Instantiate(m_fuseList[indexList[i - UIFuseCount]], Vector3.zero, Quaternion.identity);
            _fuse.Type = Fuse.FuseType.Normal;
            _fuse.State = Fuse.FuseState.UI;
            _fuse.transform.SetParent(parent, true);
            _fuse.transform.localPosition = new Vector3((i % 2) * AdjustParameter.UI_OBJECT_Constant.INTERVAL_X - 1.0f,
                1.0f + (i / 2) * AdjustParameter.UI_OBJECT_Constant.INTERVAL_Y,
                AdjustParameter.UI_OBJECT_Constant.DEFAULT_POS_Z);
            _fuse.EndPos = _fuse.transform.localPosition;
            if(rot != SuffixType.Zero)
                _fuse.transform.localEulerAngles = new Vector3(90.0f * Random.Range(0, 4), 90.0f * Random.Range(0, 4), 90.0f * Random.Range(0, 4));

            // UI専用のコライダーを子供に
            GameObject _colider = Instantiate(m_uiColider, _fuse.transform.position, Quaternion.identity);
            _colider.transform.SetParent(_fuse.transform, true);
        }
    }

    /// <summary>
    /// UI部分の導火線追加生成
    /// </summary>
    /// <param name="amout">生成量</param>
    /// <param name="parent">導火線の親オブジェクト</param>
    /// <param name="indexList">添え字の配列</param>
    public void AddCreateUIFuse(int amount, Transform parent, SuffixType index, Vector2Int fuseRean)
    {
        int[] indexList = GetSuffixList(index, amount, m_fuseList.Count, parent.GetInstanceID());
        int fuseAmount = fuseRean.x + fuseRean.y;
        for (int i = 0; i < amount; ++i)
        {
            // 出す場所
            int place;
            if (fuseRean.x <= fuseRean.y)
                place = -1;
            else
                place = 1;

            Fuse _fuse = Instantiate(m_fuseList[indexList[i]], transform.position, Quaternion.identity);
            _fuse.Type = Fuse.FuseType.Normal;
            _fuse.State = Fuse.FuseState.UI;
            _fuse.transform.SetParent(parent, true);
            _fuse.EndPos = new Vector3(place,
                1.0f + ((fuseAmount - (Mathf.Abs(fuseRean.x - fuseRean.y) / 2)) / 2) * AdjustParameter.UI_OBJECT_Constant.INTERVAL_Y,
                AdjustParameter.UI_OBJECT_Constant.DEFAULT_POS_Z);
            _fuse.transform.localPosition = new Vector3(place, AdjustParameter.UI_OBJECT_Constant.DEFAULT_POS_Y,
                AdjustParameter.UI_OBJECT_Constant.DEFAULT_POS_Z);
            _fuse.transform.localEulerAngles = new Vector3(90.0f * Random.Range(0, 4), 90.0f * Random.Range(0, 4), 90.0f * Random.Range(0, 4));

            if(GameMgr.Instance)
                GameMgr.Instance.UIFuse = _fuse;    // リストの末尾に追加
            // UI専用のコライダーを子供に
            GameObject _colider = Instantiate(m_uiColider, _fuse.transform.position, Quaternion.identity);
            _colider.transform.SetParent(_fuse.transform, true);
        }
    }

    /// <summary>
    /// ギミックのUI表示
    /// </summary>
    /// <param name="parent">UIオブジェクトの親</param>
    public void CreateUIGimmck(Transform parent)
    {
        int[] indexList = GetSuffixList(SuffixType.Turn, m_gimmkList.Count, m_gimmkList.Count, parent.GetInstanceID());
        // 
        for (int i = 0; i < m_gimmkList.Count; ++i)
        {
            GameGimmick _gimmick = Instantiate(m_gimmkList[indexList[i]], Vector3.zero, Quaternion.identity);
            _gimmick.transform.SetParent(parent, true);
            _gimmick.UI = true;
            _gimmick.transform.localPosition = new Vector3((i % 2) * AdjustParameter.UI_OBJECT_Constant.INTERVAL_X - 1.0f,
                1.0f + (i / 2) * AdjustParameter.UI_OBJECT_Constant.INTERVAL_Y, AdjustParameter.UI_OBJECT_Constant.DEFAULT_POS_Z);
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
                    indexList[i] = Random.Range(0, objListCount);
                break;
            case SuffixType.Duplication:
                for (int i = 0; i < elementCount; ++i)
                    indexList[i] = Utility.RandomDuplication.GetRandomDuplication(instanceID, 0, objListCount);
                break;
        }
        return indexList;
    }
}
