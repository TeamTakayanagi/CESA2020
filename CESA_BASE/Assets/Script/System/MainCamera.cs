﻿using System.Collections;
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
    private CameraType m_default = CameraType.AroundALL;    // 格納用

    private Vector3 m_savePos = Vector3.zero;               // 差分計算のための移動開始地点格納変数
    private Vector3 m_target = Vector3.zero;                // 回転の中心座標もしくは、移動先
    private Vector3 m_storePos = Vector3.zero;              // 元の位置格納
    private CameraState m_cameraState;                      // カメラの状態に応じて関数を格納

    private float m_moveRotate = 0.0f;                      // 回転の際の初期位置からの角度
    private float m_moveRadiuse = 0.0f;                     // 回転の際の半径
    private bool m_isScroll = false;                        // スクロール中か
    private bool m_isControl = false;                       // プレイヤーがカメラの操作をできるか
    private Camera m_myCamera = null;

    //--- フェード演出用 ---
    private Vector3 m_scaleMax = Vector3.zero;              // マスクの最初の大きさ
    private Vector3 m_scaleMin = Vector3.zero;              // マスクの縮小後のサイズ
    [SerializeField]
    private GameObject m_fadeMask;                          // マスク

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
            case CameraType.ZoomFade:           // フェード演出用
                m_cameraState = ZoomFade;
                break;
        }
    }


    void Awake()
    {
        DOTween.SetTweensCapacity(1250, 3125);

        transform.tag = "MainCamera";
        m_default = m_type;
        m_target = transform.position;
        SetState();

        m_myCamera = GetComponent<Camera>();
        m_myCamera.fieldOfView = ZOOM_FAR;

        if (m_type == CameraType.AroundY)
        {
            m_moveRadiuse = transform.position.magnitude;
            transform.position = new Vector3(m_moveRadiuse * Mathf.Cos(m_moveRotate), m_moveRadiuse * Mathf.Sin(AdjustParameter.Camera_Constant.AROUND_ANGLE), m_moveRadiuse * Mathf.Sin(m_moveRotate));
            transform.LookAt(Vector3.zero);
        }
        else if(m_type == CameraType.AroundALL)
        {
            m_target = Vector3.zero;
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

    // ズームイン準備
    public void StartZoomIn(Vector3 _zoomObj)
    {
        m_savePos = transform.position;

        if (m_type != CameraType.ZoomIn)
        {
            m_default = m_type;
            m_type = CameraType.ZoomIn;
        }
        SetState();
        m_target = new Vector3(_zoomObj.x, transform.position.y, _zoomObj.z - transform.position.y * 1.5f);
    }

    // ズームアウト準備
    public void StartZoomOut()
    {
        m_type = CameraType.ZoomOut;
        SetState();
        m_target = m_savePos;
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

            m_moveRotate -= difference.x * Time.deltaTime * AdjustParameter.Camera_Constant.ROTY_VALUE;
            m_savePos = Input.mousePosition;
            transform.position = new Vector3(m_moveRadiuse * Mathf.Cos(m_moveRotate), m_moveRadiuse * Mathf.Sin(15), m_moveRadiuse * Mathf.Sin(m_moveRotate));
            transform.LookAt(Vector3.zero);
        }

        // 上下左右移動
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
            if (Mathf.Abs(difference.x) >= Mathf.Abs(difference.y))
                transform.position -= transform.rotation *
                    new Vector3(difference.x * Time.deltaTime * AdjustParameter.Camera_Constant.ROTY_VALUE, 0.0f, 0.0f);
            else
                transform.position -= new Vector3(0.0f, difference.y * Time.deltaTime * AdjustParameter.Camera_Constant.ROTY_VALUE, 0.0f);

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
    // フリック移動
    void CameraSwipeMove()
    {
        if (!m_isScroll && Input.GetMouseButtonDown(1))
        {
            m_isScroll = true;
            m_savePos = Input.mousePosition;
            m_storePos = transform.position;
        }
        else if (m_isScroll && Input.GetMouseButtonUp(1))
        {
            m_isScroll = false;
        }
        else if (m_isScroll && Input.GetMouseButton(1))
        {
            Vector3 difference = Input.mousePosition - m_savePos;

            m_target = m_storePos - new Vector3(
                    difference.x * Time.deltaTime * AdjustParameter.Camera_Constant.SWIPE_MOVE, 0.0f,
                    difference.y * Time.deltaTime * AdjustParameter.Camera_Constant.SWIPE_MOVE);
        }

        if(transform.position != m_target)
            transform.DOMove(m_target, AdjustParameter.Camera_Constant.SWIPE_DERAY);
    }
    // ズームインの動き
    void CameraZoomIn()
    {
        if (m_myCamera.fieldOfView == ZOOM_NEAR)
            return;

        transform.DOLocalMove(m_target, AdjustParameter.Camera_Constant.ZOOM_SPEED);
        m_myCamera.DOFieldOfView(ZOOM_NEAR, AdjustParameter.Camera_Constant.ZOOM_SPEED);
    }
    // ズームアウトの動き
    void CameraZoomOut()
    {
        transform.DOLocalMove(m_target, AdjustParameter.Camera_Constant.ZOOM_SPEED);
        m_myCamera.DOFieldOfView(ZOOM_FAR, AdjustParameter.Camera_Constant.ZOOM_SPEED);
        if(m_myCamera.fieldOfView >= ZOOM_FAR - 1.0f)
        {
            m_myCamera.fieldOfView = ZOOM_FAR;
            m_type = m_default;
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
        m_scaleMax = m_fadeMask.transform.localScale;
        m_scaleMin = new Vector3(0f, 0f, 0f);
    }

    // フェードするときの動き
    void ZoomFade()
    {
        //float speed = 1f;

        // 筒の上部に移動してからさらに近づく
        Vector3 path = new Vector3(m_target.x, m_target.y + 200f, m_target.z);
        transform.DOLocalMove(path, AdjustParameter.Camera_Constant.FADE_DURATION);          
        transform.DOLocalMove(m_target, AdjustParameter.Camera_Constant.FADE_DURATION);

        m_fadeMask.transform.DOScale(m_scaleMin, AdjustParameter.Camera_Constant.FADE_DURATION);    // マスク縮小

        //transform.position = Vector3.Lerp(transform.position, path, speed * Time.deltaTime);
        //transform.position = Vector3.Lerp(transform.position, m_target, speed * Time.deltaTime);

        transform.LookAt(m_target);
    }
}
