using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : ClickedObject
{
    private float m_redian = 0;
    private Vector3 m_initPos = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        m_initPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnClick()
    {
        StartCoroutine("SwaysTree");

        if (transform.childCount > 0)
        {
            transform.GetChild(0).GetComponent<Mouse>().Run();
        }
    }

    private IEnumerator SwaysTree()
    {
        while (m_redian < 12)
        {
            m_redian += Time.deltaTime * 10;
            transform.position += Vector3.right * 0.003f * Mathf.Sin(m_redian);

            yield return null;
        }

        transform.position = m_initPos;
        m_redian = 0;
        StopCoroutine("SwaysTree");
        yield return null;
    }
}
