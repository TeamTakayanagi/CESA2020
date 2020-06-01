using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class inputFieldInt : MonoBehaviour
{
    public enum FieldType
    {
        stageNum = 0,
        stageSizeX,
        stageSizeY,
        stageSizeZ,
        createRotX,
        createRotY,
        createRotZ,
    }

    [SerializeField]
    private FieldType m_type = FieldType.stageNum;
    [SerializeField]
    private int m_number = 0;
    [SerializeField]
    private int m_addAmount = 1;
    [SerializeField]
    private int m_max = 1;
    [SerializeField]
    private int m_min = 0;
    [SerializeField]
    private UnityEvent m_event = null;

    private InputField m_inputField = null;
    private Text m_text;


    // Start is called before the first frame update
    void Awake()
    {
        // 大小逆なら入れ替え
        if(m_min > m_max)
        {
            int store = m_min;
            m_min = m_max;
            m_max = store;
        }

        m_inputField = GetComponent<InputField>();
        m_text = transform.GetChild(1).GetComponent<Text>();
        m_inputField.text = m_number.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_inputField.text.Length == 0)
        {
            m_inputField.text = m_min.ToString();
            m_event.Invoke();
        }
    }

    public void InputText()
    {
        if (m_text.text.Length == 0)
            return;

        m_inputField.text = m_text.text;
        m_number = int.Parse(m_inputField.text);

        // 範囲処理
        if (m_number > m_max + 1)
            m_number = m_max;
        else if (m_number < m_min)
            m_number = m_min;
        m_event.Invoke();
    }

    public void CountUp()
    {
        int difference = Mathf.Abs(m_max - m_min + m_addAmount);
        m_number = (m_number + m_addAmount - m_min + difference) % difference + m_min;
        m_inputField.text = m_number.ToString();
        m_event.Invoke();
    }

    public void CountDown()
    {
        int difference = Mathf.Abs(m_max - m_min + m_addAmount);
        m_number = (m_number - m_addAmount - m_min + difference) % difference + m_min;
        m_inputField.text = m_number.ToString();
        m_event.Invoke();
    }

    public static int GetInputFieldInt(FieldType type)
    {
        inputFieldInt[] _list = FindObjectsOfType<inputFieldInt>();
        foreach(inputFieldInt _obj in _list)
        {
            if (_obj.m_type == type)
                return _obj.m_number;
        }
        return ProcessedtParameter.System_Constant.ERROR_INT;
    } 
    public static void SetInputFieldInt(FieldType type, int value)
    {
        inputFieldInt[] _list = FindObjectsOfType<inputFieldInt>();
        foreach (inputFieldInt _obj in _list)
        {
            if (_obj.m_type != type)
                continue;

            _obj.m_number = value;
            _obj.m_inputField.text = _obj.m_number.ToString();
        }
    }
}
