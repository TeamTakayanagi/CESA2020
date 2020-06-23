using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditerUICreate : MonoBehaviour
{
    private enum Child
    {
        Fuse, 
        Gimmck
    }

    private Text m_uiChange = null;
    private int m_activeChild = 0;

    void Awake()
    {
        m_uiChange = GetComponentInChildren<Text>();
        m_uiChange.text = transform.GetChild(m_activeChild).name;
        StageCreateMgr.Instance.CreateUIFuse(ProcessedtParameter.GameObject_Constant.FUSE_TYPE,
            transform.GetChild((int)Child.Fuse), StageCreateMgr.SuffixType.Turn, StageCreateMgr.SuffixType.Zero, false);
        StageCreateMgr.Instance.CreateUIGimmck(transform.GetChild((int)Child.Gimmck));
        transform.GetChild(m_activeChild ^ 1).gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UIChange()
    {
        m_activeChild ^= 1;
        transform.GetChild(m_activeChild).gameObject.SetActive(true);
        transform.GetChild(m_activeChild ^ 1).gameObject.SetActive(false);
        m_uiChange.text = transform.GetChild(m_activeChild).name;
    }
}
