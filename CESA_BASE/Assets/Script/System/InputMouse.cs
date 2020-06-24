using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMouse : MonoBehaviour
{
    [SerializeField]
    private Texture2D m_cursorDefaultTemp = null;                           // マウスカーソル（通常時）
    [SerializeField]
    private Texture2D m_cursorCatchTemp = null;                             // マウスカーソル（UIの導火線選択時）
    private static Texture2D m_cursorDefault = null;                           // マウスカーソル（通常時）
    private static Texture2D m_cursorCatch = null;                             // マウスカーソル（UIの導火線選択時）

    private static readonly Vector2 CURSOR_POS = new Vector2(142.0f, 25.0f);  // マウスカーソルの位置

    private static Camera m_main;
    private static Camera m_sub;

    public enum Mouse_Place
    {
        Left,
        Right,
        Center,
    }
    public enum Mouse_Cursol
    {
        Default,
        Catch
    }


    void Start()
    {
        m_cursorDefault = m_cursorDefaultTemp;
        m_cursorCatch = m_cursorCatchTemp;
        // マウスカーソル用の画像を変更
        Cursor.SetCursor(m_cursorDefault, CURSOR_POS, CursorMode.Auto);
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
            m_sub = obj.GetComponent<Camera>();
    }

    /// <summary>
    /// マウスの位置
    /// </summary>
    /// <returns>TRUE:UIの部分　FALSE：ゲーム部分</returns>
    public static bool MouseEria()
    {
        return Input.mousePosition.x > Screen.width * Camera.main.rect.width;
    } 
    public static void ChangeCursol(Mouse_Cursol cursol)
    {
        // マウスカーソル用の画像を変更
        if (cursol == Mouse_Cursol.Default)
            Cursor.SetCursor(m_cursorDefault, CURSOR_POS, CursorMode.Auto);
        else
            Cursor.SetCursor(m_cursorCatch, CURSOR_POS, CursorMode.Auto);
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
