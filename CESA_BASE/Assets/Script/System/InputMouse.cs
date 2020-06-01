using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMouse : MonoBehaviour
{
    private static Camera m_main;
    private static Camera m_sub;

    public enum Mouse_Place
    {
        Left,
        Right,
        Center,
    }


    void Start()
    {
        RoadCamera();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Camera screenCamera = GetScreenCamera();
            Vector3 screen = Input.mousePosition + screenCamera.transform.rotation * screenCamera.transform.forward;

            screen.z = AdjustParameter.Camera_Constant.EFFECT_POS_Z;

            Vector3 mousePos = screenCamera.ScreenToWorldPoint(screen);
            Effekseer.EffekseerEmitter effect = EffectManager.Instance.EffectCreate(Effekseer.EffekseerEmitter.EffectType.Click,
                mousePos, Quaternion.identity, m_main.transform);
            effect.transform.localScale *= screenCamera.orthographicSize;
        }
    }

    public static void RoadCamera()
    {
        m_main = Camera.main;
        GameObject obj = GameObject.FindGameObjectWithTag(NameDefine.TagName.SubCamera);
        if (obj)
        {
            m_sub = obj.GetComponent<Camera>();
        }
    }

    /// <summary>
    /// マウスの位置
    /// </summary>
    /// <returns>TRUE:UIの部分　FALSE：ゲーム部分</returns>
    public static bool MouseEria()
    {
        return Input.mousePosition.x > Screen.width * Camera.main.rect.width;
    } 

    public static Camera GetScreenCamera()
    {
        if (MouseEria())
        {
            // サブカメラがないなら
            if(!m_sub)
                return m_main;
            else
                return m_sub;
        }
        else
        {
            return m_main;
        }
    } 
}
