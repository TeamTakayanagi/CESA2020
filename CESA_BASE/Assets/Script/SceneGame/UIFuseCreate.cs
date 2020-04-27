using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIFuseCreate : MonoBehaviour
{
    [SerializeField]
    private int m_firstCreate = 0;
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

    void Awake()
    {
        if (m_firstCreate > 0)
        {
            m_fuseAmount = new Vector2Int((int)Mathf.Ceil(m_firstCreate / 2.0f), (int)Mathf.Floor(m_firstCreate / 2.0f));
            StageCreateMgr.Instance.CreateUIFuse(m_firstCreate, transform, StageCreateMgr.SuffixType.Duplication, StageCreateMgr.SuffixType.Zero);
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
}
