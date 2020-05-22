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

    public static void RoadCamera()
    {
        m_main = Camera.main;
        m_sub = GameObject.FindGameObjectWithTag(NameDefine.TagName.SubCamera).GetComponent<Camera>();
    }

    public static bool MouseClick(Mouse_Place place)
    {
        if (Input.GetMouseButton((int)place))
        {
            //Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition + Camera.main.transform.forward);
            //EffectManager.Instance.EffectCreate(Effekseer.EffekseerEmitter.EffectType.Click, mousePos, Quaternion.identity);
            return true;
        }

        return false;
    }
    public static bool MouseClickDown(Mouse_Place place)
    {
        if (Input.GetMouseButtonDown((int)place))
        {
            if (place == Mouse_Place.Left)
            {
                Camera screenCamera = GetScreenCamera();
                Vector3 screen = Input.mousePosition + screenCamera.transform.rotation * screenCamera.transform.forward;

                screen.z = AdjustParameter.Camera_Constant.EFFECT_POS_Z;

                Vector3 mousePos = screenCamera.ScreenToWorldPoint(screen);
                Effekseer.EffekseerEmitter effect = EffectManager.Instance.EffectCreate(Effekseer.EffekseerEmitter.EffectType.Click, mousePos, Quaternion.identity);
                effect.transform.localScale *= screenCamera.orthographicSize;
            }
            return true;
        }

        return false;
    }
    public static bool MouseClickUp(Mouse_Place place)
    {
        return Input.GetMouseButtonUp((int)place);
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
            return m_sub;
        }
        else
        {
            return m_main;
        }
    } 
}
