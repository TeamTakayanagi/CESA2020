using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : SingletonMonoBehaviour<GameMgr>
{
    [SerializeField]
    private Vector3 m_stageSizeMax = Vector3.zero;
    [SerializeField]
    private Vector3 m_stageSizeMin = Vector3.zero;

    public Vector3 StageSizeMax
    {
        get
        {
            return m_stageSizeMax;
        }
    }
    public Vector3 StageSizeMin
    {
        get
        {
            return m_stageSizeMin;
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
