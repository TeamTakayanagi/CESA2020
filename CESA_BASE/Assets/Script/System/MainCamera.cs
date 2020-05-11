using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Camera))]
public class MainCamera : MonoBehaviour
{
    // デリゲート宣言
    delegate void CameraState();

    private const float ZOOM_NEAR = 30.0f;
    private const float ZOOM_FAR = 60.0f;
    private const float ZOOM_DISTANCE_Z = 100.0f;

    public enum CameraType
    {
        AroundALL,
        AroundY,
        SwipeMove,
        ZoomIn,
        ZoomOut
    }

    private Vector3 m_target = Vector3.zero;
    [SerializeField]
    private CameraState m_cameraState;
    [SerializeField]
    private CameraType m_type = CameraType.AroundALL;
    private Vector3 m_savePos = Vector3.zero;
    private Vector3 m_move = Vector3.zero;

    private float m_moveRotate = 0.0f;
    private float m_moveRadiuse = 0.0f;
    private bool m_isScroll = false;
    private bool m_isControl = false;
    private bool m_isMove = false;


    private Camera m_myCamera = null;
    private Vector3 m_zoomPos = Vector3.zero;
    
    public bool Control
    {
        set
        {
            m_isControl = value;
        }
    }
    public CameraType Type
    {
        get
        {
            return m_type;
        }
        set
        {
            m_type = value;
            SetState();
        }
    }

    void SetState()
    {
        switch (m_type)
        {
            case CameraType.AroundALL:
                m_cameraState = CameraAroundAll;
                break;
            case CameraType.AroundY:
                m_cameraState = CameraAroundY;
                break;
            case CameraType.SwipeMove:
                m_cameraState = CameraSwipeMove;
                break;
            case CameraType.ZoomIn:
                m_cameraState = CameraZoomIn;
                break;
            case CameraType.ZoomOut:
                m_cameraState = CameraZoomOut;
                break;
        }
    }


    void Awake()
    {
        transform.tag = "MainCamera";
        SetState();

        m_myCamera = GetComponent<Camera>();
        m_myCamera.fieldOfView = ZOOM_FAR;

        if (m_type == CameraType.AroundY)
        {
            m_moveRadiuse = transform.position.magnitude;
            transform.position = new Vector3(m_moveRadiuse * Mathf.Cos(m_moveRotate), m_moveRadiuse * Mathf.Sin(AdjustParameter.Camera_Constant.AROUND_ANGLE), m_moveRadiuse * Mathf.Sin(m_moveRotate));
            transform.LookAt(Vector3.zero);
        }
    }

    void Update()
    {
        // ゲーム終了処理
        if (Input.GetKey(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        // カメラ操作無効のフラグを立てているなら
        if (!m_isControl)
            return;

        m_cameraState();
    }

///////////////////////////////////////////////////////////////////////
//カメラのモードごとの動き
    void CameraAroundAll()
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
        // カメラ手前移動
        else if (scroll != 0.0f)
        {
            Vector3 _pos = transform.position + transform.forward * scroll * AdjustParameter.Camera_Constant.VALUE_SCROLL;
            float dis = Vector3.Distance(_pos, m_target);
            if (dis > AdjustParameter.Camera_Constant.CAMERA_NEAR &&
                dis < AdjustParameter.Camera_Constant.CAMERA_FAR)
            {
                transform.position = _pos;
            }
        }
    }
    void CameraAroundY()
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

            m_moveRotate -= difference.x * Time.deltaTime * AdjustParameter.Camera_Constant.ROTY_VALUE;
            m_savePos = Input.mousePosition;
            transform.position = new Vector3(m_moveRadiuse * Mathf.Cos(m_moveRotate), m_moveRadiuse * Mathf.Sin(15), m_moveRadiuse * Mathf.Sin(m_moveRotate));
            transform.LookAt(Vector3.zero);
        }

