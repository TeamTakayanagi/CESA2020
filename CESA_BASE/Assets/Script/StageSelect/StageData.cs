using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageData : MonoBehaviour
{
    [SerializeField]
    private PopUp m_popupPrefab = null;

    private PopUp m_popup = null;
    private MedalMgr m_starMgr = null;
    //private Image m_medalSprite = null;
    private int m_stageState = 0;   // ステージの状態：選択可能 = 1以上 / 不可能 = 0

    private int m_stageNum = 0;

    private CSVScript m_csvScript = null;

    //private CSVStageData m_stageData = null;
    //private int m_step = 0;
    public bool PopUp
    {
        get
        {
            return m_popup;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_starMgr = GetComponent<MedalMgr>();

        m_csvScript = GameObject.FindGameObjectWithTag(ConstDefine.TagName.SceneMgr).GetComponent<CSVScript>();
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    public void Click()
    {
        //if (m_stageState <= 0) return;

        //m_csvScript.StageNum = m_stageNum;
        //if (!transform.GetComponentInParent<StageMgr>().popFlg)
        //{
        //    m_popup = Instantiate(popupPrefab, transform.root.position, Quaternion.identity, transform.root);

        //    switch (m_stageState)
        //    {
        //        case 2:
        //            m_popup.MedalInstance(m_silverSprite);
        //            break;

        //        case 3:
        //            m_popup.MedalInstance(m_goldSprite);
        //            break;

        //        default:
        //            break;
        //    }

        //    transform.GetComponentInParent<StageMgr>().popFlg = true;
        //}
    }

    /// ///////////////////////
    // 引　数：スプライト番号, ステージ数
    public void SetParam(int _spriteNum, int _stageNum)
    {
        m_stageState = _spriteNum;
        m_stageNum = _stageNum;
        gameObject.GetComponent<Renderer>().material.SetInt("TexNum", m_stageState);
    }
}
