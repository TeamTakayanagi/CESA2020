using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [SerializeField]
    private Vector3 m_target = Vector3.zero;
    private Vector3 m_savePos;
    private float m_moveRotate = 90.0f;
    private float m_moveRadiuse = 10.0f;
    private bool m_isScroll = false;
    [SerializeField]
    private bool m_isAroundCamera = false;
    private const float CAMERA_MOVE = 1.0f;
    private const float AROUND_MOVE = 10.0f;

    // Start is called before the first frame update
    void Awake()
    {
        if (!m_isAroundCamera)
        {
            transform.position = new Vector3(m_moveRadiuse * Mathf.Cos(m_moveRotate), m_moveRadiuse * Mathf.Sin(15), m_moveRadiuse * Mathf.Sin(m_moveRotate));
            transform.LookAt(Vector3.zero);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        // 周り移動
        if(!m_isScroll && Input.GetMouseButtonDown(1))
        {
            m_isScroll = true;
            m_savePos = Input.mousePosition;
        }
        else if(m_isScroll && Input.GetMouseButtonUp(1))
        {
            m_isScroll = false;
        }
        else if(m_isScroll && Input.GetMouseButton(1))
        {
            Vector3 difference = Input.mousePosition - m_savePos;
            if (m_isAroundCamera)
            {
                if (Mathf.Abs(difference.x) > 2.0f)
                {
                    transform.RotateAround(m_target, transform.up, difference.x * Time.deltaTime * AROUND_MOVE);
                    m_savePos = Input.mousePosition;
                }
                if (Mathf.Abs(difference.y) > 2.0f)
                {
                    transform.RotateAround(m_target, transform.right, -difference.y * Time.deltaTime * AROUND_MOVE);
                    m_savePos = Input.mousePosition;
                }

            }
            else
            {
                m_moveRotate -= difference.x * Time.deltaTime * CAMERA_MOVE;
                m_savePos = Input.mousePosition;
                transform.position = new Vector3(m_moveRadiuse * Mathf.Cos(m_moveRotate), m_moveRadiuse * Mathf.Sin(15), m_moveRadiuse * Mathf.Sin(m_moveRotate));
                transform.LookAt(Vector3.zero);
            }
        }
        // 左右移動
        else if (!m_isAroundCamera && !m_isScroll && Input.GetMouseButtonDown(2))
        {
            m_isScroll = true;
            m_savePos = Input.mousePosition;
        }
        else if (!m_isAroundCamera && m_isScroll && Input.GetMouseButtonUp(2))
        {
            m_isScroll = false;
        }
        else if (!m_isAroundCamera && m_isScroll && Input.GetMouseButton(2))
        {
            Vector3 difference = Input.mousePosition - m_savePos;
            transform.position -= transform.rotation * new Vector3(difference.x * Time.deltaTime, 0.0f, 0.0f);
            m_savePos = Input.mousePosition;
        }
       // カメラ手前移動
        else if(scroll != 0.0f)
        {
            if (m_isAroundCamera)
            {
                Vector3 _pos = transform.position + transform.forward * scroll * ConstDefine.ConstParameter.VALUE_CAMERA;
                float dis = Vector3.Distance(_pos, m_target);
                if (dis > ConstDefine.ConstParameter.CAMERA_NEAR &&
                    dis < ConstDefine.ConstParameter.CAMERA_FAR)
                {
                    transform.position = _pos;
                }
            }
            else
            {
                float next = m_moveRadiuse - scroll * ConstDefine.ConstParameter.VALUE_CAMERA * 10;

                if (next > ConstDefine.ConstParameter.CAMERA_NEAR &&
                    next < ConstDefine.ConstParameter.CAMERA_FAR)
                {
                    m_moveRadiuse = next;
                    transform.position = new Vector3(m_moveRadiuse * Mathf.Cos(m_moveRotate), m_moveRadiuse * Mathf.Sin(15), m_moveRadiuse * Mathf.Sin(m_moveRotate));
                    transform.LookAt(Vector3.zero);
                }
            }
        }
    }
}
