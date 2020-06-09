using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

[RequireComponent(typeof(Camera))]
public class MainCamera : MonoBehaviour
{
    // デリゲート宣言
    delegate void CameraState();

    private const float ZOOM_NEAR = 30.0f;
    private const float ZOOM_FAR = 60.0f;

    public enum CameraType
    {
        AroundALL,
        AroundY,
        SwipeMove,
        ZoomIn,
        ZoomOut,
        ZoomFade    // フェード演出用
    }

    [SerializeField]
    private CameraType m_type = CameraType.AroundALL;       // カメラの移動タイプ
    [SerializeField]
    private GameObject m_movePlace = null;                  // （注）取得方法模索中

    private CameraType m_defType = CameraType.AroundALL;    // 格納用

    private Vector3 m_savePos = Vector3.zero;               // マウス移動開始地点格納変数
    private Vector3 m_target = Vector3.zero;                // 回転の中心座標もしくは、移動先
    private Vector3 m_targetOld = Vector3.zero;             // 回転の中心座標もしくは、移動先
    private Vector3 m_storePos = Vector3.zero;              // 元の位置格納
    private Vector3 m_moveMax = Vector3.zero;               // 移動範囲最大値
    private Vector3 m_moveMin = Vector3.zero;               // 移動範囲最小値

    private Camera m_myCamera = null;                       // 
    private CameraState m_cameraState;                      // カメラの状態に応じて関数を格納

    private bool m_isScroll = false;                        // スクロール中か
    private bool m_isControl = false;                       // プレイヤーがカメラの操作をできるか
    private float m_moveRotate = 0.0f;                      // 回転の際の初期位置からの角度
    private float m_moveRadiuse = 0.0f;                     // 回転の際の半径
    private float m_near = AdjustParameter.Camera_Constant.CAMERA_NEAR;

