using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMgr : SingletonMonoBehaviour<StageMgr>
{
    private Vector3 m_touchStartPos;
    private Vector3 m_touchEndPos;
    private Vector3 m_direction;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);

    }

    // Update is called once per frame
    void Update()
    {
        if (TitleMgr.Instance.Step < 7) return;

        Scroll();
        Camera.main.transform.position += m_direction;

    }

    private void FixedUpdate()
    {
        m_direction *= 0.9f;
    }

    void Scroll()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_touchStartPos = new Vector3(Input.mousePosition.x / 500, 0, Input.mousePosition.y / 500);
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 _touchiNow = new Vector3(Input.mousePosition.x / 500, 0, Input.mousePosition.y / 500);

            {
                m_touchEndPos = _touchiNow;
                m_direction = m_touchStartPos - m_touchEndPos;
            }
        }
    }

}
