using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMgr : MonoBehaviour
{
    [SerializeField]
    private StageData panelPrefab;
    [SerializeField]
    private int m_stageNum;

    private Vector3 m_initPos;
    private Vector3 m_endPos;

    private StageData m_panel;
    private Vector3 touchStartPos;
    private Vector3 touchEndPos;

    private Vector3 m_direction;

    private CSVStageData m_stageData = null;

    private int m_step = 0;
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
        m_stageData = GetComponent<CSVStageData>();

        m_endPos = m_initPos;
        
    }

    // Update is called once per frame
    void Update()
    {
        // CSV読み込み
        if (m_step == 0)
        {
            if (m_stageData.LoadSaveData())
            {
                m_step++;
            }
        }

        if (m_step == 1)
        {
            // ステージの選択肢生成
            for (int i = 0; i < m_stageNum; i++)
            {
                m_panel = Instantiate(panelPrefab, transform);
                m_panel.SetSprite(m_stageData.StageData[i][1]);
            }

            m_step++;
        }

        if (m_step == 2)
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

                if (transform.position.x > m_initPos.x)
                {
                    if (!Input.GetMouseButton(0))
                    {
                        //m_direction = Vector3.zero;
                        //transform.position = m_initPos;
                        transform.position = Vector3.Lerp(transform.position, m_initPos, Time.deltaTime * 20);
                    }
                }
                if (transform.position.x < m_endPos.x)
                {
                    if (!Input.GetMouseButton(0))
                    {
                        //m_direction = Vector3.zero;
                        //transform.position = new Vector3(m_endPos.x, transform.position.y, transform.position.z);
                        transform.position = Vector3.Lerp(transform.position, new Vector3(m_endPos.x, transform.position.y, transform.position.z), Time.deltaTime * 20);
                    }
                }
            }
            else
            {
                m_direction = Vector3.zero;

            }
        }
    }

    private void FixedUpdate()
    {
        m_direction *= 0.95f;
    }

    void Flick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStartPos = new Vector3(Input.mousePosition.x/500, 0, 0);
        }
        if (Input.GetMouseButton(0))
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
