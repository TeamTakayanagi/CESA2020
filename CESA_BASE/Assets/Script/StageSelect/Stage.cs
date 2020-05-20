using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage : MonoBehaviour
{
    private int m_stageNum = 0;

    public int StageNum
    {
        get
        {
            return m_stageNum;
        }
        set
        {
            m_stageNum = value;
        }
    }
}
