using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputCreateRot : MonoBehaviour
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
        m_inputField.text = StageEditerMgr.Instance.GetCreateRot(m_endLetter).ToString();
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
        StageEditerMgr.Instance.SetCreateRot(m_endLetter, int.Parse(m_inputField.text));
    }

    public void CountUp()
    {
        int amount = StageEditerMgr.Instance.GetCreateRot(m_endLetter) + 90;
        amount = (amount + 360) % 360;
        StageEditerMgr.Instance.SetCreateRot(m_endLetter, amount);
        m_inputField.text = amount.ToString();
    }
    public void CountDown()
    {
        int amount = StageEditerMgr.Instance.GetCreateRot(m_endLetter) - 90;
        amount = (amount + 360) % 360;
        StageEditerMgr.Instance.SetCreateRot(m_endLetter, amount);
        m_inputField.text = amount.ToString();
    }
}
