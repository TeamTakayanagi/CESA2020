using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PushButton : MonoBehaviour
{
    [SerializeField]
    private float m_speed = 0.5f;

    private Text m_text = null;
    private Image m_image = null;

    private float m_time = 0; 
    private bool m_flg = false;
    public bool Flg
    {
        set
        {
            m_flg = value;
        }
    }

    private enum ObjType
    {
        TEXT,
        IMAGE
    };
    private ObjType thisObjType = ObjType.TEXT;


    // Start is called before the first frame update
    void Start()
    {
        //アタッチしてるオブジェクトを判別
        if (gameObject.GetComponent<Image>())
        {
            thisObjType = ObjType.IMAGE;
            m_image = this.gameObject.GetComponent<Image>();
        }
        else if (gameObject.GetComponent<Text>())
        {
            thisObjType = ObjType.TEXT;
            m_text = this.gameObject.GetComponent<Text>();
        }

        m_flg = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_flg)
        {
            //オブジェクトのAlpha値を更新
            if (thisObjType == ObjType.IMAGE)
            {
                m_image.color = GetAlphaColor(m_image.color);
            }
            else if (thisObjType == ObjType.TEXT)
            {
                m_text.color = GetAlphaColor(m_text.color);
            }
        }
    }

    //Alpha値を更新してColorを返す
    private Color GetAlphaColor(Color color)
    {
        m_time += Time.deltaTime * 5.0f * m_speed;
        color.a = Mathf.Sin(m_time) * 0.5f + 0.5f;

        if (color.a <= Mathf.Clamp(color.a, 0.001f, 1.0f))
        {
            color.a = Mathf.Clamp(color.a, 0.001f, 1.0f);
        }

        return color;
    }
}
