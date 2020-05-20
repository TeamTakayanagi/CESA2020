﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameMgr : SingletonMonoBehaviour<GameMgr>
{
    // デリゲート宣言
    private delegate void GameStep();

    private enum ResultPlacement
    {
        Text = 0,
        Button,
    }

    [SerializeField]
    private Texture2D m_cursorDefault = null;                           // マウスカーソル（通常時）
    [SerializeField]
    private Texture2D m_cursorCatch = null;                             // マウスカーソル（UIの導火線選択時）
    [SerializeField]
    private Vector3Int m_stageSize = Vector3Int.zero;                   // ステージサイズ

    // 定数
    private readonly Vector3 TEXT_POS = new Vector3(0.0f, 100, 0.0f);   // リザルトテキストの移動距離
    private readonly Vector3 BUTTON_POS = new Vector3(0.0f, 100.0f, 0.0f); // リザルトボタンの移動距離              
    private readonly Vector3 OUTPOS = new Vector3(-50, -50, -50);       // 導火線を生成できない位置
    private readonly AnimationCurve m_animCurve = AnimationCurve.Linear(0, 0, 1, 1);   // リザルトUIの移動用 
    
    private int m_burnCount = 1;                                        // 燃えている導火線の数
    //private int m_gameSpeed = 1;                                        // ゲーム加速処理
    private Vector3 m_createPos = Vector3.zero;                         // 導火線の生成位置
    private GameObject m_resultClear = null;                            // ゲームクリア用のUIの親オブジェクト
    private GameObject m_resultGameover = null;                         // ゲームオーバー用のUIの親オブジェクト
    private Fuse m_selectFuse = null;                                   // 選択しているUIの導火線   
    private UIFuseCreate m_UIFuseCreate = null;                         // UIの導火線生成オブジェクト
    private List<GameObject> m_saveObj = new List<GameObject>();  // 各GameStepごとにオブジェクトを格納（スタート：カウントダウン数字　ゲームクリア：花火）
    private LinkedList<GameObject> m_fieldObject = new LinkedList<GameObject>();      // ゲーム画面の導火線
    private LinkedList<Fuse> m_uiFuse = new LinkedList<Fuse>();         // UI部分の導火線
    private GameStep m_gameStep = null;                                 // 現在のゲームの進行状況の関数

    private float m_tutorialTIme = 0;
    private int m_tutorialState = 0;

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
    //public int GameSpeed
    //{
    //    get
    //    {
    //        return m_gameSpeed;
    //    }
    //}
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

    override protected void Awake()
    {
        Utility.CSVFile.CSVData info = Utility.CSVFile.LoadCsv(ProcessedtParameter.CSV_Constant.STAGE_DATA_PATH + 0);
        StageCreateMgr.Instance.CreateStage(transform, info);
        m_gameStep = GameStart;

        base.Awake();
    }

    // Start is called before the first frame update
    void Start()
    {
        Camera.main.rect = new Rect(0.0f, 0.0f, 0.8f, 1.0f);

        // マウスカーソル用の画像をデフォルトに変更
        Cursor.SetCursor(m_cursorDefault, Vector2.zero, CursorMode.Auto);

        // ゲームクリア用のUIの親オブジェクト取得
        m_resultClear = GameObject.FindGameObjectWithTag(NameDefine.TagName.UIGameClear);

        // ゲームオーバー用のUIの親オブジェクト取得
        m_resultGameover = GameObject.FindGameObjectWithTag(NameDefine.TagName.UIGameOver);

        // UIの導火線生成オブジェクト取得し、動きを止める
        m_UIFuseCreate = FindObjectOfType<UIFuseCreate>();
        m_UIFuseCreate.enabled = false;

        // 初期生成位置はわからないので生成不可能場所を格納
        m_createPos = OUTPOS;

        // 地形生成オブジェクト取得
        TerrainCreate terrainCreate = FindObjectOfType<TerrainCreate>();
        terrainCreate.CreateGround(m_stageSize.x, m_stageSize.z, - m_stageSize.y / 2 - 1);

        // 開始演出準備
        GameObject canvas = GameObject.FindGameObjectWithTag(NameDefine.TagName.UICanvas);
        m_saveObj.Add(canvas.transform.GetChild(0).gameObject);

        // フィールドオブジェクトの取得
        Fuse[] _fuseList = FindObjectsOfType<Fuse>();
        foreach (Fuse _fuse in _fuseList)
        {
            if (_fuse.State == Fuse.FuseState.UI)
                m_uiFuse.AddLast(_fuse);
            else
            {
                if(_fuse.Type == Fuse.FuseType.Start)
                    m_saveObj.Add(_fuse.gameObject);

                m_fieldObject.AddLast(_fuse.gameObject);
            }
        }
        GameGimmick[] _gimmicks = FindObjectsOfType<GameGimmick>();
        foreach (GameGimmick _gimmick in _gimmicks)
        {
            m_fieldObject.AddLast(_gimmick.gameObject);
            // スタート演出のため導火線の更新処理停止（ステージエディタ完成後修正予定）
            _gimmick.enabled = false;
        }

        Camera.main.GetComponent<MainCamera>().Control = true;
        //Sound.Instance.PlayBGM(ConstDefine.Audio.BGM.GameMain);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_gameStep == null)
            return;

         m_gameStep();
    }

    /// <summary>
    /// ゲームスタート処理
    /// </summary>
    void GameStart()
    {
        Number number = m_saveObj[0].GetComponent<Number>();

        if (number && number.TexCount < 0)
        {
            m_gameStep = GameMain;
            Destroy(m_saveObj[0]);

            foreach (Fuse _fuse in m_uiFuse)
                _fuse.enabled = true;
            foreach (GameObject _obj in m_fieldObject)
                _obj.GetComponent<Behaviour>().enabled = true;

            Fuse _start = m_saveObj[1].GetComponent<Fuse>();
            _start.GameStart();

            m_UIFuseCreate.enabled = true;
            m_saveObj.Clear();
        }
    }

    /// <summary>
    /// ゲームメイン処理
    /// </summary>
    void GameMain()
    {
        // 導火線を選択しているなら
        if (m_selectFuse)
        {
            // マウス座標をワールド座標で取得
            Vector3 mousePos = Vector3.zero;
            Vector3 screen = Camera.main.WorldToScreenPoint(transform.position);
            mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screen.z);
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            // 生成場所を取得
            m_createPos = FindNearFuse(mousePos);
            if (m_createPos == OUTPOS)
                return;

            // UI画面
            if (Input.mousePosition.x > Screen.width * 0.8f)
                m_selectFuse.transform.position = m_selectFuse.DefaultPos;
            // ゲーム画面
            else
            {
                m_selectFuse.transform.position = m_createPos;
                m_selectFuse.transform.localEulerAngles = m_selectFuse.DefaultRot;
            }
        }

        RaycastHit hit = new RaycastHit();
        // 設置or選択
        if (Input.GetMouseButtonDown(0))
        {
            // UI画面
            if (Input.mousePosition.x > Screen.width * Camera.main.rect.width)
            {
                // サブカメラ取得
                Ray ray = GameObject.FindGameObjectWithTag(NameDefine.TagName.SubCamera).GetComponent<Camera>().
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

                        if (_fuse.State == Fuse.FuseState.UI)
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
                    m_selectFuse.Type = Fuse.FuseType.Normal;
                    m_selectFuse.State = Fuse.FuseState.None;
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
                                _fuse.EndPos = _fuse.transform.localPosition - new Vector3(0.0f, AdjustParameter.UI_OBJECT_Constant.INTERVAL_Y, 0.0f);
                            else
                                _fuse.EndPos -= new Vector3(0.0f, AdjustParameter.UI_OBJECT_Constant.INTERVAL_Y, 0.0f);
                        }
                    }

                    m_uiFuse.Remove(m_selectFuse);
                    m_fieldObject.AddLast(m_selectFuse.gameObject);
                    m_selectFuse.transform.localEulerAngles = m_selectFuse.DefaultRot;
                    m_selectFuse.transform.parent = transform;
                    m_selectFuse.DefaultPos = transform.position;

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
                    Ray ray = new Ray();
                    ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    //マウスクリックした場所からRayを飛ばし、オブジェクトがあればtrue
                    if (Physics.Raycast(ray, out hit))
                    {
                        if (Utility.TagSeparate.getParentTagName(hit.collider.gameObject.tag) == NameDefine.TagName.Fuse)
                        {
                            // 導火線のギミック始動
                            hit.collider.gameObject.GetComponent<Fuse>().OnGimmick();
                        }
                    }
                    //else
                    //{
                    //    int store = m_gameSpeed - 1;
                    //    m_gameSpeed = store % 2 + 1;
                    //}
                }
            }
        }
    }

    /// <summary>
    /// ゲームクリア処理
    /// </summary>
    public void GameClear()
    {
        Vector3 centerPos = Vector3.zero;

        // 花火を移動させメインカメラに注視させる
        if (m_saveObj.Count > 0)
        {
            foreach (GameObject _goal in m_saveObj)
            {
                _goal.transform.position = Vector3.Lerp(_goal.transform.position, new Vector3(
                    _goal.transform.position.x, AdjustParameter.Result_Constant.END_FIRE_POS_Y, _goal.transform.position.z), Time.deltaTime);
                centerPos += _goal.transform.position;
            }
        }

        Camera.main.transform.LookAt(centerPos);
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

    /// チュートリアルステージ処理
    /// </summary>
    private void GameTutorial()
    {
        //if (m_tutorialTIme <= 3 * 60)
        //return;

        // 開始後数秒何もできない

        // UIの導火線をクリックさせる
        // この時、ほかの動作は停止
        RaycastHit hit = new RaycastHit();
        switch (m_tutorialState)
        {
            // 開始後数秒何もできない
            case 0:
                {
                    m_tutorialTIme += Time.deltaTime;
                    if (m_tutorialTIme >= 2)
                    {
                        foreach (Fuse _fuse in m_uiFuse)
                            _fuse.enabled = false;
                        foreach (GameObject _obj in m_fieldObject)
                            _obj.GetComponent<Behaviour>().enabled = false;

                        m_UIFuseCreate.enabled = false;
                        m_tutorialState++;
                    }
                }
                break;

            // UIの導火線をクリックさせる
            // この時、ほかの動作は停止
            case 1:
                {
                    // ガイドを出す
                    {

                    }

                    // 設置or選択
                    if (Input.GetMouseButtonDown(0))
                    {
                        // UI画面
                        if (Input.mousePosition.x > Screen.width * Camera.main.rect.width)
                        {
                            // サブカメラ取得
                            Ray ray = GameObject.FindGameObjectWithTag(NameDefine.TagName.SubCamera).GetComponent<Camera>().
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

                                    if (_fuse.State == Fuse.FuseState.UI)
                                    {
                                        if (m_selectFuse)
                                            m_selectFuse.SelectUIFuse(false);
                                        m_selectFuse = _fuse;
                                        m_selectFuse.SelectUIFuse(true);
                                        // マウスカーソル用の画像を選択時に変更
                                        Cursor.SetCursor(m_cursorCatch, Vector2.zero, CursorMode.Auto);

                                        m_tutorialState = 2;
                                    }
                                }
                                // 選択解除
                                else
                                {
                                    m_selectFuse.SelectUIFuse(false);
                                    m_selectFuse = null;
                                    // マウスカーソル用の画像をデフォルトに変更
                                    Cursor.SetCursor(m_cursorDefault, Vector2.zero, CursorMode.Auto);

                                    m_tutorialState = 1;
                                }
                            }
                        }
                    }
                }
                break;

            // 選んだ導火線を配置させる
            // この時も、ほかの動作は停止
            case 2:
                {
                    // ガイドを出す
                    {

                    }

                    // 導火線を選択しているなら
                    if (m_selectFuse)
                    {
                        // マウス座標をワールド座標で取得
                        Vector3 mousePos = Vector3.zero;
                        Vector3 screen = Camera.main.WorldToScreenPoint(transform.position);
                        mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screen.z);
                        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
                        // 生成場所を取得
                        m_createPos = FindNearFuse(mousePos);
                        if (m_createPos == OUTPOS)
                            return;

                        // UI画面
                        if (Input.mousePosition.x > Screen.width * 0.8f)
                            m_selectFuse.transform.position = m_selectFuse.DefaultPos;
                        // ゲーム画面
                        else
                        {
                            m_selectFuse.transform.position = m_createPos;
                            m_selectFuse.transform.localEulerAngles = m_selectFuse.DefaultRot;
                        }
                    }

                    // 設置or選択
                    if (Input.GetMouseButtonDown(0))
                    {
                        // UI画面
                        if (Input.mousePosition.x > Screen.width * Camera.main.rect.width)
                        {
                        }
                        // ゲーム画面
                        else
                        {
                            // 導火線設置
                            if (m_selectFuse)
                            {
                                m_selectFuse.Type = Fuse.FuseType.Normal;
                                m_selectFuse.State = Fuse.FuseState.None;
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
                                            _fuse.EndPos = _fuse.transform.localPosition - new Vector3(0.0f, AdjustParameter.UI_OBJECT_Constant.INTERVAL_Y, 0.0f);
                                        else
                                            _fuse.EndPos -= new Vector3(0.0f, AdjustParameter.UI_OBJECT_Constant.INTERVAL_Y, 0.0f);
                                    }
                                }

                                m_uiFuse.Remove(m_selectFuse);
                                m_fieldObject.AddLast(m_selectFuse.gameObject);
                                m_selectFuse.transform.localEulerAngles = m_selectFuse.DefaultRot;
                                m_selectFuse.transform.parent = transform;
                                m_selectFuse.DefaultPos = transform.position;

                                // UI選択用の子供オブジェクトを削除
                                GameObject child = m_selectFuse.transform.GetChild(m_selectFuse.transform.childCount - 1).gameObject;
                                Destroy(child);

                                m_selectFuse = null;
                                m_createPos = OUTPOS;
                                // マウスカーソル用の画像をデフォルトに変更
                                Cursor.SetCursor(m_cursorDefault, Vector2.zero, CursorMode.Auto);

                                foreach (Fuse _fuse in m_uiFuse)
                                    _fuse.enabled = true;
                                foreach (GameObject _obj in m_fieldObject)
                                    _obj.GetComponent<Behaviour>().enabled = true;
                                m_UIFuseCreate.enabled = true;

                                m_tutorialState = 3;
                            }
                        }

                    }
                }
                break;

            // 燃え広がるまで待機
            case 3:
                {
                    m_tutorialTIme += Time.deltaTime;
                    if (m_tutorialTIme >= 10)
                    {
                        foreach (Fuse _fuse in m_uiFuse)
                            _fuse.enabled = false;
                        foreach (GameObject _obj in m_fieldObject)
                            _obj.GetComponent<Behaviour>().enabled = false;
                        m_UIFuseCreate.enabled = false;

                        m_tutorialState = 4;
                    }
                }
                break;

            // 通常プレイ
            case 4:
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        m_gameStep = GameMain;

                        m_tutorialState = 5;
                    }
                }
                break;
        }
    }

    private void RunToGamemain(bool _flg)
    {
        foreach (Fuse _fuse in m_uiFuse)
            _fuse.enabled = _flg;
        foreach (GameObject _obj in m_fieldObject)
            _obj.GetComponent<Behaviour>().enabled = _flg;

        m_UIFuseCreate.enabled = _flg;
    }

    /// <summary>
    /// 一番近い導火線の座標から生成位置を決定
    /// </summary>
    /// <param name="mousePos">マウスのワールド座標</param>
    /// <returns>導火線の生成位置</returns>
    private Vector3 FindNearFuse(Vector3 mousePos)
    {
        // 一番近くにあるオブジェクト探索用変数
        GameObject nearObj = m_fieldObject.First.Value;
        Vector3 objPos = Vector3.zero;

        // マウスのワールド座標に一番近いオブジェクトを取得
        foreach (GameObject _obj in m_fieldObject)
        {
            // 導火線以外は対象にしないかつ、2回目以降もしくは、距離を比べて遠ければ
            if (Utility.TagSeparate.getParentTagName(_obj.tag) != NameDefine.TagName.Fuse ||
                Vector3.Distance(nearObj.transform.position, mousePos) <
                Vector3.Distance(_obj.transform.position, mousePos))
                continue;

            nearObj = _obj;
        }

        if (nearObj == null)
            return m_createPos;

        // そのオブジェクトの上下左右前後どちらにあるのか
        {
            //float disX, disY, disZ, max;
            //disX = mousePos.x - nearObj.transform.position.x;
            //disY = mousePos.y - nearObj.transform.position.y;
            //disZ = mousePos.z - nearObj.transform.position.z;
            //max = Mathf.Max(Mathf.Abs(disX), Mathf.Abs(disY), Mathf.Abs(disZ));
            //
            //// X座標のが大きい
            //if (Mathf.Abs(disX) == max)
            //{
            //    if (disX >= 0)
            //        objPos = nearObj.transform.position + new Vector3(AdjustParameter.Fuse_Constant.DEFAULT_SCALE, 0.0f, 0.0f);
            //    else
            //        objPos = nearObj.transform.position - new Vector3(AdjustParameter.Fuse_Constant.DEFAULT_SCALE, 0.0f, 0.0f);
            //}
            //// Y座標のが近い
            //else if (Mathf.Abs(disY) == max)
            //{
            //    if (disY >= 0)
            //        objPos = nearObj.transform.position + new Vector3(0.0f, AdjustParameter.Fuse_Constant.DEFAULT_SCALE, 0.0f);
            //    else
            //        objPos = nearObj.transform.position - new Vector3(0.0f, AdjustParameter.Fuse_Constant.DEFAULT_SCALE, 0.0f);
            //}
            //// Z座標のが近い
            //else
            //{
            //    if (disZ >= 0)
            //        objPos = nearObj.transform.position + new Vector3(0.0f, 0.0f, AdjustParameter.Fuse_Constant.DEFAULT_SCALE);
            //    else
            //        objPos = nearObj.transform.position - new Vector3(0.0f, 0.0f, AdjustParameter.Fuse_Constant.DEFAULT_SCALE);
            //}

            Vector3 distance, absolute;
            // 距離を求める
            distance = mousePos - nearObj.transform.position;
            // 絶対値にしたものを入れる
            absolute = new Vector3(Mathf.Abs(distance.x), Mathf.Abs(distance.y), Mathf.Abs(distance.z));
            // XYZの絶対値の最大値を求める
            float max = Mathf.Max(absolute.x, absolute.y, absolute.z);
            // 一番大きい要素は１、そのほか2つは0を入れる
            absolute -= new Vector3(max, max, max);
            absolute = new Vector3(Mathf.Clamp01(Mathf.Floor(absolute.x + 1)),
                Mathf.Clamp01(Mathf.Floor(absolute.y + 1)), Mathf.Clamp01(Mathf.Floor(absolute.z + 1)));

            objPos = nearObj.transform.position +
                    absolute * AdjustParameter.Fuse_Constant.DEFAULT_SCALE * Mathf.Sign(Vector3.Dot(distance, absolute));
        }

        Vector3Int half = new Vector3Int((int)Mathf.Floor(m_stageSize.x / 2.0f),
            (int)Mathf.Floor(m_stageSize.x / 2.0f), (int)Mathf.Floor(m_stageSize.z / 2.0f));
        Vector3Int stageMax = half;
        Vector3Int stageMin = -half;

        foreach (GameObject _obj in m_fieldObject)
        {
            // 2回目以降もしくは、距離を比べて遠ければ
            if (_obj.transform.position != objPos &&

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
    public void BurnOutFuse()
    {
        // ゲーム中以外はこの関数には入らない
        if (m_gameStep != GameMain)
            return;

        m_burnCount--;

        // 燃えてる導火線がなくなったなら
        if (m_burnCount <= 0)
        {
            // ゲームオーバー
            if (m_saveObj.Count == 0)
            {
                m_resultGameover.SetActive(true);
                m_gameStep = GameOver;
            }
            // ゲームクリア
            else
            {
                m_resultClear.SetActive(true);
                m_gameStep = GameClear;
            }

            // ゲーム部分の事後処理
            if (m_selectFuse)
                m_selectFuse.SelectUIFuse(false);
            m_UIFuseCreate.enabled = false;
            Camera.main.GetComponent<MainCamera>().Control = false;
        }
    }
    /// <summary>
    /// 導火線が消えるときの
    /// </summary>
    /// <param name="_fuse">燃え尽きた導火線</param>
    public void DestroyFuse(Fuse _fuse)
    {
        // ゲーム中以外はこの関数には入らない
        if (m_gameStep != GameMain)
            return;

        m_fieldObject.Remove(_fuse.gameObject);
    }

    /// <summary>
    /// ゴールが燃えた時、燃え尽きた時の処理
    /// </summary>
    /// <param name="goal">ゴールのオブジェクト</param>
    public void FireGoal(GameGimmick goal, bool isBurnOut)
    {
        if (m_gameStep != GameMain)
            return;

        // 燃え尽きたのなら
        if (isBurnOut)
        {
            // 燃え尽きた場合
            m_resultClear.SetActive(true);
            m_gameStep = GameClear;

            // ゲーム部分の事後処理
            if (m_selectFuse)
                m_selectFuse.SelectUIFuse(false);
            m_UIFuseCreate.enabled = false;
            Camera.main.GetComponent<MainCamera>().Control = false;
        }
        // 花火に着火
        else
        {
            m_saveObj.Add(goal.gameObject);
        }
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
        SceneManager.LoadScene("StageSelectLatte");
    }

    public void RePlay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        m_gameStep = null;
    }
    public void NextStsge()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        m_gameStep = null;
    }
}

