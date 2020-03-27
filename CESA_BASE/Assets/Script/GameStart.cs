using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    private PopUp m_objParent = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_objParent == null)
        {
            m_objParent = GetComponentInParent<PopUp>();

            transform.GetComponent<RectTransform>().sizeDelta = m_objParent.RectTrans.sizeDelta * 0.125f;
        }
    }

    public void Click()
    {
        GameObject.FindGameObjectWithTag("Fade").GetComponent<FadeMgr>().StartFade("Alpha");
    }
}
