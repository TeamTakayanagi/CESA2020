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

    public int StageNum
    {
        get
        {
            return m_stageNum;
        }
    }
}
