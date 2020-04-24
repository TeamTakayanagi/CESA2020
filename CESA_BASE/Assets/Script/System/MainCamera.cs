using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [SerializeField]
    private Vector3 m_target = Vector3.zero;
    [SerializeField]
    private bool m_isAroundCamera = false;
    private Vector3 m_savePos;
    private float m_moveRotate = 90.0f;
    private float m_moveRadiuse = 10.0f;
    private bool m_isScroll = false;
    private bool m_isControl = false;

    [SerializeField]
    private float m_value = 0;

    private Camera m_myCamera = null;
    private Vector3 m_zoomPos = Vector3.zero;
    private float m_zoomSpeed = 5.0f;
    private float m_fov = 0;
    private int m_zoomFlg = 0;  // ０：ズームしていない　１：ズームイン　２：ズームアウト

    public bool Control
    {
        set
        {
            m_isControl = value;
        }
    }

    public int Zoom
    {
        get
        {
            return m_zoomFlg;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (!m_isAroundCamera)
        {
            transform.position = new Vector3(m_moveRadiuse * Mathf.Cos(m_moveRotate), m_moveRadiuse * Mathf.Sin(15), m_moveRadiuse * Mathf.Sin(m_moveRotate));
            transform.LookAt(Vector3.zero);
        }
    }

    private void Start()
    {
        m_myCamera = GetComponent<Camera>();
        m_fov = m_myCamera.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_isControl)
            return;

        if (m_isControl)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            // 周り移動
            if (!m_isScroll && Input.GetMouseButtonDown(1))
            {
                m_isScroll = true;
                m_savePos = Input.mousePosition;
            }
            else if (m_isScroll && Input.GetMouseButtonUp(1))
            {
                m_isScroll = false;
            }
            else if (m_isScroll && Input.GetMouseButton(1))
            {
                Vector3 difference = Input.mousePosition - m_savePos;
                if (m_isAroundCamera)
                {
                    if (Mathf.Abs(difference.x) > AdjustParameter.Camera_Constant.PERMISSION_MOVE)
                    {
                        transform.RotateAround(m_target, transform.up, difference.x * Time.deltaTime * AdjustParameter.Camera_Constant.AROUND_MOVE);
                        m_savePos = Input.mousePosition;
                    }
                    if (Mathf.Abs(difference.y) > AdjustParameter.Camera_Constant.PERMISSION_MOVE)
                    {
                        transform.RotateAround(m_target, transform.right, -difference.y * Time.deltaTime * AdjustParameter.Camera_Constant.AROUND_MOVE);
                        m_savePos = Input.mousePosition;
                    }

                }
                else
                {
                    m_moveRotate -= difference.x * Time.deltaTime * AdjustParameter.Camera_Constant.CAMERA_MOVE;
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
            else if (scroll != 0.0f)
            {
                if (m_isAroundCamera)
                {
                    Vector3 _pos = transform.position + transform.forward * scroll * AdjustParameter.Camera_Constant.VALUE_CAMERA;
                    float dis = Vector3.Distance(_pos, m_target);
                    if (dis > AdjustParameter.Camera_Constant.CAMERA_NEAR &&
                        dis < AdjustParameter.Camera_Constant.CAMERA_FAR)
                    {
                        transform.position = _pos;
                    }
                }
                else
                {
                    float next = m_moveRadiuse - scroll * AdjustParameter.Camera_Constant.VALUE_CAMERA * 10;

                    if (next > AdjustParameter.Camera_Constant.CAMERA_NEAR &&
                        next < AdjustParameter.Camera_Constant.CAMERA_FAR)
                    {
                        m_moveRadiuse = next;
                        transform.position = new Vector3(m_moveRadiuse * Mathf.Cos(m_moveRotate), m_moveRadiuse * Mathf.Sin(15), m_moveRadiuse * Mathf.Sin(m_moveRotate));
                        transform.LookAt(Vector3.zero);
                    }
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (m_zoomFlg == 1)
        {
            transform.position = Vector3.Lerp(transform.position, m_zoomPos, Time.deltaTime * m_zoomSpeed);
            m_myCamera.fieldOfView = Mathf.Lerp(m_myCamera.fieldOfView, m_value, Time.deltaTime * m_zoomSpeed);
        }
        else if (m_zoomFlg == 2)
        {
            transform.position = Vector3.Lerp(transform.position, m_zoomPos, Time.deltaTime * m_zoomSpeed);
            m_myCamera.fieldOfView = Mathf.Lerp(m_myCamera.fieldOfView, m_fov, Time.deltaTime * m_zoomSpeed);
            if (Mathf.RoundToInt(m_myCamera.fieldOfView) >= m_fov)
            {
                m_zoomFlg = 0;
            }
        }
    }

    public void ZoomIn(Vector3 _stagePos)
    {

        if (m_zoomFlg == 1)
        {
            m_zoomPos = new Vector3(_stagePos.x, transform.position.y, _stagePos.z - 90);
            //transform.position = _setPos;
        }
        else if (m_zoomFlg == 0)
        {
            m_zoomFlg = 1;
            m_zoomPos = new Vector3(_stagePos.x, transform.position.y - 10, _stagePos.z - 90);
            //transform.position = _setPos;
            //m_myCamera.fieldOfView -= m_value;
        }
    }

    public void ZoomOut()
    {
        m_zoomFlg = 2;
        m_zoomPos = new Vector3(transform.position.x, transform.position.y + 10, transform.position.z);
        //transform.position = _setPos;
        //m_myCamera.fieldOfView += m_value;
    }
}
