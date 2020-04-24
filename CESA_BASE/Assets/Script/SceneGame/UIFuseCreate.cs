using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class UIFuseCreate : MonoBehaviour
{
    [SerializeField]
    private int m_firstCreate = 0;
    [SerializeField]
    GameObject m_uiColider = null;
    [SerializeField]
    List<Fuse> m_uiList = new List<Fuse>();
    private Vector2Int m_fuseAmount = Vector2Int.zero;        // 導火線の生成数（X：左レーン、　Y：右レーン）
    private int m_createCount = AdjustParameter.UI_Fuse_Constant.CREATE_COOUNT;
    public Vector2Int FuseAmount
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
    public int FirstCreate
    {
        get
        {
            return m_firstCreate;
        }
    }

    void Awake()
    {
        if (m_firstCreate > 0)
        {
            int[] indexList = new int[m_firstCreate];
            StageCreateMgr.Instance.CreateUIFuse(m_firstCreate, transform, StageCreateMgr.SuffixType.Duplication, StageCreateMgr.SuffixType.Zero);

            m_fuseAmount = new Vector2Int((int)Mathf.Ceil((float)m_firstCreate / 2.0f), (int)Mathf.Floor((float)m_firstCreate / 2.0f));

            // 生成数が最大値と同じなら
            if (m_firstCreate == m_uiList.Count)
                m_fuseAmount = new Vector2Int(AdjustParameter.UI_Fuse_Constant.UI_FUSE_MAX, AdjustParameter.UI_Fuse_Constant.UI_FUSE_MAX);
        }
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        int fuseAmount = m_fuseAmount.x + m_fuseAmount.y;

        // 生成数が一定数より少ないなら生成処理へ
        if (fuseAmount < AdjustParameter.UI_Fuse_Constant.UI_FUSE_MAX)
        {
            m_createCount--;
            if (m_createCount <= 0)
            {

                StageCreateMgr.Instance.AddCreateUIFuse(1, transform,
                    StageCreateMgr.SuffixType.Duplication, m_fuseAmount);

                // 生成後処理
                if (m_fuseAmount.x <= m_fuseAmount.y)
                    m_fuseAmount += new Vector2Int(1, 0);
                else
                    m_fuseAmount += new Vector2Int(0, 1);

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
}
