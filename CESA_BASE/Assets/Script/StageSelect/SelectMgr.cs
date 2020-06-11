using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectMgr : SingletonMonoBehaviour<SelectMgr>
{
    private const float CAMERA_ATTENTION = 3.5f;
    private static int ms_selectStage = 0;          // 直前に遊んだステージ
    private static int ms_tryStage = -1;            // ステージ選択から当選したステージ
    private int m_clearStage = 0;                   // クリアした一番先のステージ

    private MainCamera m_camera = null;
    private GameObject m_uiArrow = null;
    private GameObject m_uiStartBack = null;

    private List<Stage> m_stageList = new List<Stage>();
    private Stage m_zoomObj = null;

    private static Utility.CSVFile.BinData ms_saveData = new Utility.CSVFile.BinData();

    public static int SelectStage
    {
        get
        {
            return ms_selectStage;
        }
        set
        {
            ms_selectStage = value;
        }
    }

    public Stage ZoomObj
    {
        set
        {
            m_zoomObj = value;
        }
    }

    public static Utility.CSVFile.BinData SaveData
    {
        get
        {
            return ms_saveData;
        }
    }

    public static void SaveStage(int state)
    {
        // クリア段階がすでに完全クリアなら飛ばす
        if (int.Parse(ms_saveData.data[SelectStage - 1]) > state)
            return;

        Utility.CSVFile.SaveBinAt("SaveData", SelectStage, state);
    }

    override protected void Awake()
    {
        // セーブデータを読み込む
        if (ms_saveData.data == null)
            ms_saveData = Utility.CSVFile.LoadBin("SaveData", m_stageList.Count);

        // ステージデータを保存
        m_stageList.AddRange(GameObject.FindGameObjectWithTag(NameDefine.TagName.StageParent).GetComponentsInChildren<Stage>());
        for (int i = 0, size = m_stageList.Count; i < size; ++i)
        {
            int newState = int.Parse(ms_saveData.data[i]);
            m_stageList[i].StageNum = i + 1;

            // 最後までクリアしたか、もしくは次のステージがクリアされていないなら
            if (newState > 0)
            {
                // 挑戦したステージと選択したステージが異なるなら
                if (ms_tryStage != ms_selectStage)
                {
                    // クリアした一番先のステージは
                    m_clearStage = i + 1;
                }
            }
            m_stageList[i].ClearState = newState;
        }

        // クリアしたステージがあるなら
        if (m_clearStage > 0 && ms_selectStage > 0)
            m_stageList[m_clearStage - 1].ClearState *= -1;
    }

    void Start()
    {
        SelectFuse _fuse;
        SelectFuse[] _fuseList;
        Transform _fuseGroup;
        Transform fuseParent = GameObject.FindGameObjectWithTag(NameDefine.TagName.FuseParent).transform;

        // ステージ毎にそこにつながる導火線の確認
        for (int i = 0; i < fuseParent.childCount; ++i)
        {
            _fuseGroup = fuseParent.GetChild(i);
            _fuseList = _fuseGroup.GetComponentsInChildren<SelectFuse>();
            if (i < m_clearStage - 1 || (ms_selectStage < m_clearStage && i == m_clearStage - 1))
            {
                // 導火線のまとまりごとに開放していく
                for (int j = 0, size = _fuseList.Length; j < size; ++j)
                    _fuseList[j].BurnOut();
            }
            // 未クリアのステージをクリアした
            else if (i == m_clearStage - 1 && ms_selectStage >= m_clearStage)
            {
                // 1つ目には、ステージの座標を参照して進行向きを求める
                _fuseList[0].SetTarget(m_stageList[m_clearStage - 1].transform.position);
                // 先頭に着火
                _fuseList[0].BurnStart(_fuseList.Length);
                // まとまりごとに開放していく
                for (int j = 0, size = _fuseList.Length; j < size - 1; ++j)
                {
                    _fuse = _fuseList[j];
                    // 次の導火線を格納
                    _fuse.NextFuse = _fuseList[j + 1];
                }
            }
        }

        // UIのオブジェクトの格納
        m_uiArrow = transform.GetChild(0).gameObject;
        m_uiStartBack = transform.GetChild(1).gameObject;

        // UIのオブジェクトの描画を変更
        m_uiArrow.SetActive(false);
        m_uiStartBack.SetActive(false);

        // カメラの初期化（角度はタイトル演出で使うので、タイトルで設定）
        m_camera = Camera.main.GetComponent<MainCamera>();
        m_camera.Type = MainCamera.CameraType.SwipeMove;
        int attention = ms_selectStage - 1;
        if (attention > 0)
        {
            // 完全クリアなら
            if (attention == m_stageList.Count)
                attention -= 1;     // 最終ステージに注目
            Vector3 zoom = m_stageList[attention].transform.position;
            m_camera.transform.position = new Vector3(zoom.x,
                zoom.y + CAMERA_ATTENTION * Mathf.Sin(Mathf.Deg2Rad * m_camera.transform.localEulerAngles.x),
                zoom.z - CAMERA_ATTENTION * Mathf.Cos(Mathf.Deg2Rad * m_camera.transform.localEulerAngles.x));
        }


        // ステージ番号順にソート
        m_stageList.Sort((a, b) => a.StageNum - b.StageNum);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_camera.Type == MainCamera.CameraType.SwipeMove && 
            Input.GetMouseButtonDown(0))
        {
            RaycastHit _hit = new RaycastHit();
            Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(_ray, out _hit))
            {
                // ステージとの判定
                if (_hit.transform.tag == NameDefine.TagName.Stage)
                {
                    Stage _stage = _hit.transform.GetComponent<Stage>();
                    m_zoomObj = _stage;
                    m_zoomObj.MoveCoroutine(true);
                    m_camera.StartZoomIn(_stage.transform.position);

                    // UIを表示
                    m_uiArrow.SetActive(true);
                    m_uiStartBack.SetActive(true);
                }
                // 背景オブジェクトとの判定
                else if (_hit.transform.tag == NameDefine.TagName.ClickObj)
                {
                    ClickedObject _click = _hit.transform.GetComponent<ClickedObject>();
                    if (_click)
                        _click.OnClick();
                }
            }
        }
    }

    /// <summary>
    /// 矢印をクリック
    /// </summary>
    /// <param name="gameObj">そのボタンのオブジェクト</param>
    public void ClickArrow(GameObject gameObj)
    {
        if (FadeMgr.Instance.State != FadeBase.FadeState.None)
            return;

        m_zoomObj.MoveCoroutine(false);
        gameObj.GetComponent<Animator>().ResetTrigger("Highlighted");

        int direct = gameObj.transform.position.x > gameObj.transform.root.position.x ? 1 : -1;
        m_zoomObj = m_stageList[Mathf.Clamp(m_zoomObj.StageNum - 1 + direct, 0, m_stageList.Count - 1)];
        m_camera.StartZoomIn(m_zoomObj.transform.position);
        gameObj.transform.localScale = Vector3.one;
        m_zoomObj.MoveCoroutine(true);

        // サウンド
        Sound.Instance.PlaySE("se_click", GetInstanceID());
    }

    /// <summary>
    /// ズームアウトの開始時の処理
    /// </summary>
    public void ZoomOut()
    {
        // サウンド
        Sound.Instance.PlaySE("se_click", GetInstanceID());

        if (FadeMgr.Instance.State != FadeBase.FadeState.None)
            return;

        m_uiArrow.SetActive(false);
        m_uiStartBack.SetActive(false);
        m_camera.StartZoomOut();
        m_zoomObj.MoveCoroutine(false);
    }

    /// <summary>
    /// ゲームシーンへの遷移
    /// </summary>
    public void SceneLoad()
    {
        if (FadeMgr.Instance.State != FadeBase.FadeState.None)
            return;

        // クリアしたステージの次のステージもしくはクリア済みのステージか
        if (m_zoomObj.StageNum == m_clearStage + 1 || int.Parse(ms_saveData.data[m_zoomObj.StageNum - 1]) > 0)
        {
            m_uiArrow.SetActive(false);
            m_uiStartBack.SetActive(false);
            m_zoomObj.MoveCoroutine(false);
            m_zoomObj.OffText();
            m_camera.StartZoomFade(m_zoomObj.transform.position);
            ms_tryStage = ms_selectStage = m_zoomObj.StageNum;

            // サウンド
            Sound.Instance.PlaySE("se_click", GetInstanceID());

            // ステージセレクト→ゲーム のフェード
            FadeMgr.Instance.StartFade(FadeMgr.FadeType.Scale, NameDefine.Scene_Name.GAME_MAIN);
        }
    }
}