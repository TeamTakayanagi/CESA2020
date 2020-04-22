using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class UIFuseMgr : SingletonMonoBehaviour<UIFuseMgr>
{
    [SerializeField]
    private int m_firstCreate = 0;
    [SerializeField]
    GameObject m_uiColider = null;
    [SerializeField]
    List<Fuse> m_uiList = new List<Fuse>();
    private Vector2 m_fuseAmount = Vector2.zero;        // 導火線の生成数（X：左レーン、　Y：右レーン）
    private int m_createCount = AdjustParameter.UI_Fuse_Constant.CREATE_COOUNT;
    private List<int> m_randomList = new List<int>();
    public Vector2 FuseAmount
    {
        get
        {
            return m_fuseAmount;
        }
        set
        {
            m_fuseAmount = value;
        }
    }

    private void Awake()
    {
        if (m_firstCreate > 0)
        {
            for (int i = 0; i < m_firstCreate; ++i)
            {
                Fuse _fuse = Instantiate(m_uiList[GetDuplicationRandom(0, m_uiList.Count)], Vector3.zero, Quaternion.identity);
                _fuse.transform.SetParent(transform, true);
                int y = ((i / 2) / 2);
                _fuse.transform.localPosition = new Vector3((i % 2) * AdjustParameter.UI_Fuse_Constant.UI_FUSE_INTERVAL_X - 1.0f,
                    1.0f + (i / 2) * AdjustParameter.UI_Fuse_Constant.UI_FUSE_INTERVAL_Y,
                    AdjustParameter.UI_Fuse_Constant.UI_FUSE_POS_Z);
                _fuse.Type = Fuse.FuseType.UI;

                // UI専用のコライダーを子供に
                GameObject _colider = Instantiate(m_uiColider, _fuse.transform.position, Quaternion.identity);
                _colider.transform.SetParent(_fuse.transform, true);
            }

            // 生成数が最大値と同じなら
            if(m_firstCreate == m_uiList.Count)
                m_fuseAmount = new Vector2(AdjustParameter.UI_Fuse_Constant.UI_FUSE_MAX, AdjustParameter.UI_Fuse_Constant.UI_FUSE_MAX);
        }     
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        int fuseAmount = (int)(m_fuseAmount.x + m_fuseAmount.y);

        // 生成数が一定数より少ないなら生成処理へ
        if (fuseAmount < AdjustParameter.UI_Fuse_Constant.UI_FUSE_MAX)
        {
            m_createCount--;
            if (m_createCount <= 0)
            {
                int idx = GetDuplicationRandom(0, m_uiList.Count);  // 出すモデルをプレハブからランダム選択

                // 出す場所を
                int place;
                if (m_fuseAmount.x <= m_fuseAmount.y)
                    place = -1;
                else
                    place = 1;

                Fuse _fuse = Instantiate(m_uiList[idx], transform.position, Quaternion.identity);
                _fuse.transform.SetParent(transform, true);

                _fuse.EndPos = new Vector3(place,
                    1.0f + ((fuseAmount - ((int)Mathf.Abs(m_fuseAmount.x - m_fuseAmount.y) / 2)) / 2) * AdjustParameter.UI_Fuse_Constant.UI_FUSE_INTERVAL_Y,
                    AdjustParameter.UI_Fuse_Constant.UI_FUSE_POS_Z);
                _fuse.transform.localPosition = new Vector3(place, AdjustParameter.UI_Fuse_Constant.UI_FUSE_POS_Y, AdjustParameter.UI_Fuse_Constant.UI_FUSE_POS_Z);
                _fuse.transform.localEulerAngles = new Vector3(90.0f * Random.Range(0, 4), 90.0f * Random.Range(0, 4), 90.0f * Random.Range(0, 4));
                _fuse.Type = Fuse.FuseType.UI;


                GameMgr.Instance.UIFuse = _fuse;    // リストの末尾に追加

                // UI専用のコライダーを子供に
                GameObject _colider = Instantiate(m_uiColider, _fuse.transform.position, Quaternion.identity);
                _colider.transform.SetParent(_fuse.transform, true);

                // 生成後処理
                m_fuseAmount += new Vector2((-place + 1) / 2, (place + 1) / 2);
                m_createCount = AdjustParameter.UI_Fuse_Constant.CREATE_COOUNT;
            }
        }
    }

    /// <summary>
    /// 最小値以上最大値未満の重複なしランダム数を取得
    /// </summary>
    /// <param name="min">最小値</param>
    /// <param name="max">最大値</param>
    /// <returns></returns>
    int GetDuplicationRandom(int min, int max)
    {
        if (m_randomList.Count <= 0)
        {
            for (int i = min; i < max; ++i)
                m_randomList.Add(i);
        }

        int idx = Random.Range(0, m_randomList.Count);
        int num = m_randomList[idx];
        m_randomList.Remove(num);
        return num;
    }
}
