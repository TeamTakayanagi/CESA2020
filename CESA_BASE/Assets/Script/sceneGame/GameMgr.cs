using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using DG.Tweening;
using UnityEngine.UI;

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
    private Vector3Int m_stageSize = Vector3Int.zero;                   // ステージサイズ
    [SerializeField]
    private Sprite[] m_SpeedTex = null;

    // 定数
    private const float SLIDE_UI = 1.0f;                                // UIの移動時間
    private const float FILED_ADJUST_VALUE_Z = 0.25f; 
    private const float MOUSE_ADJUST_VALUE_Y = 0.25f; 
    private readonly Vector3 TEXT_POS = new Vector3(0.0f, 150, 0.0f);           // リザルトテキストの移動距離
    private readonly Vector3 BUTTON_POS = new Vector3(0.0f, -200.0f, 0.0f);      // リザルトボタンの移動距離              
    private readonly Vector3 OUTPOS = new Vector3(-50, -50, -50);       // 導火線を生成できない位置
    private readonly AnimationCurve m_animCurve = AnimationCurve.Linear(0, 0, 1, 1);   // リザルトUIの移動用

    private int m_burnCount = 1;                                        // 燃えている導火線の数
    private int m_gameSpeed = 1;                                        // ゲーム加速処理
    private Vector3 m_createPos = Vector3.zero;                         // 導火線の生成位置
    private GameStep m_gameStep = null;                                 // 現在のゲームの進行状況の関数
    private GameObject m_resultClear = null;                            // ゲームクリア用のUIの親オブジェクト
    private GameObject m_resultGameover = null;                         // ゲームオーバー用のUIの親オブジェクト
    private GameButton m_gameUI = null;                                  // ゲームオーバー用のUIの親オブジェクト
    private GameFuse m_selectFuse = null;                                   // 選択しているUIの導火線   
    private GameObject m_saveObj = null;                                // 各GameStepごとにオブジェクトを格納（スタート：カウントダウン数字　ゲームクリア：花火）
    private StartProduction m_start = null;                                  // ゲームオーバー用のUIの親オブジェクト
    private UIFuseCreate m_UIFuseCreate = null;                         // UIの導火線生成オブジェクト
    private LinkedList<GameObject> m_fieldObject = new LinkedList<GameObject>();      // ゲーム画面のオブジェクト
    private LinkedList<GameGimmick> m_gimmickList = new LinkedList<GameGimmick>();      // ゲーム画面のオブジェクト
    private LinkedList<GameFuse> m_uiFuse = new LinkedList<GameFuse>();         // UI部分の導火線

    private Image m_uiSpeed = null;

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
    public GameFuse UIFuse
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
        Utility.CSVFile.CSVData info = Utility.CSVFile.LoadCsv(
            ProcessedtParameter.CSV_Constant.STAGE_DATA_PATH + SelectMgr.SelectStage);
        StageCreateMgr.Instance.CreateStage(transform, info);
        m_stageSize = info.size;
        m_gameStep = GameStart;

        base.Awake();
    }

    // Start is called before the first frame update
    void Start()
    {
        Camera.main.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
        Camera.main.GetComponent<MainCamera>().Near = Mathf.Max(m_stageSize.x, m_stageSize.z) + 1;
        // マウス制御クラスにカメラの情報を渡す
        InputMouse.RoadCamera();

        // ゲームクリア用のUIの親オブジェクト取得
        m_resultClear = GameObject.FindGameObjectWithTag(NameDefine.TagName.UIGameClear);
        // ゲームオーバー用のUIの親オブジェクト取得
        m_resultGameover = GameObject.FindGameObjectWithTag(NameDefine.TagName.UIGameOver);
        // ゲームのポーズ処理の親オブジェクト取得
        m_gameUI = FindObjectOfType<GameButton>();
        // ゲームスタート用のオブジェクト格納
        m_start = FindObjectOfType<StartProduction>();
        // UIの導火線生成オブジェクト取得し、動きを止める
        m_UIFuseCreate = FindObjectOfType<UIFuseCreate>();
        m_UIFuseCreate.enabled = false;

        // 初期生成位置はわからないので生成不可能場所を格納
        m_createPos = OUTPOS;

        // 地形生成
        TerrainCreate terrainCreate = FindObjectOfType<TerrainCreate>();
        terrainCreate.CreateGround(m_stageSize.x, m_stageSize.z, -m_stageSize.y / 2 - 1);
        terrainCreate.CreateBackGround(SelectMgr.SelectStage);

        // フィールドオブジェクトの取得
        GameFuse[] _fuseList = FindObjectsOfType<GameFuse>();
        foreach (GameFuse _fuse in _fuseList)
        {
            if (_fuse.State == FuseBase.FuseState.UI)
                m_uiFuse.AddLast(_fuse);
            else
            {
                if(_fuse.Type == GameFuse.FuseType.Start)
                    m_saveObj =_fuse.gameObject;

                m_fieldObject.AddLast(_fuse.gameObject);
            }
        }
        GameGimmick[] _gimmicks = FindObjectsOfType<GameGimmick>();
        foreach (GameGimmick _gimmick in _gimmicks)
        {
            if (_gimmick.Type == GameGimmick.GimmickType.Goal)
                m_gimmickList.AddLast(_gimmick);
            m_fieldObject.AddLast(_gimmick.gameObject);
        }
        Camera.main.GetComponent<MainCamera>().Control = true;
        Sound.Instance.PlayBGM(Audio.BGM.GameMain);

        m_uiSpeed = GameObject.FindGameObjectWithTag(NameDefine.TagName.UIGameButton).transform.GetChild(0).GetChild(0).GetComponent<Image>();
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
        if (m_start.State == StartProduction.Production.end)
        {
            m_gameStep = GameMain;

            GameFuse _start = m_saveObj.GetComponent<GameFuse>();
            _start.GameStart();

            DestroyImmediate(m_start.gameObject);
            m_UIFuseCreate.enabled = true;
            // 開始エフェクト（花火作成）
            EffectManager.Instance.EffectCreate(EffectManager.Instance.GetFireworks(), Vector3.zero, Quaternion.identity);
            
            // 格納用オブジェクトの中身を削除
            m_saveObj = null;
        }
        else if(Input.GetMouseButtonDown(0) && m_start.State == StartProduction.Production.wait)
        {
            // UI部分を表示・稼働
            Camera.main.DORect(new Rect(0.0f, 0.0f, ProcessedtParameter.Camera_Constant.RECT_WIDTH, SLIDE_UI), 1.0f);
            // サウンド
            Sound.Instance.PlaySE(Audio.SE.Click, GetInstanceID());
            m_gameUI.SlideVar();

            m_start.State = StartProduction.Production.moveY;
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
            Vector3 screen = Camera.main.WorldToScreenPoint(transform.position) -
                new Vector3(0.0f, 0.0f, FILED_ADJUST_VALUE_Z * m_stageSize.z);

            screen = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screen.z);
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(screen) -
                new Vector3(0.0f, MOUSE_ADJUST_VALUE_Y, 0.0f);

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

        Ray ray = new Ray();
        RaycastHit hit = new RaycastHit();
        // 設置or選択
        if (Input.GetMouseButtonDown(0))
        {
            // UI画面
            if (InputMouse.MouseEria())
            {
                // サブカメラ取得
                ray = InputMouse.GetScreenCamera().GetComponent<Camera>().
                    ScreenPointToRay(Input.mousePosition);

                // 導火線を選択
                if (Physics.Raycast(ray, out hit))
                {
                    // 新規選択
                    if (!m_selectFuse || m_selectFuse.gameObject != hit.collider.transform.parent.gameObject)
                    {
                        // サウンド
                        Sound.Instance.PlaySE(Audio.SE.Catch, GetInstanceID());

                        GameFuse _fuse = hit.collider.transform.parent.GetComponent<GameFuse>();
                        if (!_fuse || _fuse.EndPos != Vector3.zero)
                            return;

                        if (_fuse.State == FuseBase.FuseState.UI)
                        {
                            if (m_selectFuse)
                                m_selectFuse.SelectUIFuse(false);
                            m_selectFuse = _fuse;
                            m_selectFuse.SelectUIFuse(true);
                            // マウスカーソル用の画像を選択時に変更
                            InputMouse.ChangeCursol(InputMouse.Mouse_Cursol.Catch);
                        }
                    }
                    // 選択解除
                    else
                    {
                        Sound.Instance.PlaySE(Audio.SE.Release, GetInstanceID());
                        m_selectFuse.SelectUIFuse(false);
                        m_selectFuse = null;
                        // マウスカーソル用の画像をデフォルトに変更
                        InputMouse.ChangeCursol(InputMouse.Mouse_Cursol.Default);
                    }
                }
            }
            // ゲーム画面
            else
            {
                // 導火線設置
                if (m_selectFuse)
                {
                    m_selectFuse.Type = GameFuse.FuseType.Normal;
                    m_selectFuse.State = GameFuse.FuseState.None;
                    m_UIFuseCreate.FuseAmount -= new Vector2Int
                        ((int)((m_selectFuse.DefaultPos.x + 1) / 2 + 1) % 2, (int)((m_selectFuse.DefaultPos.x + 1) / 2) % 2);

                    // UI部分の移動
                    foreach (GameFuse _fuse in m_uiFuse)
                    {
                        if (_fuse == m_selectFuse)
                            continue;

                        if (m_selectFuse.DefaultPos.x == _fuse.DefaultPos.x &&
                            m_selectFuse.DefaultPos.y < _fuse.DefaultPos.y)
                        {
                            if (_fuse.EndPos == Vector3.zero)
                                _fuse.EndPos = _fuse.transform.localPosition - new Vector3(0.0f, ProcessedtParameter.UI_Object_Constant.INTERVAL_Y, 0.0f);
                            else
                                _fuse.EndPos -= new Vector3(0.0f, ProcessedtParameter.UI_Object_Constant.INTERVAL_Y, 0.0f);
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
                    InputMouse.ChangeCursol(InputMouse.Mouse_Cursol.Default);
                }
                // ギミック動作
                else
                {
                    ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit))
                    {
                        // 導火線のギミック始動
                        Transform parent = hit.collider.transform.parent;
                        if (Utility.TagSeparate.getParentTagName(hit.collider.tag) == NameDefine.TagName.Fuse)
                        {
                            hit.collider.gameObject.GetComponent<GameFuse>().OnGimmick();
                        }
                        else if (parent && Utility.TagSeparate.getParentTagName(parent.tag) == NameDefine.TagName.Fuse)
                        {
                            hit.collider.transform.parent.GetComponent<GameFuse>().OnGimmick();
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// ゲームクリア処理
    /// </summary>
    public void GameClear()
    {
        if (m_saveObj)
            Camera.main.transform.LookAt(m_saveObj.transform.position);
    }

    /// <summary>
    /// ゲームオーバー処理
    /// </summary>
    public void GameOver()
    {

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

        // 距離を求める
        Vector3 distance = mousePos - nearObj.transform.position;
        //  一番大きい値が大きさ１それ以外の要素が0の値を受け取る
        Vector3 absolute = Utility.MyMath.GetMaxDirectSign(distance);
        // そのオブジェクトの上下左右前後どちらにあるのか
        Vector3 objPos = nearObj.transform.position + absolute;

        Vector3Int half = new Vector3Int((int)Mathf.Floor(m_stageSize.x / 2.0f),
            (int)Mathf.Floor(m_stageSize.y / 2.0f), (int)Mathf.Floor(m_stageSize.z / 2.0f));
        Vector3Int stageMax = half -
            new Vector3Int((m_stageSize.x + 1) % 2, (m_stageSize.y + 1) % 2, (m_stageSize.z + 1) % 2);
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
            // ゲームメインの終了処理
            GameMainEnd();

            // ゲームオーバー
            m_gameStep = GameOver;
            m_resultGameover.SetActive(true);
            Sound.Instance.PlayBGM(Audio.BGM.GameOver);
            // UIの移動
            StartCoroutine(SlideResultUI(m_resultGameover, 0.0f));
        }
    }

    /// <summary>
    /// 導火線が消えるときの
    /// </summary>
    /// <param name="_fuse">燃え尽きた導火線</param>
    public void DestroyFuse(GameFuse _fuse)
    {
        // ゲーム中以外はこの関数には入らない
        if (m_gameStep != GameMain)
            return;

        m_fieldObject.Remove(_fuse.gameObject);
    }

    /// <summary>
    /// ゴールが燃えた時、燃え尽きた時の処理
    /// </summary>
    public void FireGoal(bool isBurnOut, GameObject fireworks = null)
    {
        if (m_gameStep != GameMain)
            return;

        // 燃え尽きたのなら
        if (isBurnOut)
        {
            // ゲームメインの終了処理
            GameMainEnd();

            // 燃え尽きた場合
            m_resultClear.SetActive(true);
            m_gameStep = GameClear;

            // ゴール
            int fireGoal = 0;
            foreach (GameGimmick _gimmick in m_gimmickList)
            {
                if (!_gimmick.GimmickStart)
                    continue;

                fireGoal++;
            }
            // ゴールが1つなら、クリアを全クリアに書き換え
            if (m_gimmickList.Count == 1)
                fireGoal++;

            SelectMgr.SaveStage(Mathf.Clamp(fireGoal, 0, 2));
            // クリアUIの移動
            StartCoroutine(SlideResultUI(m_resultClear, AdjustParameter.Production_Constant.RESULT_TIME));

            Sound.Instance.PlayBGM(Audio.BGM.Clear);
        }

        // 花火に着火
        if(!m_saveObj)
            m_saveObj = fireworks;
    }

    private void GameMainEnd()
    {
        Sound.Instance.StopAllSE();

        // ライトの親を外す
        Camera.main.transform.GetChild(0).parent = null;
        Camera.main.GetComponent<MainCamera>().Control = false;
        Camera.main.DORect(new Rect(0.0f, 0.0f, 1.0f, 1.0f), SLIDE_UI);

        m_gameUI.SlideVar();
        if (m_selectFuse)
            m_selectFuse.State = GameFuse.FuseState.None;

        // ゲーム部分の事後処理
        if (m_selectFuse)
            m_selectFuse.SelectUIFuse(false);
        m_UIFuseCreate.enabled = false;
    }

    /// <summary>
    /// リザルトのUIの移動
    /// </summary>
    /// <param name="result">リザルトの種類</param>
    /// <returns></returns>
    private IEnumerator SlideResultUI(GameObject result, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        float startTime = Time.time;             // 開始時間
        Vector3 moveDistance_text;              // 移動距離および方向
        Vector3 moveDistance_button;            // 移動距離および方向
        Transform _text = result.transform.GetChild((int)ResultPlacement.Text);
        Transform _button = result.transform.GetChild((int)ResultPlacement.Button);

        Vector3 startPos_text = _text.localPosition;  // 開始位置
        Vector3 startPos_rePlay = _button.localPosition;  // 開始位置

        moveDistance_text = TEXT_POS - startPos_text;
        moveDistance_button = BUTTON_POS - startPos_rePlay;

        while ((Time.time - startTime) < AdjustParameter.Production_Constant.DURATION)
        {
            _text.localPosition = startPos_text + moveDistance_text * m_animCurve.Evaluate((Time.time - startTime) / AdjustParameter.Production_Constant.DURATION);
            _button.localPosition = startPos_rePlay + moveDistance_button * m_animCurve.Evaluate((Time.time - startTime) / AdjustParameter.Production_Constant.DURATION);
            yield return null;
        }
        yield break;
    }

    private void EndScene()
    {
        // サウンド
        Sound.Instance.PlaySE(Audio.SE.Click, GetInstanceID());

        EffectManager.Instance.DestoryEffects();
        m_gameStep = null;

        // 画面内にUIが設置されているなら
        if(m_gameUI.Slide)
            m_gameUI.SlideVar();

        if (Camera.main.rect != new Rect(0.0f, 0.0f, 1.0f, 1.0f))
            Camera.main.DORect(new Rect(0.0f, 0.0f, 1.0f, 1.0f), SLIDE_UI);

        m_resultClear.SetActive(false);
        m_resultGameover.SetActive(false);
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // ボタンの処理

    public void BackToTitle()
    {
        EndScene();
        FadeMgr.Instance.StartFade(FadeMgr.FadeType.Rat, NameDefine.Scene_Name.STAGE_SELECT);
    }
    public void Retry()
    {
        if (FadeMgr.Instance.State != FadeBase.FadeState.None)
            return;

        EndScene();

        FadeMgr.Instance.StartFade(FadeMgr.FadeType.Rat, SceneManager.GetActiveScene().name);
    }
    public void NextStsge()
    {

        SelectMgr.SelectStage++;
        EndScene();

        FadeMgr.Instance.StartFade(FadeMgr.FadeType.Rat, SceneManager.GetActiveScene().name);
    }
    public void Retire()
    {
        if (FadeMgr.Instance.State != FadeBase.FadeState.None)
            return;

        EndScene();
        FadeMgr.Instance.StartFade(FadeMgr.FadeType.Rat, NameDefine.Scene_Name.STAGE_SELECT);
    }
    public void ChangeGameSpeed()
    {
        // サウンド
        Sound.Instance.PlaySE(Audio.SE.Click, GetInstanceID());

        m_gameSpeed = m_gameSpeed % 2 + 1;
        m_uiSpeed.sprite = m_SpeedTex[m_gameSpeed - 1];
    }
}

