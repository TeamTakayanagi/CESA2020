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
    
    // Start is called before the first frame update
    void Awake()
    {
        for(int i = 0; i < m_uiList.Count; ++i)
        {
            Fuse _fuse = Instantiate(m_uiList[i].prefab, transform.position, Quaternion.identity);
            _fuse.transform.SetParent(transform, true);
            _fuse.transform.localPosition = new Vector3((i & 1) * 2.0f - 1.0f, 9.0f - (int)(i / 2) * 2.0f, 5.0f);
            _fuse.transform.localEulerAngles = m_uiList[i].rotate;
            _fuse.Type = Fuse.FuseType.UI;
            _fuse.transform.tag = ConstDefine.TagName.Fuse;
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
