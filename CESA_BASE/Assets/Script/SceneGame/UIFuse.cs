using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFuse : MonoBehaviour
{
    [System.Serializable]
    private struct FuseStatus
    {
        public Fuse prefab;
        public Vector3 rotate;
    }

    [SerializeField]
    List<FuseStatus> m_uiList = new List<FuseStatus>();
    
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < m_uiList.Count; ++i)
        {
            Fuse _fuse = Instantiate(m_uiList[i].prefab, transform.position, Quaternion.identity);
            _fuse.transform.SetParent(transform, true);
            _fuse.transform.localPosition = new Vector3((i & 1) * 2.0f - 1.0f, 9.0f - (int)(i / 2) * 2.0f, 5.0f);
            _fuse.transform.localEulerAngles = m_uiList[i].rotate;
            _fuse.Type = Fuse.FuseType.UI;
            _fuse.transform.tag = ConstDefine.TagName.Fuse;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
