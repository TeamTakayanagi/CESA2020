using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputStageSize : MonoBehaviour
{
    private InputField m_inputField = null;
    private Text m_text;
    private char m_endLetter;

    // Start is called before the first frame update
    void Awake()
    {
        m_inputField = GetComponent<InputField>();
        m_text = transform.GetChild(1).GetComponent<Text>();
        m_endLetter = transform.name[transform.name.Length - 1];
        m_inputField.text = StageEditerMgr.Instance.GetStageSize(m_endLetter).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_inputField.text.Length == 0)
        {
            m_inputField.text = "0";
        }
    }

    public void InputText()
    {
        if (m_text.text.Length == 0)
            return;

        m_inputField.text = m_text.text;
        StageEditerMgr.Instance.SetStageSize(m_endLetter, int.Parse(m_inputField.text));
    }

    public void CountUp()
    {
        int amount = StageEditerMgr.Instance.GetStageSize(m_endLetter) + 1;
        StageEditerMgr.Instance.SetStageSize(m_endLetter, amount);
        m_inputField.text = amount.ToString();
    }
    public void CountDown()
    {
        int amount = StageEditerMgr.Instance.GetStageSize(m_endLetter);
        if (amount > 1)
        {
            amount--;
            StageEditerMgr.Instance.SetStageSize(m_endLetter, amount);
        }
        m_inputField.text = amount.ToString();
    }
}