        // 左右移動
        else if (!m_isScroll && Input.GetMouseButtonDown(2))
        {
            m_isScroll = true;
            m_savePos = Input.mousePosition;
        }
        else if (m_isScroll && Input.GetMouseButtonUp(2))
        {
            m_isScroll = false;
        }
        else if (m_isScroll && Input.GetMouseButton(2))
        {
            Vector3 difference = Input.mousePosition - m_savePos;
            transform.position -= transform.rotation * 
                new Vector3(difference.x * Time.deltaTime * AdjustParameter.Camera_Constant.ROTY_VALUE, 0.0f, 0.0f);
            m_savePos = Input.mousePosition;
        }

        // カメラ手前移動
        else if (scroll != 0.0f)
        {
            float next = m_moveRadiuse - scroll * AdjustParameter.Camera_Constant.VALUE_SCROLL * 10;

            if (next > AdjustParameter.Camera_Constant.CAMERA_NEAR &&
                next < AdjustParameter.Camera_Constant.CAMERA_FAR)
            {
                m_moveRadiuse = next;
                transform.position = new Vector3(m_moveRadiuse * Mathf.Cos(m_moveRotate), m_moveRadiuse * Mathf.Sin(15), m_moveRadiuse * Mathf.Sin(m_moveRotate));
                transform.LookAt(Vector3.zero);
            }
        }
    }
    void CameraSwipeMove()
    {
        if (!m_isScroll && Input.GetMouseButtonDown(1))
        {
            m_isScroll = true;
            m_savePos = Input.mousePosition;
        }
        else if (m_isScroll && Input.GetMouseButtonUp(1))
        {
            m_isScroll = false;
            m_isMove = true;
            Vector3 difference = Input.mousePosition - m_savePos;
            m_move = transform.position - new Vector3(
                difference.x * Time.deltaTime * AdjustParameter.Camera_Constant.SWIPE_VALUE, 0.0f,
                difference.y * Time.deltaTime * AdjustParameter.Camera_Constant.SWIPE_VALUE);
        }
        else if (m_isScroll && Input.GetMouseButton(1))
        {
            Vector3 difference = Input.mousePosition - m_savePos;
            //transform.position -= new Vector3(
            //    difference.x * Time.deltaTime * AdjustParameter.Camera_Constant.SWIPE_VALUE, 0.0f,
            //    difference.y * Time.deltaTime * AdjustParameter.Camera_Constant.SWIPE_VALUE);
            //m_savePos = Input.mousePosition;
        }

        if (m_isMove)
        {
            transform.DOMove(m_move, 0.5f);
            if (transform.position == m_move)
                m_isMove = false;
        }
    }

    void CameraZoomIn()
    {
        transform.DOLocalMove(m_zoomPos, AdjustParameter.Camera_Constant.ZOOM_SPEED);
        m_myCamera.DOFieldOfView(ZOOM_NEAR, AdjustParameter.Camera_Constant.ZOOM_SPEED);
    }

    void CameraZoomOut()
    {
        transform.DOLocalMove(m_zoomPos, AdjustParameter.Camera_Constant.ZOOM_SPEED);
        m_myCamera.DOFieldOfView(ZOOM_FAR, AdjustParameter.Camera_Constant.ZOOM_SPEED);
        if(m_myCamera.fieldOfView >= ZOOM_FAR - 1.0f)
        {
            m_myCamera.fieldOfView = ZOOM_FAR;
            m_type = CameraType.SwipeMove;
            SetState();
        }
    }


    public void StartZoomIn(Vector3 _zoomObj)
    {
        m_savePos = transform.position;
        if (m_type == CameraType.ZoomIn)
        {
            m_zoomPos = new Vector3(_zoomObj.x, transform.position.y, _zoomObj.z - transform.position.y * 1.5f);
        }
        else if (m_type != CameraType.ZoomIn && m_type != CameraType.ZoomOut)
        {
            m_type = CameraType.ZoomIn;
            SetState();
            m_zoomPos = new Vector3(_zoomObj.x, transform.position.y, _zoomObj.z - transform.position.y * 1.5f);
        }
    }

    public void StartZoomOut()
    {
        m_type = CameraType.ZoomOut;
        SetState();
        m_zoomPos = m_savePos;
    }
}
