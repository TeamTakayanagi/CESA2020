using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTutorial : MonoBehaviour
{
    [SerializeField]
    private GameObject m_tutorialObjPrefab = null;

    private GameObject m_tutorialObj = null;
    private GameObject m_resultCanvas = null;

    // Start is called before the first frame update
    void Start()
    {
        m_resultCanvas = GameObject.FindGameObjectWithTag(NameDefine.TagName.Result);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Click()
    {
        if (m_tutorialObj == null)
        {
            m_tutorialObj = Instantiate(m_tutorialObjPrefab, m_resultCanvas.transform);
        }
        else
        {
            Destroy(m_tutorialObj);
        }
    }
}
