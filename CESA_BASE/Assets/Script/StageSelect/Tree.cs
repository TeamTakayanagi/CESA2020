using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : ClickedObject
{
    private float m_redian = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnClick()
    {   
        StartCoroutine("SwaysTree");
    }

    private IEnumerator SwaysTree()
    {
        while (m_redian < 9)
        {
            m_redian += Time.deltaTime * 10;
            transform.localEulerAngles = Vector3.forward * 3 * Mathf.Sin(m_redian);

            yield return null;
        }

        transform.rotation = Quaternion.identity;
        m_redian = 0;
        StopCoroutine("SwaysTree");
        yield return null;
    }
}
