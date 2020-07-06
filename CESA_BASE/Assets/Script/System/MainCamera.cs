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

    private const float FIELD_VIEW = 60.0f;
    private readonly Vector3 FADE_UP = new Vector3(0.0f, 10.0f, 0.0f);          // 導火線のどれくらい上にフェード画面を持っていくか
    private readonly Vector3 FADE_INTO = new Vector3(0.01f, 5.0f, 0.0f);        // 除きこむ座標

    public enum CameraType
    {
        AroundALL,
        AroundY,
        AroundDome,
        SwipeMove,
        ZoomIn,
        ZoomOut,
        ZoomFade    // フェード演出用
    }

    [SerializeField]
    private CameraType m_type = CameraType.AroundALL;       // カメラの移動タイプ
    [SerializeField]
    private GameObject m_movePlace = null;                  // （注）取得方法模索中

    private CameraState m_cameraState;                      // カメラの状態に応じて関数を格納
    private CameraType m_defType = CameraType.AroundALL;    // 格納用

    private Vector3 m_savePos = Vector3.zero;               // マウス移動開始地点格納変数
    private Vector3 m_target = Vector3.zero;                // 回転の中心座標もしくは、移動先
    private Vector3 m_targetOld = Vector3.zero;             // 回転の中心座標もしくは、移動先
    private Vector3 m_storePos = Vector3.zero;              // 元の位置格納
    private Vector3 m_moveMax = Vector3.zero;               // 移動範囲最大値
    private Vector3 m_moveMin = Vector3.zero;               // 移動範囲最小値


    private bool m_isScroll = false;                        // スクロール中か
    private bool m_isControl = false;                       // プレイヤーがカメラの操作をできるか
    private float m_moveRotate = 0.0f;                      // 回転の際の初期位置からの角度
    private float m_moveRadiuse = 0.0f;                     // 回転の際の半径
    private float m_cameraDistance = AdjustParameter.Camera_Constant.CAMERA_DISTANCE;                     // 回転の際の半径
    private float m_cameraHeight = AdjustParameter.Camera_Constant.CAMERA_HEIGHT;
    private float m_near = AdjustParameter.Camera_Constant.CAMERA_NEAR;

    public float Near
    {
        set
        {
            m_near = value;
        }
    }
    public float Height
    {
        set
        {
            m_cameraHeight = value;
        }
    }
    public float Distance
    {
        set
        {
            m_cameraDistance = value;
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
            case CameraType.AroundDome:
                m_cameraState = CameraDome;
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
            case CameraType.ZoomFade:
                m_cameraState = null;
                break;
        }
    }

    public void InitCamera()
    {
        if (m_type == CameraType.AroundY)
        {
            m_target = m_targetOld = Vector3.zero;
            m_moveRadiuse = transform.position.magnitude;
            transform.position = new Vector3(
                m_moveRadiuse * Mathf.Cos(Mathf.Deg2Rad * m_moveRotate),
                m_moveRadiuse * Mathf.Sin(Mathf.Deg2Rad * AdjustParameter.Camera_Constant.AROUND_ANGLE),
                m_moveRadiuse * Mathf.Sin(Mathf.Deg2Rad * m_moveRotate));
            transform.LookAt(m_target);
        }
        else if (m_type == CameraType.AroundALL)
        {
            m_target = m_targetOld = Vector3.zero;
            m_storePos = transform.position;
        }
        else if (m_type == CameraType.AroundDome)
        {
            m_target = m_targetOld = Vector3.zero;
            transform.position = new Vector3(0.0f, m_cameraHeight, m_cameraDistance);
            m_storePos = transform.position;
            transform.LookAt(m_target);
            transform.RotateAround(m_target, transform.right, 30);
        }
        else
            m_target = m_targetOld = transform.position;
    }

    void Awake()
    {
        // キャパシティーの設定
        DOTween.SetTweensCapacity(3125, 3125);

        transform.tag = "MainCamera";
        m_defType = m_type;
        SetState();

        // カメラの設定
        Camera myCamera = GetComponent<Camera>();
        myCamera.fieldOfView = FIELD_VIEW;

        InitCamera();

        // 移動範囲のオブジェクトがある場合
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
        // ズームインに移行する時なら
        if (m_type != CameraType.ZoomIn)
        {
            // 今の情報を格納
            m_savePos = transform.position;
            m_defType = m_type;
        }
        m_type = CameraType.ZoomIn;
        SetState();
        m_cameraState = null;

        m_target = new Vector3(_zoomObj.x,
            _zoomObj.y + AdjustParameter.Camera_Constant.ZOOM_LENGTH * Mathf.Sin(Mathf.Deg2Rad * transform.localEulerAngles.x),
            _zoomObj.z - AdjustParameter.Camera_Constant.ZOOM_LENGTH * Mathf.Cos(Mathf.Deg2Rad * transform.localEulerAngles.x));

        transform.DOLocalMove(m_target, AdjustParameter.Camera_Constant.ZOOM_SPEED);
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
                transform.RotateAround(m_target, transform.up,
                    difference.x * Time.deltaTime * AdjustParameter.Camera_Constant.AROUND_MOVE);
                m_savePos = Input.mousePosition;
            }
            if (Mathf.Abs(difference.y) > AdjustParameter.Camera_Constant.PERMISSION_MOVE)
            {
                transform.RotateAround(m_target, transform.right,
                    -difference.y * Time.deltaTime * AdjustParameter.Camera_Constant.AROUND_MOVE);
                m_savePos = Input.mousePosition;
            }
        }
        // カメラ手前移動
        else if (scroll != 0.0f)
        {
            Vector3 _pos = transform.position + transform.forward * 
                scroll * AdjustParameter.Camera_Constant.VALUE_SCROLL;
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
                m_moveRadiuse * Mathf.Cos(Mathf.Deg2Rad * m_moveRotate),
                m_moveRadiuse * Mathf.Sin(Mathf.Deg2Rad * AdjustParameter.Camera_Constant.AROUND_ANGLE),
                m_moveRadiuse * Mathf.Sin(Mathf.Deg2Rad * m_moveRotate));
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
                transform.position = new Vector3(
                    m_moveRadiuse * Mathf.Cos(Mathf.Deg2Rad * m_moveRotate),
                    m_moveRadiuse * Mathf.Sin(Mathf.Deg2Rad * AdjustParameter.Camera_Constant.AROUND_ANGLE),
                    m_moveRadiuse * Mathf.Sin(Mathf.Deg2Rad * m_moveRotate));
            }
        }
    }
    // ドーム状にカメラが移動
    void CameraDome()
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
            Vector3 defPos = transform.position;

            if (Mathf.Abs(difference.x) > Mathf.Abs(difference.y) && Mathf.Abs(difference.x) > AdjustParameter.Camera_Constant.PERMISSION_MOVE)
            {
                transform.RotateAround(m_target, new Vector3(0, 1, 0),
                    difference.x * Time.deltaTime * AdjustParameter.Camera_Constant.AROUND_MOVE);
                m_savePos = Input.mousePosition;
            }
            else if (Mathf.Abs(difference.y) > Mathf.Abs(difference.x) && Mathf.Abs(difference.y) > AdjustParameter.Camera_Constant.PERMISSION_MOVE)
            {
                transform.RotateAround(m_target, transform.right,
                    -difference.y * Time.deltaTime * AdjustParameter.Camera_Constant.AROUND_MOVE);
                m_savePos = Input.mousePosition;
            }

            // 範囲処理
            if (transform.position.y < m_cameraHeight)
            {
                transform.position = new Vector3(defPos.x, m_cameraHeight, defPos.z);
                transform.LookAt(Vector3.zero);
            }
            else if (transform.position.y > Vector3.Magnitude(m_storePos) -1.0f)
            {
                transform.position = new Vector3(defPos.x, defPos.y, defPos.z);
                transform.LookAt(Vector3.zero);
            }
        }
        // カメラ手前移動
        else if (scroll != 0.0f)
        {
            Vector3 _pos = transform.position + transform.forward *
                scroll * AdjustParameter.Camera_Constant.VALUE_SCROLL;

            float dis = Vector3.Distance(_pos, m_target);
            if (dis > AdjustParameter.Camera_Constant.CAMERA_NEAR &&
                dis < AdjustParameter.Camera_Constant.CAMERA_FAR)
            {
                transform.position = _pos;
            }
        }
    }
    // フリック移動
    void CameraSwipeMove()
    {
        if (m_type != CameraType.SwipeMove) 
            return;

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
            transform.DOMove(m_target, AdjustParameter.Camera_Constant.SWIPE_DERAY);
        }
        m_targetOld = m_target;

        Vector3 pos = new Vector3(
            Mathf.Clamp(transform.position.x, m_moveMin.x, m_moveMax.x), transform.position.y,
            Mathf.Clamp(transform.position.z, m_moveMin.z, m_moveMax.z));

        // 範囲外
        if (transform.position != pos)
        {
            Vector3 bound = (m_target - pos) * AdjustParameter.Camera_Constant.SWIPE_OUT;
            transform.DOPause();
            transform.DOMove(pos + bound, AdjustParameter.Camera_Constant.SWIPE_DERAY);
        }
    }

    // ズームインの動き
    void CameraZoomIn()
    {
        if(transform.position.z == m_target.z)
        {
            transform.position = m_target;
            transform.DOPause();
            m_cameraState = null;
        }
    }
    // ズームアウトの動き
    void CameraZoomOut()
    {
        if(transform.position.z == m_target.z)
        {
            transform.position = m_target;
            m_type = m_defType;
            transform.DOPause();
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
        transform.position = m_target + FADE_UP;
        transform.LookAt(m_target);

        transform.DOLocalMove(m_target + FADE_INTO,
            AdjustParameter.Camera_Constant.FADE_DURATION);
   }
}
