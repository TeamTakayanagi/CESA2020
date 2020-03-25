using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUp : MonoBehaviour
{
    // ポップアップのサイズ
    Vector2 m_popupSize = new Vector2(0.8f*Screen.width, 0.8f*Screen.height);
    RectTransform m_rectTrans;

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
        m_rectTrans = transform.root.GetComponent<RectTransform>();
        
        // サイズいじいじ
        transform.GetComponent<RectTransform>().sizeDelta = m_rectTrans.sizeDelta * 0.5f;

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

    private void OnDestroy()
    {
        
    }

    public void Click()
    {
        m_rectTrans.GetComponentInChildren<StageMgr>().popFlg = false;
        m_Exit = false;
    }
}
