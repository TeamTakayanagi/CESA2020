using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rat : MonoBehaviour
{
    private float m_speed;
    private float m_radius;         // 半径
    private float m_radian;

    private void Awake()
    {
        m_radian = -90 * Mathf.Deg2Rad;
        m_speed = 3.0f;
        m_radius = 0.2f;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        

    }

    public void Run()
    {
        StartCoroutine("RunAround");
    }

    private IEnumerator RunAround()
    {
        while (m_radian <= Mathf.Sin(30 * Mathf.Deg2Rad))
        {
            m_radian += Time.deltaTime;
            transform.position = new Vector3(transform.parent.position.x + m_radius * Mathf.Cos(m_radian * m_speed),
                                            0.05f,
                                            transform.parent.position.z + m_radius * Mathf.Sin(m_radian * m_speed));
            transform.LookAt(transform.parent.position);
            transform.localEulerAngles += new Vector3(0, 90);

            yield return null;
        }

        m_radian = -90 * Mathf.Deg2Rad;
        StopCoroutine("RunAround");
        yield return null;
    }
}
