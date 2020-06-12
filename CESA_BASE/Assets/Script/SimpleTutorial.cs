using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTutorial : MonoBehaviour
{
    [SerializeField]
    private GameObject m_imagePrefab = null;

    private GameObject m_image = null;
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
        if (Input.GetMouseButtonUp(0))
        {
            if (m_image == null)
            {
                m_image = Instantiate(m_imagePrefab, m_resultCanvas.transform);
            }
            else
            {
                Destroy(m_image);
            }
        }
    }
}
