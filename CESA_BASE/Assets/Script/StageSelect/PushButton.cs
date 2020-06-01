using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PushButton : MonoBehaviour
{
    //public
    public float Speed;
    public static float stSpeed;

    //private
    private Text text;
    private Image image;
    private float time;

    private Color m_oldColor;
    private bool m_noneAlphaFlg = false;   // true : 透過 / false : 不透明
    public bool NoneAalpha
    {
        get
        {
            return m_noneAlphaFlg;
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
        stSpeed = Speed;

        //アタッチしてるオブジェクトを判別
        if (gameObject.GetComponent<Image>())
        {
            thisObjType = ObjType.IMAGE;
            image = this.gameObject.GetComponent<Image>();
        }
        else if (gameObject.GetComponent<Text>())
        {
            thisObjType = ObjType.TEXT;
            text = this.gameObject.GetComponent<Text>();
        }

    }

    // Update is called once per frame
    void Update()
    {
        //オブジェクトのAlpha値を更新
        if (thisObjType == ObjType.IMAGE)
        {
            image.color = GetAlphaColor(image.color);
        }
        else if (thisObjType == ObjType.TEXT)
        {
            text.color = GetAlphaColor(text.color);
        }
    }

    //Alpha値を更新してColorを返す
    Color GetAlphaColor(Color color)
    {
        time += Time.deltaTime * 5.0f * stSpeed;
        color.a = Mathf.Sin(time) * 0.5f + 0.5f;

        if (color.a <= Mathf.Clamp(color.a, 0.0001f, 1.0f))
        {
            color.a = Mathf.Clamp(color.a, 0.0001f, 1.0f);

            m_noneAlphaFlg = true;
        }
        else
        {
            m_noneAlphaFlg = false;
        }
        m_oldColor = color;

        return color;
    }

    public static void setFlashSpead(float setSpead)
    {
        stSpeed = setSpead;
    }
}
