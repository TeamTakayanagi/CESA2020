using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiFunction : MonoBehaviour
{
    public enum AlphaType
    {
        None,
        FadeIn,
        FadeOut,
        Flashing,
    }

    public enum FunctionType
    {
        Scaling,        // 拡縮
        Shaking,        // 揺れ
    }

    public enum SinType
    {
        Normal,     // 通常
        PlusOnly,   // +のみ
        MinusOnly,  // -のみ
        None,       // サインなし
    }

    [System.Serializable]
    public class AlphaChange
    {
        public AlphaType _alphaType;    // 機能タイプ
        public float _min = 1;          // 最小
        public float _max = 1;          // 最大
        public float _time;             // フェード時間

        [System.NonSerialized]
        public float _alphaValue;       // α値変化量
        [System.NonSerialized]
        public float _speed;            // 速度
    }

    [System.Serializable]
    public class Function
    {
        public FunctionType _functionType;  // 機能タイプ
        public SinType _sinType;            // sin値タイプ
        public bool _isLooping;             // ループするかどうか  
        public bool _isDestroy;             // ループしない場合、削除されるか  
        public float _deleyTime;            // 遅延
        public float _intervalTime;         // 間隔

        public Vector3 _startAngle;        // 初期値
        public Vector3 _speed;             // 速度
        public Vector3 _amp;               // 振幅

        [System.NonSerialized]
        public Vector3 _angleValue;        // 移動量
        [System.NonSerialized]
        public bool _stop = false;         // 停止
        [System.NonSerialized]
        public float _interval;
    }

    public bool Stop
    {
        set
        {
            for (int i = 0, size = m_function.Count; i < size; ++i)
            {
                m_function[i]._stop = value;
            }
        }
    }
    [SerializeField]
    private AlphaChange m_alphaChange = null;   // α値変化

    [SerializeField]
    private List<Function> m_function = new List<Function>();   // 機能のリスト

    private CanvasRenderer m_canvasRenderer = null;
    private Vector3 m_standardValue = Vector3.zero;         // 基準となる初期座標

    private void Awake()
    {
        // Alpha
        m_canvasRenderer = gameObject.GetComponent<CanvasRenderer>();
        m_alphaChange._speed = Time.deltaTime / m_alphaChange._time;

        switch (m_alphaChange._alphaType)
        {
            case AlphaType.FadeIn:
                m_alphaChange._alphaValue = m_alphaChange._min;
                break;
            case AlphaType.FadeOut:
                m_alphaChange._alphaValue = m_alphaChange._max;
                break;
            case AlphaType.Flashing:
                m_alphaChange._alphaValue = m_alphaChange._min;
                break;
            case AlphaType.None:
                m_alphaChange._alphaValue = 1.0f;
                break;
            default:
                break;
        }


        // Function
        for (int _typeNum = 0; _typeNum < m_function.Count; _typeNum++)
        {
            m_function[_typeNum]._angleValue = Vector3.zero;            // 移動量初期化
            m_function[_typeNum]._interval = 0.0f;

            // Typeごとの初期処理
            switch (m_function[_typeNum]._functionType)
            {
                case FunctionType.Scaling:
                    m_standardValue = transform.localScale;
                    break;
                case FunctionType.Shaking:
                    m_standardValue = transform.localPosition;
                    break;

                default:
                    break;
            }

            if (m_function[_typeNum]._sinType != SinType.None)
            {
                m_standardValue += new Vector3(Mathf.Sin(Mathf.Deg2Rad * m_function[_typeNum]._startAngle.x) * m_function[_typeNum]._amp.x,
                                               Mathf.Sin(Mathf.Deg2Rad * m_function[_typeNum]._startAngle.y) * m_function[_typeNum]._amp.y,
                                               Mathf.Sin(Mathf.Deg2Rad * m_function[_typeNum]._startAngle.z) * m_function[_typeNum]._amp.z);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Alpha
        switch (m_alphaChange._alphaType)
        {
            case AlphaType.FadeIn:
                m_alphaChange._alphaValue += m_alphaChange._speed;
                if (m_alphaChange._alphaValue > m_alphaChange._max)
                    m_alphaChange._alphaType = AlphaType.None;
                break;
            case AlphaType.FadeOut:
                m_alphaChange._alphaValue -= m_alphaChange._speed;
                if (m_alphaChange._alphaValue < m_alphaChange._min)
                    m_alphaChange._alphaType = AlphaType.None;
                break;
            case AlphaType.Flashing:
                m_alphaChange._alphaValue += m_alphaChange._speed;
                if (m_alphaChange._alphaValue >= m_alphaChange._max || m_alphaChange._alphaValue <= m_alphaChange._min)
                    m_alphaChange._speed *= -1;
                break;
            case AlphaType.None:
                break;
            default:
                break;
        }
        m_canvasRenderer.SetAlpha(m_alphaChange._alphaValue);

        // Function
        for (int _typeNum = 0; _typeNum < m_function.Count; _typeNum++)
        {
            if (m_function[_typeNum]._stop)
                continue;

            if (m_function[_typeNum]._deleyTime > 0.0f)
            {
                m_function[_typeNum]._deleyTime -= 1 * Time.deltaTime;
                continue;
            }
            if (m_function[_typeNum]._interval > 0.0f)
            {
                m_function[_typeNum]._interval -= 1 * Time.deltaTime;
                continue;
            }

            m_function[_typeNum]._angleValue += m_function[_typeNum]._speed * Time.deltaTime * 60.0f;

            if (m_function[_typeNum]._isLooping)
            {
                if (m_function[_typeNum]._sinType == SinType.PlusOnly || m_function[_typeNum]._sinType == SinType.MinusOnly)
                {
                    if (m_function[_typeNum]._angleValue.x >= 180.0f)
                    {
                        m_function[_typeNum]._angleValue.x -= 180.0f;
                        m_function[_typeNum]._interval = m_function[_typeNum]._intervalTime;
                    }
                    if (m_function[_typeNum]._angleValue.y >= 180.0f)
                    {
                        m_function[_typeNum]._angleValue.y -= 180.0f;
                        m_function[_typeNum]._interval = m_function[_typeNum]._intervalTime;
                    }
                }
                else
                {
                    if (m_function[_typeNum]._angleValue.x >= 360.0f)
                    {
                        m_function[_typeNum]._angleValue.x -= 360.0f;
                        m_function[_typeNum]._interval = m_function[_typeNum]._intervalTime;
                    }
                    if (m_function[_typeNum]._angleValue.y >= 360.0f)
                    {
                        m_function[_typeNum]._angleValue.y -= 360.0f;
                        m_function[_typeNum]._interval = m_function[_typeNum]._intervalTime;
                    }
                }
            }
            else
            {
                //if (m_function[_typeNum]._isDestroy)
                //{
                //    DestroyImmediate(gameObject);
                //    return;
                //}
                if (m_function[_typeNum]._sinType == SinType.PlusOnly || m_function[_typeNum]._sinType == SinType.MinusOnly)
                {
                    if (m_function[_typeNum]._angleValue.x >= 180.0f)
                    {
                        m_function[_typeNum]._angleValue.x = 180.0f;
                        if (m_function[_typeNum]._isDestroy)
                        {
                            DestroyImmediate(gameObject);
                            return;
                        }
                    }
                    if (m_function[_typeNum]._angleValue.y >= 180.0f)
                    {
                        m_function[_typeNum]._angleValue.y = 180.0f;
                        if (m_function[_typeNum]._isDestroy)
                        {
                            DestroyImmediate(gameObject);
                            return;
                        }
                    }
                    //if (m_function[_typeNum]._angleValue.z >= 180.0f)
                    //    m_function[_typeNum]._angleValue.z = 180.0f;

                }
                else if (m_function[_typeNum]._sinType == SinType.Normal)
                {
                    if (m_function[_typeNum]._angleValue.x >= 360.0f)
                    {
                        m_function[_typeNum]._angleValue.x = 360.0f;
                        if (m_function[_typeNum]._isDestroy)
                        {
                            DestroyImmediate(gameObject);
                            return;
                        }
                    }
                    if (m_function[_typeNum]._angleValue.y >= 360.0f)
                    {
                        m_function[_typeNum]._angleValue.y = 360.0f;
                        if (m_function[_typeNum]._isDestroy)
                        {
                            DestroyImmediate(gameObject);
                            return;
                        }
                    }
                    //if (m_function[_typeNum]._angleValue.z >= 360.0f)
                    //    m_function[_typeNum]._angleValue.z = 360.0f;
                }
                else
                {
                    if (m_function[_typeNum]._amp.x != 0 && m_function[_typeNum]._angleValue.x >= m_function[_typeNum]._amp.x)
                    {
                        m_function[_typeNum]._angleValue.x = m_function[_typeNum]._amp.x;
                        if (m_function[_typeNum]._isDestroy)
                        {
                            DestroyImmediate(gameObject);
                            return;
                        }
                    }
                    if (m_function[_typeNum]._amp.y != 0 && m_function[_typeNum]._angleValue.y >= m_function[_typeNum]._amp.y)
                    {
                        m_function[_typeNum]._angleValue.y = m_function[_typeNum]._amp.y;
                        if (m_function[_typeNum]._isDestroy)
                        {
                            DestroyImmediate(gameObject);
                            return;
                        }
                    }
                    //if (m_function[_typeNum]._angleValue.z >= m_function[_typeNum]._amp.z)
                    //{
                    //    m_function[_typeNum]._angleValue.z = m_function[_typeNum]._amp.z;
                    //    if (m_function[_typeNum]._isDestroy)
                    //    {
                    //        DestroyImmediate(gameObject);
                    //        return;
                    //    }
                    //}
                }
            }

            Vector3 value = Vector3.zero;

            if (m_function[_typeNum]._sinType == SinType.None)
            {
                value = m_function[_typeNum]._angleValue;
            }
            else
            {
                value = new Vector3(Mathf.Sin(Mathf.Deg2Rad * m_function[_typeNum]._angleValue.x) * m_function[_typeNum]._amp.x,
                                            Mathf.Sin(Mathf.Deg2Rad * m_function[_typeNum]._angleValue.y) * m_function[_typeNum]._amp.y,
                                            Mathf.Sin(Mathf.Deg2Rad * m_function[_typeNum]._angleValue.z) * m_function[_typeNum]._amp.z);
            }

            if (m_function[_typeNum]._sinType == SinType.PlusOnly)
            {
                if (value.x < 0.0f)
                    value.x *= -1;
                if (value.y < 0.0f)
                    value.y *= -1;
                if (value.z < 0.0f)
                    value.z *= -1;
            }
            if (m_function[_typeNum]._sinType == SinType.MinusOnly)
            {
                if (value.x > 0.0f)
                    value.x *= -1;
                if (value.y > 0.0f)
                    value.y *= -1;
                if (value.z > 0.0f)
                    value.z *= -1;
            }

            switch (m_function[_typeNum]._functionType)
            {
                case FunctionType.Scaling:
                    transform.localScale = m_standardValue + value;
                    break;

                case FunctionType.Shaking:
                    transform.localPosition = m_standardValue + value;
                    break;

                default:
                    break;
            }
        }
    }
}