    public float Near
    {
        set
        {
            m_near = value;
        }
    }

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
                m_cameraState = null;
                break;
            case CameraType.ZoomOut:
                m_cameraState = CameraZoomOut;
                break;
            case CameraType.ZoomFade:
                m_cameraState = null;
                break;
        }
    }

    void Awake()
    {
        transform.tag = "MainCamera";
        m_defType = m_type;
        m_target = m_targetOld = transform.position;
        SetState();
        m_myCamera = GetComponent<Camera>();
        m_myCamera.fieldOfView = ZOOM_FAR;

        if (m_type == CameraType.AroundY)
        {
            m_moveRadiuse = transform.position.magnitude;
            transform.position = new Vector3(
                m_moveRadiuse * Mathf.Cos(m_moveRotate),
                m_moveRadiuse * Mathf.Sin(AdjustParameter.Camera_Constant.AROUND_ANGLE),
                m_moveRadiuse * Mathf.Sin(m_moveRotate));
            transform.LookAt(Vector3.zero);
        }
        else if(m_type == CameraType.AroundALL)
        {
            m_target = Vector3.zero;
        }

        if (m_movePlace)
        {
            // カメラ移動範囲取得
            m_moveMax = m_movePlace.GetComponent<Renderer>().bounds.max;
            m_moveMin = m_movePlace.GetComponent<Renderer>().bounds.min;
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
        if (!m_isControl || m_cameraState == null)
            return;

        m_cameraState();
    }

    // ズームイン準備
    public void StartZoomIn(Vector3 _zoomObj)
    {
        if (m_type == CameraType.ZoomOut)
            return;

        transform.DOPause();
        if (m_type != CameraType.ZoomIn)
        {
            m_savePos = transform.position;
            m_defType = m_type;
        }
        m_type = CameraType.ZoomIn;
        SetState();
        m_cameraState = null;
        m_target = new Vector3(_zoomObj.x, m_savePos.y + _zoomObj.y - 2, _zoomObj.z - 3.5f);
        transform.DOLocalMove(m_target, AdjustParameter.Camera_Constant.ZOOM_SPEED);
        m_myCamera.DOFieldOfView(ZOOM_NEAR, AdjustParameter.Camera_Constant.ZOOM_SPEED);
    }

    // ズームアウト準備
    public void StartZoomOut()
    {
        if (m_type != CameraType.ZoomIn)
            return;

        transform.DOPause();
        m_type = CameraType.ZoomOut;
        SetState();
        m_target = m_savePos;
        transform.DOLocalMove(m_target, AdjustParameter.Camera_Constant.ZOOM_SPEED);
        m_myCamera.DOFieldOfView(ZOOM_FAR, AdjustParameter.Camera_Constant.ZOOM_SPEED);
    }

    ///////////////////////////////////////////////////////////////////////
    //カメラのモードごとの動き

    // 全方向見渡す
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
    // Y軸回転で見渡す
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

            m_moveRotate -= difference.x * Time.deltaTime * AdjustParameter.Camera_Constant.ROT_Y_VALUE;
            m_savePos = Input.mousePosition;
            transform.position = new Vector3(
                m_moveRadiuse * Mathf.Cos(m_moveRotate),
                m_moveRadiuse * Mathf.Sin(AdjustParameter.Camera_Constant.AROUND_ANGLE),
                m_moveRadiuse * Mathf.Sin(m_moveRotate));
            transform.LookAt(Vector3.zero);
        }

        // 上下左右移動
        //else if (!m_isScroll && Input.GetMouseButtonDown(2))
        //{
        //    m_isScroll = true;
        //    m_savePos = Input.mousePosition;
        //}
        //else if (m_isScroll && Input.GetMouseButtonUp(2))
        //{
        //    m_isScroll = false;
        //}
        //else if (m_isScroll && Input.GetMouseButton(2))
        //{
        //    Vector3 difference = Input.mousePosition - m_savePos;
        //    if (Mathf.Abs(difference.x) >= Mathf.Abs(difference.y))
        //        transform.position -= transform.rotation *
        //            new Vector3(difference.x * Time.deltaTime * AdjustParameter.Camera_Constant.ROT_Y_VALUE, 0.0f, 0.0f);
        //    else
        //        transform.position -= new Vector3(0.0f, difference.y * Time.deltaTime * AdjustParameter.Camera_Constant.ROT_Y_VALUE, 0.0f);
        //
        //    m_savePos = Input.mousePosition;
        //}

        // カメラ手前移動
        else if (scroll != 0.0f)
        {
            float next = m_moveRadiuse - scroll * AdjustParameter.Camera_Constant.VALUE_SCROLL * 10;

            if (next > m_near &&
                next < AdjustParameter.Camera_Constant.CAMERA_FAR)
            {
                m_moveRadiuse = next;
                transform.position = new Vector3(m_moveRadiuse * Mathf.Cos(m_moveRotate), m_moveRadiuse * Mathf.Sin(AdjustParameter.Camera_Constant.AROUND_ANGLE), m_moveRadiuse * Mathf.Sin(m_moveRotate));
                transform.LookAt(Vector3.zero);
            }
        }
    }
    // フリック移動
    void CameraSwipeMove()
    {
        if (m_type != CameraType.SwipeMove) return;

        if (!m_isScroll && Input.GetMouseButtonDown(1))
        {
            m_isScroll = true;
            m_savePos = Input.mousePosition;
            m_storePos = transform.position;
            transform.DOPause();
        }
        else if (m_isScroll && Input.GetMouseButtonUp(1))
        {
            m_isScroll = false;
        }
        else if (m_isScroll && Input.GetMouseButton(1))
        {
            Vector3 difference = Input.mousePosition - m_savePos;
            m_target = m_storePos - new Vector3(
                    difference.x * 0.01f * AdjustParameter.Camera_Constant.SWIPE_MOVE, 0.0f,
                    difference.y * 0.01f * AdjustParameter.Camera_Constant.SWIPE_MOVE);
        }

        if (transform.position != m_target && m_targetOld != m_target)
        {
            transform.DOPause();
            transform.DOMove(m_target, AdjustParameter.Camera_Constant.SWIPE_DERAY);
        }
        m_targetOld = m_target;

        Vector3 pos = new Vector3(
            Mathf.Clamp(transform.position.x, m_moveMin.x, m_moveMax.x), transform.position.y,
            Mathf.Clamp(transform.position.z, m_moveMin.z, m_moveMax.z));

        // 範囲外
        if (transform.position != pos)
        {
            Vector3 bound = (m_target - pos) * -0.3f;
            transform.DOPause();
            transform.DOMove(pos + bound, AdjustParameter.Camera_Constant.SWIPE_DERAY);
        }
    }
    
    // ズームアウトの動き
    void CameraZoomIn()
    {
        if(m_myCamera.fieldOfView < ZOOM_NEAR + 1.0f)
        {
            m_myCamera.fieldOfView = ZOOM_NEAR;
            transform.DOPause();
            m_cameraState = null;
        }
    }
    void CameraZoomOut()
    {
        if(m_myCamera.fieldOfView >= ZOOM_FAR - 1.0f)
        {
            m_myCamera.fieldOfView = ZOOM_FAR;
            m_type = m_defType;
            transform.DOPause();
            m_target = transform.position;
            SetState();
        }
    }

    // フェードの準備
    public void StartZoomFade(Vector3 _zoomObj)
    {
        m_type = CameraType.ZoomFade;
        SetState();

        m_target = _zoomObj;
        transform.DOPause();

        // 筒の上部に移動してからさらに近づく
        transform.position = m_target + new Vector3(0.0f, 10.0f, 0.0f);
        transform.LookAt(m_target);

        transform.DOLocalMove(m_target + new Vector3(0.01f, 3.0f, 0.0f),
            AdjustParameter.Camera_Constant.FADE_DURATION);
   }
}
