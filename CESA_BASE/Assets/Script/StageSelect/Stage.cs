using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage : MonoBehaviour
{
    [SerializeField]
    private int m_stageNum = 0;
    private bool m_isClear;
    private bool m_isSelect;


    private MainCamera m_camera = null;

    public int StageNum
    {
        get
        {
            return m_stageNum;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_camera = Camera.main.GetComponent<MainCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (TitleMgr.Instance.Step != TitleMgr.TitleStep.Select) 
            return;

    }
}
