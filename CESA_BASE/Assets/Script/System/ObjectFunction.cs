using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFunction : MonoBehaviour
{
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
    }

    [System.Serializable]
    public class Function
    {
        public FunctionType _functionType;  // 機能タイプ
        public SinType _sinType;            // sin値タイプ
        public bool _isLooping;             // ループするかどうか  
        public float _deleyTime;            // 遅延

        public Vector3 _startAngle;        // 初期値
        public Vector3 _speed;             // 速度
        public Vector3 _amp;               // 振幅

        [System.NonSerialized]
        public Vector3 _angleValue;        // 移動量
        [System.NonSerialized]
        public bool _stop;                 // 停止
    }

    [SerializeField]
    private List<Function> m_function = new List<Function>();   // 機能のリスト
    private Vector3 m_standardValue = Vector3.zero;         // 基準となる初期座標

    public bool Stop
    {
        set
        {
            for(int i = 0, size = m_function.Count; i < size; ++i)
            {
                m_function[i]._stop = !value;
            }
        }
    }
    
    private void Awake()
    {
        // Function
        for (int _typeNum = 0; _typeNum < m_function.Count; _typeNum++)
        {
            m_function[_typeNum]._angleValue = Vector3.zero;            // 移動量初期化

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

            m_standardValue += new Vector3(Mathf.Sin(Mathf.Deg2Rad * m_function[_typeNum]._startAngle.x) * m_function[_typeNum]._amp.x,
                                           Mathf.Sin(Mathf.Deg2Rad * m_function[_typeNum]._startAngle.y) * m_function[_typeNum]._amp.y,
                                           Mathf.Sin(Mathf.Deg2Rad * m_function[_typeNum]._startAngle.z) * m_function[_typeNum]._amp.z);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
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

            m_function[_typeNum]._angleValue += m_function[_typeNum]._speed * Time.deltaTime * 60.0f;

            if (m_function[_typeNum]._isLooping)
            {
                if (m_function[_typeNum]._angleValue.x >= 360.0f)
                    m_function[_typeNum]._angleValue.x -= 360.0f;
                if (m_function[_typeNum]._angleValue.y >= 360.0f)
                    m_function[_typeNum]._angleValue.y -= 360.0f;
                if (m_function[_typeNum]._angleValue.z >= 360.0f)
                    m_function[_typeNum]._angleValue.z -= 360.0f;
            }
            else
            {
                if (m_function[_typeNum]._sinType == SinType.PlusOnly || m_function[_typeNum]._sinType == SinType.MinusOnly)
                {
                    if (m_function[_typeNum]._angleValue.x >= 180.0f)
                        m_function[_typeNum]._angleValue.x = 180.0f;
                    if (m_function[_typeNum]._angleValue.y >= 180.0f)
                        m_function[_typeNum]._angleValue.y = 180.0f;
                    if (m_function[_typeNum]._angleValue.z >= 180.0f)
                        m_function[_typeNum]._angleValue.z = 180.0f;
                }
                else
                {
                    if (m_function[_typeNum]._angleValue.x >= 360.0f)
                        m_function[_typeNum]._angleValue.x = 360.0f;
                    if (m_function[_typeNum]._angleValue.y >= 360.0f)
                        m_function[_typeNum]._angleValue.y = 360.0f;
                    if (m_function[_typeNum]._angleValue.z >= 360.0f)
                        m_function[_typeNum]._angleValue.z = 360.0f;
                }
            }

            Vector3 value = new Vector3(Mathf.Sin(Mathf.Deg2Rad * m_function[_typeNum]._angleValue.x) * m_function[_typeNum]._amp.x,
                                        Mathf.Sin(Mathf.Deg2Rad * m_function[_typeNum]._angleValue.y) * m_function[_typeNum]._amp.y,
                                        Mathf.Sin(Mathf.Deg2Rad * m_function[_typeNum]._angleValue.z) * m_function[_typeNum]._amp.z);

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
