using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMgr : MonoBehaviour
{
    [SerializeField]
    private StarMgr panelPrefab;
    [SerializeField]
    private int m_stageNum;

    private Vector3 m_initPos;
    private Vector3 m_endPos;

    private StarMgr m_panel;
    private Vector3 touchStartPos;
    private Vector3 touchEndPos;

    private Vector3 m_direction;

    private bool m_popFlg;

    public bool popFlg
    {
        get
        {
            return m_popFlg;
        }
        set
        {
            m_popFlg = value;
        }
    }

    private void Awake()
    {
        m_initPos = transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        // ステージの選択肢生成
        for (int i = 0; i < m_stageNum; i++)
        {
            m_panel = Instantiate(panelPrefab, transform);
            //m_panel.SetParam(i);
        }
        m_endPos = m_initPos;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_endPos == m_initPos)
        {
            m_endPos = m_panel.transform.position;
            m_endPos = new Vector3(m_endPos.x * -1, m_endPos.y, m_endPos.z);
        }

        if (!popFlg)
        {
            Flick();

            transform.position += m_direction;

            Debug.Log("trans:"+ transform.position);
            Debug.Log("endpos:"+m_endPos);

            if (transform.position.x > m_initPos.x)
            {
                transform.position = m_initPos;
            }
            if (transform.position.x < m_endPos.x)
            {
                transform.position = new Vector3(m_endPos.x, transform.position.y, transform.position.z);
            }
        }
        else
        {
            m_direction = Vector3.zero;
            
        }

    }

    private void FixedUpdate()
    {
        m_direction *= 0.95f;
    }

    void Flick()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            touchStartPos = new Vector3(Input.mousePosition.x/500, 0, 0);
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            touchEndPos = new Vector3(Input.mousePosition.x/500, 0, 0);

            m_direction = touchEndPos - touchStartPos;
        }
    }

    public void DestroyPopup()
    {
        m_popFlg = false;
    }
}
