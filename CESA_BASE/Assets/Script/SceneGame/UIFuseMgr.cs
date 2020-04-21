using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFuseMgr : SingletonMonoBehaviour<UIFuseMgr>
{
    [System.Serializable]
    private class FuseStatus
    {
        public Fuse prefab = null;
        public Vector3 rotate = Vector3.zero;
    }

    [SerializeField]
    GameObject m_uiColider = null;
    [SerializeField]
    List<FuseStatus> m_uiList = new List<FuseStatus>();
    private Vector2 m_fuseAmount = Vector2.zero;        // 導火線の生成数（X：左レーン、　Y：右レーン）
    private int m_createCount = AdjustParameter.UI_Fuse_Constant.CREATE_COOUNT;

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
                int idx = Random.Range(0, m_uiList.Count);  // 出すモデルをプレハブからランダム選択

                // 出す場所を
                int place;
                if (m_fuseAmount.x <= m_fuseAmount.y)
                    place = -1;
                else
                    place = 1;

                Fuse _fuse = Instantiate(m_uiList[idx].prefab, transform.position, Quaternion.identity);
                _fuse.transform.SetParent(transform, true);

                _fuse.EndPos = new Vector3(place,
                    1.0f + ((fuseAmount - ((int)Mathf.Abs(m_fuseAmount.x - m_fuseAmount.y) / 2)) / 2) * AdjustParameter.UI_Fuse_Constant.UI_FUSE_INTERVAL_Y,
                    AdjustParameter.UI_Fuse_Constant.UI_FUSE_POS_Z);
                _fuse.transform.localPosition = new Vector3(place, AdjustParameter.UI_Fuse_Constant.UI_FUSE_POS_Y, AdjustParameter.UI_Fuse_Constant.UI_FUSE_POS_Z);
                _fuse.transform.localEulerAngles = m_uiList[idx].rotate;
                _fuse.Type = Fuse.FuseType.UI;
                _fuse.transform.tag = StringDefine.TagName.Fuse;


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
}
