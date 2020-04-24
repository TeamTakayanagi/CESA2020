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
        Zero,
        Turn,               // 順番
        Random,             // 完全なランダム
        Duplication,        // 重複なしランダム
    }

    [SerializeField]
    GameObject m_uiColider = null;
    [SerializeField]
    List<Fuse> m_fuseList = new List<Fuse>();
    [SerializeField]
    List<GameObject> m_gimmkList = new List<GameObject>();
    [SerializeField]
    List<GameObject> m_fieldList = new List<GameObject>();

    public void CreateStage()
    {

    }

    /// <summary>
    /// UI部分の導火線の初期生成
    /// </summary>
    /// <param name="amount">生成量</param>
    /// <param name="parent">導火線の親オブジェクト</param>
    /// <param name="index">添え字の配列の決め方</param>
    public void CreateUIFuse(int amount, Transform parent, SuffixType index, SuffixType rot)
    {
        int[] indexList = GetIndexList(index, amount, m_fuseList.Count, parent.GetInstanceID());
        int UIFuseCount = 0;
        if (GameMgr.Instance)
            UIFuseCount = GameMgr.Instance.UIFuseCount;
        // 
        for (int i = UIFuseCount; i < amount; ++i)
        {
            Fuse _fuse = Instantiate(m_fuseList[indexList[i - UIFuseCount]], Vector3.zero, Quaternion.identity);
            _fuse.transform.SetParent(parent, true);
            _fuse.transform.localPosition = new Vector3((i % 2) * AdjustParameter.UI_Fuse_Constant.UI_FUSE_INTERVAL_X - 1.0f,
                1.0f + (i / 2) * AdjustParameter.UI_Fuse_Constant.UI_FUSE_INTERVAL_Y,
                AdjustParameter.UI_Fuse_Constant.UI_FUSE_POS_Z);
            _fuse.EndPos = _fuse.transform.localPosition;
            if(rot != SuffixType.Zero)
                _fuse.transform.localEulerAngles = new Vector3(90.0f * Random.Range(0, 4), 90.0f * Random.Range(0, 4), 90.0f * Random.Range(0, 4));
            _fuse.Type = Fuse.FuseType.UI;

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
        int[] indexList = GetIndexList(index, amount, m_fuseList.Count, parent.GetInstanceID());
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
            _fuse.transform.SetParent(transform, true);

            _fuse.EndPos = new Vector3(place,
                1.0f + ((fuseAmount - (Mathf.Abs(fuseRean.x - fuseRean.y) / 2)) / 2) * AdjustParameter.UI_Fuse_Constant.UI_FUSE_INTERVAL_Y,
                AdjustParameter.UI_Fuse_Constant.UI_FUSE_POS_Z);
            _fuse.transform.localPosition = new Vector3(place, AdjustParameter.UI_Fuse_Constant.UI_FUSE_POS_Y,
                AdjustParameter.UI_Fuse_Constant.UI_FUSE_POS_Z);
            _fuse.transform.localEulerAngles = new Vector3(90.0f * Random.Range(0, 4), 90.0f * Random.Range(0, 4), 90.0f * Random.Range(0, 4));
            _fuse.Type = Fuse.FuseType.UI;


            GameMgr.Instance.UIFuse = _fuse;    // リストの末尾に追加
            // UI専用のコライダーを子供に
            GameObject _colider = Instantiate(m_uiColider, _fuse.transform.position, Quaternion.identity);
            _colider.transform.SetParent(_fuse.transform, true);
        }
    }

    /// <summary>
    /// 配列の添え字のリスト生成
    /// </summary>
    /// <param name="type">添え字の配列の決め方</param>
    /// <param name="elementCount">戻り値の要素数</param>
    /// <param name="objListCount">戻り値の配列を使うオブジェクトの要素数</param>
    /// <param name="instanceID">一意な値(Object.instanceID)</param>
    /// <returns>オブジェクトの添え字の配列</returns>
    private int[] GetIndexList(SuffixType type, int elementCount, int objListCount, int instanceID)
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
