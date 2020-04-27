using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGimmick : MonoBehaviour
{
    public enum GimmickType
    {
        Goal,
        Water,
        SubMission
    }

    [SerializeField]
    private GimmickType m_type = GimmickType.Goal;
    private bool m_isGimmickOn = false;
    private int m_isGimmickValue = 0;
    private bool m_isUI;


    public GimmickType Type
    {
        get
        {
            return m_type;
        }
        set
        {
            m_type = value;
        }
    }

    public bool UI
    {
        get
        {
            return m_isUI;
        }
        set
        {
            m_isUI = value;
        }
    }
    public int Value
    {
        get
        {
            return m_isGimmickValue;
        }
        set
        {
            m_isGimmickValue = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
