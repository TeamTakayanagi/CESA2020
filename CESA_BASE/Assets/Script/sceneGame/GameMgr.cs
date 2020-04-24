using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameMgr : SingletonMonoBehaviour<GameMgr>
{
    // デリゲート宣言
    delegate void GameStep();

    private enum ResultPlacement
    {
        Text = 0,
        Button,
        //RePlay,
        //Bask
    }

    [SerializeField]
    private Texture2D m_cursorDefault = null;                           // マウスカーソル（通常時）
    [SerializeField]
    private Texture2D m_cursorCatch = null;                             // マウスカーソル（UIの導火線選択時）
    [SerializeField]
    private Vector3Int m_stageSize = Vector3Int.zero;                   // ステージサイズ
    [SerializeField]
    private GameObject m_fireworks = null;                              // 花火のプレハブ

    // 定数
    private Vector3 END_FIRE_POS = new Vector3(0.0f, 30.0f, 0.0f);      // 花火の終着地点との距離      
    private readonly Vector3 TEXT_POS = new Vector3(0.0f, 100, 0.0f);   // リザルトテキストの移動距離
    private readonly Vector3 BUTTON_POS = new Vector3(0.0f, -50, 0.0f); // リザルトボタンの移動距離              
    private readonly Vector3 OUTPOS = new Vector3(-50, -50, -50);       // 導火線を生成できない位置
    private readonly AnimationCurve m_animCurve = AnimationCurve.Linear(0, 0, 1, 1);   // リザルトUIの移動用 

    private int m_burnCount = 1;                                        // 燃えている導火線の数
    private int m_gameSpeed = 1;                                        // ゲーム加速処理
    private Vector3 m_createPos = Vector3.zero;                         // 導火線の生成位置
    private GameObject m_saveObj = null;                                // 各GameStepごとにオブジェクトを格納（スタート：カウントダウン数字　ゲームクリア：花火）
    private GameObject m_resultClear = null;                            // ゲームクリア用のUIの親オブジェクト
    private GameObject m_resultGameover = null;                         // ゲームオーバー用のUIの親オブジェクト
    private Fuse m_selectFuse = null;                                   // 選択しているUIの導火線   
    private UIFuseCreate m_UIFuseCreate = null;                         // UIの導火線生成オブジェクト
    private LinkedList<Fuse> m_fieldFuse = new LinkedList<Fuse>();      // ゲーム画面の導火線
    private LinkedList<Fuse> m_uiFuse = new LinkedList<Fuse>();         // UI部分の導火線
    private GameStep m_gameStep = null;                                 // 現在のゲームの進行状況の関数

    public int BurnCount
    {
        get
        {
            return m_burnCount;
        }
        set
        {
            m_burnCount = value;
        }
    }
    public int GameSpeed
    {
        get
        {
            return m_gameSpeed;
        }
    }
    public Fuse UIFuse
    {
        set
        {
            m_uiFuse.AddLast(value);
        }
    }
    public int UIFuseCount
    {
        get
        {
            return m_uiFuse.Count;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        // マウスカーソル用の画像をデフォルトに変更
        Cursor.SetCursor(m_cursorDefault, Vector2.zero, CursorMode.Auto);
        // ゲームクリア用のUIの親オブジェクト取得
        m_resultClear = GameObject.FindGameObjectWithTag(StringDefine.TagName.UIGameClear);
        // ゲームオーバー用のUIの親オブジェクト取得
        m_resultGameover = GameObject.FindGameObjectWithTag(StringDefine.TagName.UIGameOver);
        // UIの導火線生成オブジェクト取得
        m_UIFuseCreate = FindObjectOfType<UIFuseCreate>();
        // 初期生成位置はわからないので生成不可能場所を格納
        m_createPos = OUTPOS;
        // 地形生成オブジェクト取得
        TerrainCreate terrainCreate = FindObjectOfType<TerrainCreate>();
        terrainCreate.CreateGround(m_stageSize.x, m_stageSize.z, -m_stageSize.y / 2 - 1);

        // 開始演出準備
        GameObject canvas = GameObject.FindGameObjectWithTag(StringDefine.TagName.UICanvas);
        m_saveObj = canvas.transform.GetChild(0).gameObject;
        m_gameStep = GameStart;

        // フィールドオブジェクトの取得
        Fuse[] _fuseList = FindObjectsOfType<Fuse>();
        foreach (Fuse _fuse in _fuseList)
        {
            if (_fuse.Type == Fuse.FuseType.UI)
                m_uiFuse.AddLast(_fuse);
            else
                m_fieldFuse.AddLast(_fuse);

            // スタート演出のため導火線の更新処理停止（ステージエディタ完成後修正予定）
            _fuse.enabled = false;
        }
        m_UIFuseCreate.enabled = false;
        Camera.main.GetComponent<MainCamera>().Control = true;
        //Sound.Instance.PlayBGM(ConstDefine.Audio.BGM.GameMain);
    }

    // Update is called once per frame
    void Update()
    {
        m_gameStep();
    }

    /// <summary>
    /// ゲームスタート処理
    /// </summary>
    void GameStart()
    {
        Number number = m_saveObj.GetComponent<Number>();
        if (number.TexCount < 0)
        {
            m_gameStep = GameMain;
            Destroy(m_saveObj);

            // 導火線の更新処理再開
            Fuse[] _fuseList = FindObjectsOfType<Fuse>();
            foreach (Fuse _fuse in _fuseList)
            {
                _fuse.enabled = true;
            }
            m_UIFuseCreate.enabled = true;
        }
    }

    /// <summary>
    /// ゲームメイン処理
    /// </summary>
    void GameMain()
    {
#if UNITY_EDITOR
        if(Input.GetKeyUp(KeyCode.C))
        {
            m_resultClear.SetActive(true);
            Camera.main.GetComponent<MainCamera>().Control = false;
            m_saveObj = Instantiate(m_fireworks, Vector3.zero, Quaternion.identity);
            m_gameStep = GameClear;
            return;
        }
        else if(Input.GetKeyUp(KeyCode.O))
        {
            m_gameStep = GameOver;
            return;
        }
#endif

        // マウス座標をワールド座標で取得
        Vector3 mousePos = Vector3.zero;
        Vector3 screen = Camera.main.WorldToScreenPoint(transform.position);
        mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screen.z);
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        // 生成場所を取得
        m_createPos = FindNearFuse(mousePos);

        // 導火線を選択しているなら
        if (m_selectFuse)
        {
            // UI画面
            if (Input.mousePosition.x > Screen.width * 0.8f)
                m_selectFuse.transform.position = m_selectFuse.DefaultPos;
            // ゲーム画面
            else
            {
                m_selectFuse.transform.position = m_createPos;
                m_selectFuse.transform.localEulerAngles = m_selectFuse.DefaultRot;
            }

            if (m_selectFuse.transform.position == OUTPOS)
                return;
        }

        // 設置or選択
        if (Input.GetMouseButtonDown(0))
        {
            // UI画面
            if (Input.mousePosition.x > Screen.width * 0.8f)
            {
                // サブカメラ取得
                RaycastHit hit = new RaycastHit();
                Ray ray = GameObject.FindGameObjectWithTag(StringDefine.TagName.SubCamera).GetComponent<Camera>().
                    ScreenPointToRay(Input.mousePosition);

                // 導火線を選択
                if (Physics.Raycast(ray, out hit))
                {
                    // 新規選択
                    if (!m_selectFuse || m_selectFuse.gameObject != hit.collider.transform.parent.gameObject)
                    {
                        Fuse _fuse = hit.collider.transform.parent.GetComponent<Fuse>();
                        if (!_fuse || _fuse.EndPos != Vector3.zero)
                            return;
                        if (_fuse.Type == Fuse.FuseType.UI)
                        {
                            if(m_selectFuse)
                                m_selectFuse.SelectUIFuse(false);
                            m_selectFuse = _fuse;
                            m_selectFuse.SelectUIFuse(true);
                            // マウスカーソル用の画像を選択時に変更
                            Cursor.SetCursor(m_cursorCatch, Vector2.zero, CursorMode.Auto);
                        }
                    }
                    // 選択解除
                    else
                    {
                        m_selectFuse.SelectUIFuse(false);
                        m_selectFuse = null;
                        // マウスカーソル用の画像をデフォルトに変更
                        Cursor.SetCursor(m_cursorDefault, Vector2.zero, CursorMode.Auto);
                    }
                }
            }
            // ゲーム画面
            else
            {
                // 導火線設置
                if (m_selectFuse)
                {
                    m_selectFuse.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
                    m_selectFuse.Type = Fuse.FuseType.Fuse;
                    m_UIFuseCreate.FuseAmount -= new Vector2Int
                        ((int)((m_selectFuse.DefaultPos.x + 1) / 2 + 1) % 2, (int)((m_selectFuse.DefaultPos.x + 1) / 2) % 2);

                    // UI部分の移動
                    foreach (Fuse _fuse in m_uiFuse)
                    {
                        if (_fuse == m_selectFuse)
                            continue;

                        if (m_selectFuse.DefaultPos.x == _fuse.DefaultPos.x &&
                            m_selectFuse.DefaultPos.y < _fuse.DefaultPos.y)
                        {
                            if (_fuse.EndPos == Vector3.zero)
                                _fuse.EndPos = _fuse.transform.localPosition - new Vector3(0.0f, 2.0f, 0.0f);
                            else
                                _fuse.EndPos -= new Vector3(0.0f, 2.0f, 0.0f);
                        }
                    }

                    m_uiFuse.Remove(m_selectFuse);
                    m_fieldFuse.AddLast(m_selectFuse);
                    m_selectFuse.transform.localEulerAngles = m_selectFuse.DefaultRot;
                    m_selectFuse.transform.parent = transform;

                    // UI選択用の子供オブジェクトを削除
                    GameObject child = m_selectFuse.transform.GetChild(m_selectFuse.transform.childCount - 1).gameObject;
                    Destroy(child);

                    m_selectFuse = null;
                    m_createPos = OUTPOS;
                    // マウスカーソル用の画像をデフォルトに変更
                    Cursor.SetCursor(m_cursorDefault, Vector2.zero, CursorMode.Auto);
                }
                // ゲーム加速
                else
                {
                    int store = m_gameSpeed - 1;
                    m_gameSpeed = store ^ 1 + 1;
                }
            }
        }
    }

    /// <summary>
    /// 一番近い導火線の座標から生成位置を決定
    /// </summary>
    /// <param name="mousePos">マウスのワールド座標</param>
    /// <returns>導火線の生成位置</returns>
    private Vector3 FindNearFuse(Vector3 mousePos)
    {
        // 一番近くにあるオブジェクト探索用変数
        Fuse nearObj = m_fieldFuse.First.Value;
        Vector3 objPos = Vector3.zero;

        // マウスのワールド座標に一番近いオブジェクトを取得
        foreach (Fuse fuse in m_fieldFuse)
        {
            // 2回目以降もしくは、距離を比べて遠ければ
            if (Vector3.Distance(nearObj.transform.position, mousePos) <
                Vector3.Distance(fuse.transform.position, mousePos))
                continue;

            nearObj = fuse;
        }

        if (nearObj == null)
            return OUTPOS;

        // そのオブジェクトの上下左右どちらにあるのか
        {
            float disX, disY, disZ;
            disX = mousePos.x - nearObj.transform.position.x;
            disY = mousePos.y - nearObj.transform.position.y;
            disZ = mousePos.z - nearObj.transform.position.z;

            // X座標のが大きい
            if (Mathf.Abs(disX) > Mathf.Abs(disY) && Mathf.Abs(disX) > Mathf.Abs(disZ))
            {
                if (disX >= 0)
                    objPos = nearObj.transform.position + new Vector3(AdjustParameter.Fuse_Constant.FUSE_SCALE, 0.0f, 0.0f);
                else
                    objPos = nearObj.transform.position - new Vector3(AdjustParameter.Fuse_Constant.FUSE_SCALE, 0.0f, 0.0f);
            }
            // Y座標のが近い
            else if (Mathf.Abs(disY) > Mathf.Abs(disZ))
            {
                if (disY >= 0)
                    objPos = nearObj.transform.position + new Vector3(0.0f, AdjustParameter.Fuse_Constant.FUSE_SCALE, 0.0f);
                else
                    objPos = nearObj.transform.position - new Vector3(0.0f, AdjustParameter.Fuse_Constant.FUSE_SCALE, 0.0f);
            }
            // Z座標のが近い
            else
            {
                if (disZ >= 0)
                    objPos = nearObj.transform.position + new Vector3(0.0f, 0.0f, AdjustParameter.Fuse_Constant.FUSE_SCALE);
                else
                    objPos = nearObj.transform.position - new Vector3(0.0f, 0.0f, AdjustParameter.Fuse_Constant.FUSE_SCALE);
            }
        }

        Vector3Int half = new Vector3Int((int)Mathf.Floor((float)(m_stageSize.x / 2.0f)),
            (int)Mathf.Floor((float)(m_stageSize.x / 2.0f)), (int)Mathf.Floor((float)(m_stageSize.z / 2.0f)));
        Vector3Int stageMax = half;
        Vector3Int stageMin = -half;

        foreach (Fuse fuse in m_fieldFuse)
        {
            // 2回目以降もしくは、距離を比べて遠ければ
            if (fuse.transform.position != objPos &&

            // 画面外処理
             objPos.x <= stageMax.x && objPos.x >= stageMin.x &&
             objPos.y <= stageMax.y && objPos.y >= stageMin.y &&
             objPos.z <= stageMax.z && objPos.z >= stageMin.z)
                continue;

            return m_createPos;
        }
        return objPos;
    }

    /// <summary>
    /// 導火線が燃え尽きた処理
    /// </summary>
    /// <param name="_fuse">燃え尽きた導火線</param>
    public void BurnOutFuse(Fuse _fuse)
    {
        m_fieldFuse.Remove(_fuse);
        m_burnCount--;
        if (_fuse.Type == Fuse.FuseType.Goal)
        {
            m_resultClear.SetActive(true);
            m_gameStep = GameClear;
            m_saveObj = Instantiate(m_fireworks, _fuse.transform.position, Quaternion.identity);
            END_FIRE_POS += _fuse.transform.position;

            if (m_selectFuse)
                m_selectFuse.SelectUIFuse(false);
            m_UIFuseCreate.enabled = true;
           Camera.main.GetComponent<MainCamera>().Control = false;
        }
        else if (m_burnCount <= 0)
        {
            m_resultGameover.SetActive(true);
            m_gameStep = GameOver;

            if (m_selectFuse)
                m_selectFuse.SelectUIFuse(false);
            m_UIFuseCreate.enabled = true;
            Camera.main.GetComponent<MainCamera>().Control = false;
        }
    }

    /// <summary>
    /// ゲームクリア処理
    /// </summary>
    public void GameClear()
    {
        // 花火を移動させメインカメラに注視させる
        m_saveObj.transform.position = Vector3.Lerp(m_saveObj.transform.position, END_FIRE_POS, Time.deltaTime);
        Camera.main.transform.LookAt(m_saveObj.transform.position);
        Camera.main.rect = new Rect(0.0f, 0.0f, Mathf.Lerp(Camera.main.rect.width, 1.0f, Time.deltaTime), 1.0f);
        // UIの移動
        StartCoroutine(SlideResultUI(m_resultClear));
    }

    /// <summary>
    /// ゲームオーバー処理
    /// </summary>
    public void GameOver()
    {
        Camera.main.rect = new Rect(0.0f, 0.0f, Mathf.Lerp(Camera.main.rect.width, 1.0f, Time.deltaTime), 1.0f);
        // UIの移動
        StartCoroutine(SlideResultUI(m_resultGameover));
    }

    /// <summary>
    /// リザルトのUIの移動
    /// </summary>
    /// <param name="result">リザルトの種類</param>
    /// <returns></returns>
    private IEnumerator SlideResultUI(GameObject result)
    {
        float startTime = Time.time;             // 開始時間
        Vector3 moveDistance_text;            // 移動距離および方向
        Vector3 moveDistance_button;            // 移動距離および方向
        Transform _text = result.transform.GetChild((int)ResultPlacement.Text);
        Transform _button = result.transform.GetChild((int)ResultPlacement.Button);

        Vector3 startPos_text = _text.localPosition;  // 開始位置
        Vector3 startPos_rePlay = _button.localPosition;  // 開始位置

        moveDistance_text = TEXT_POS - startPos_text;
        moveDistance_button = BUTTON_POS - startPos_rePlay;

        while ((Time.time - startTime) < AdjustParameter.Result_Constant.DURATION)
        {
            _text.localPosition = startPos_text + moveDistance_text * m_animCurve.Evaluate((Time.time - startTime) / AdjustParameter.Result_Constant.DURATION);
            _button.localPosition = startPos_rePlay + moveDistance_button * m_animCurve.Evaluate((Time.time - startTime) / AdjustParameter.Result_Constant.DURATION);
            yield return 0;
        }
    }

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// ボタンの処理

    public void BackToTitle()
    {
        SceneManager.LoadScene("Title");
    }

    public void RePlay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void NextStsge()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

