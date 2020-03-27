using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageData : MonoBehaviour
{
    [SerializeField]
    private PopUp popupPrefab;

    [SerializeField]
    private Sprite m_notopenSprite;
    [SerializeField]
    private Sprite m_openSprite;
    [SerializeField]
    private Sprite m_silverSprite;
    [SerializeField]
    private Sprite m_goldSprite;

    private PopUp m_popup;
    private MedalMgr m_starMgr = null;
    private Image m_medalSprite = null;
    private int m_stageState = 0;

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
        m_medalSprite = GetComponent<Image>();
        m_medalSprite.sprite = null;
    }

    // Update is called once per frame
    void Update()
    {
        // メダルのテクスチャ変更
        if (m_medalSprite.sprite == null)
        {
            switch (m_stageState)
            {
                case 0:
                    m_medalSprite.sprite = m_notopenSprite;
                    break;

                case 1:
                    m_medalSprite.sprite = m_openSprite;
                    break;

                case 2:
                    m_medalSprite.sprite = m_openSprite;
                    m_starMgr.MedalCreate(m_silverSprite, 0.07f);
                    break;

                case 3:
                    m_medalSprite.sprite = m_openSprite;
                    m_starMgr.MedalCreate(m_goldSprite, 0.07f);
                    break;

                default:
                    m_medalSprite.sprite = m_openSprite;
                    break;
            }
        }
    }

    public void Click()
    {
        if (!transform.GetComponentInParent<StageMgr>().popFlg)
        {
            m_popup = Instantiate(popupPrefab, transform.root.position, Quaternion.identity, transform.root);

            switch (m_stageState)
            {
                case 2:
                    m_popup.MedalInstance(m_silverSprite);
                    break;

                case 3:
                    m_popup.MedalInstance(m_goldSprite);
                    break;

                default:
                    break;
            }
            
            transform.GetComponentInParent<StageMgr>().popFlg = true;
        }
    }

    public void SetSprite(int _spriteNum)
    {
        m_stageState = _spriteNum;
    }
}
