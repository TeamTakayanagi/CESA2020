using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUp : MonoBehaviour
{
    private MedalMgr m_medalMgr = null;
    private Sprite m_medalSprite = null;

    private bool m_updateFlg = false;

    // ポップアップのサイズ
    private RectTransform m_rectTrans;
    public RectTransform RectTrans
    {
        get
        {
            return m_rectTrans;
        }
    }

    // 生存フラグ
    private bool m_Exit;
    public bool Exit
    {
        set
        {
            m_Exit = value;
        }
    }

    private void Awake()
    {
        m_Exit = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_medalMgr = GetComponent<MedalMgr>();

        m_rectTrans = transform.root.GetComponent<RectTransform>();
        m_rectTrans.sizeDelta *= 0.5f;

        // サイズいじいじ
        transform.GetComponent<RectTransform>().sizeDelta = m_rectTrans.sizeDelta;

    }

    // Update is called once per frame
    void Update()
    {
        if (m_Exit)
        {
            return;
        }
        else
        {
            
            Destroy(gameObject);
        }
    }

    private void LateUpdate()
    {
        if (m_updateFlg)
        {
            m_medalMgr.MedalCreate(m_medalSprite, 0.03f, 5);
            m_updateFlg = false;
        }
    }

    public void MedalInstance(Sprite _sprite)
    {
        m_medalSprite = _sprite;
        
        m_updateFlg = true;
    }

    public void Click()
    {
        //m_rectTrans.GetComponentInChildren<StageMgr>().popFlg = false;
        m_Exit = false;
    }
}
