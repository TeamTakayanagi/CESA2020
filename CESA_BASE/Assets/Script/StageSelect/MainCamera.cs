using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    const float radius = 13;
    private const float CAMERA_MOVE = 1.0f;
    [SerializeField]
    private Vector3 m_target = Vector3.zero;
    private bool m_isSceoll;
    private Vector3 m_savePos;
    private float m_moveRotate = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(radius * Mathf.Cos(m_moveRotate), radius * Mathf.Sin(15), radius * Mathf.Sin(m_moveRotate));
        transform.LookAt(Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if(!m_isSceoll && Input.GetMouseButtonDown(1))
        {
            m_isSceoll = true;
            m_savePos = Input.mousePosition;
        }
        else if(m_isSceoll && Input.GetMouseButtonUp(1))
        {
            m_isSceoll = false;
        }
        else if(m_isSceoll && Input.GetMouseButton(1))
        {
            Vector3 difference = Input.mousePosition - m_savePos;
#if false
            if (difference.x > 0.0f)
            {
                transform.RotateAround(target, transform.up, difference.x * Time.deltaTime * CAMERA_MOVE);
                savePos = Input.mousePosition;
            }
            else if (difference.x < 0.0f)
            {
                transform.RotateAround(target, transform.up, difference.x * Time.deltaTime * CAMERA_MOVE);
                savePos = Input.mousePosition;
            }
            if (difference.y > 0.0f)
            {
                transform.RotateAround(target, transform.right, -difference.y * Time.deltaTime * CAMERA_MOVE);
                savePos = Input.mousePosition;
            }
            else if (difference.y < 0.0f)
            {
                transform.RotateAround(target, transform.right, -difference.y * Time.deltaTime * CAMERA_MOVE);
                savePos = Input.mousePosition;
            }
#else
            m_moveRotate -= difference.x * Time.deltaTime * CAMERA_MOVE;
            m_savePos = Input.mousePosition;
            transform.position = new Vector3(radius * Mathf.Cos(m_moveRotate), radius * Mathf.Sin(15), radius * Mathf.Sin(m_moveRotate));
            transform.LookAt(Vector3.zero);
#endif
        }
        // カメラ移動
        else if(scroll != 0.0f)
        {
            Vector3 _pos = transform.position + transform.forward * scroll * ConstDefine.ConstParameter.VALUE_CAMERA;
            float dis = Vector3.Distance(_pos, m_target);
            if (dis > ConstDefine.ConstParameter.CAMERA_NEAR &&
                dis < ConstDefine.ConstParameter.CAMERA_FAR)
            {
                transform.position = _pos;
            }
        }
    }
}
