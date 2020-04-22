using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIFuse : MonoBehaviour
{
    [System.Serializable]
    private class FuseStatus
    {
        public Fuse prefab = null;
        public Vector3 rotate = Vector3.zero;
    }

    [SerializeField]
    GameObject m_uiColider = null;
    [SerializeField]
    List<FuseStatus> m_uiList = new List<FuseStatus>();

    public const int CREATE_COOUNT = 60;
    private string[] m_FadeTag = new string[7];

    // Start is called before the first frame update
    void Awake()
    {
        m_FadeTag[0] = ConstDefine.Fuse.FuseI;
        m_FadeTag[1] = ConstDefine.Fuse.FuseL;
        m_FadeTag[2] = ConstDefine.Fuse.FuseT;
        m_FadeTag[3] = ConstDefine.Fuse.FuseX;
        m_FadeTag[4] = ConstDefine.Fuse.FuseLL;
        m_FadeTag[5] = ConstDefine.Fuse.FuseTT;
        m_FadeTag[6] = ConstDefine.Fuse.FuseXX;

        for(int i = 0; i < m_uiList.Count; ++i)
        {
            Fuse _fuse = Instantiate(m_uiList[i].prefab, transform.position, Quaternion.identity);
            _fuse.transform.SetParent(transform, true);
            _fuse.transform.localPosition = new Vector3((i & 1) * 2.0f - 1.0f, 1.0f + (int)(i / 2) * 2.0f, 5.0f);
            _fuse.transform.localEulerAngles = m_uiList[i].rotate;
            _fuse.Type = Fuse.FuseType.UI;
            _fuse.transform.tag = m_FadeTag[i];

            // UI専用のコライダーを子供に
            GameObject _colider = Instantiate(m_uiColider, _fuse.transform.position, Quaternion.identity);
            _colider.transform.SetParent(_fuse.transform, true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
