using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectMgr : SingletonMonoBehaviour<SelectMgr>
{
    // UIの矢印のポジション
    private float UI_POS_X = 430.0f;
    private float UI_POS_Y = 220.0f;

    private static int ms_selectStage = 0;          // 直前に遊んだステージ
    private static int ms_tryStage = -1;            // ステージ選択から当選したステージ
    private int m_clearStage = 0;                   // クリアした一番先のステージ
    private bool m_isSelect = false;                // 

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
    public int ClearStage
    {
        get
        {
           return m_clearStage;
        }
    }
    public bool Select
    {
        set
        {
            m_isSelect = value;
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
        m_isSelect = false;
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
                if (m_stageList[i].StageNum != ms_selectStage)
                {
                    // クリアした一番先のステージは
                    m_clearStage = i + 1;
                }
            }
            m_stageList[i].ClearState = newState;
        }

        // クリアしたステージがあるなら(クリア演出)
        if (m_clearStage > 0 && ms_tryStage < ms_selectStage && ms_tryStage > 0)
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
            // 未クリアのステージをクリアした(クリア演出)
            if (i == m_clearStage - 1 && ms_tryStage < ms_selectStage && ms_tryStage > 0)
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
            // クリア済みのステージなら（クリア済み挑戦してリタイヤもここ）
            else if (i < m_clearStage - 1 ||
                (ms_selectStage - 1 <= m_clearStage && i == m_clearStage - 1))
            {
                // 導火線のまとまりごとに開放していく
                for (int j = 0, size = _fuseList.Length; j < size; ++j)
                    _fuseList[j].BurnOut();
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
            // 最終ステージまでクリアなら
            if (attention == m_stageList.Count)
                attention -= 1;     // 最終ステージに注目

            Vector3 zoom = m_stageList[attention].transform.position;
            m_camera.transform.position = new Vector3(
                zoom.x, m_camera.transform.position.y, m_camera.transform.position.z);
        }

        // ステージ番号順にソート
        m_stageList.Sort((a, b) => a.StageNum - b.StageNum);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_camera.Type == MainCamera.CameraType.SwipeMove && 
            Input.GetMouseButtonDown(0) && m_isSelect)
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
                    SetArrowUI(m_zoomObj);
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
    /// <param name="arrow">そのボタンのオブジェクト</param>
    public void ClickArrow(Transform arrow)
    {
        if (FadeMgr.Instance.State != FadeBase.FadeState.None)
            return;

        // 現在のステージ番号UIの動きを止める
        m_zoomObj.MoveCoroutine(false);

        // ボタンの処理
        arrow.GetComponent<Animator>().ResetTrigger("Highlighted");
        int direct = arrow.tag == "UI/ArrowR" ? 1 : -1;
        m_zoomObj = m_stageList[Mathf.Clamp(m_zoomObj.StageNum - 1 + direct, 0, m_stageList.Count - 1)];
        arrow.localScale = Vector3.one;

        // 移動後の場所のステージを注視する
        m_camera.StartZoomIn(m_zoomObj.transform.position);
        // 変更後ののステージ番号UIの動きを始める
        m_zoomObj.MoveCoroutine(true);

        // 矢印の位置を変更する
        SetArrowUI(m_zoomObj);

        // クリックサウンド
        Sound.Instance.PlaySE("se_click", GetInstanceID());
    }

    private void SetArrowUI(Stage stage)
    {
        Vector3 distance = Vector3.zero;
        Vector3 absolute = Vector3.zero;

        // 最終ステージではない
        if (stage.StageNum != m_stageList.Count)
        {
            Vector3 aft = m_stageList[Mathf.Clamp(m_zoomObj.StageNum, 0, m_stageList.Count - 1)].transform.position;
            distance = aft - stage.transform.position;
            // Y座標は比較しない
            distance.y = distance.z;
            distance.z = 0.0f;
            // 変化が一番大きい要素が１の変数を取得
            absolute = Utility.MyMath.GetMaxDirectSign(distance);
            Transform UIRight = m_uiArrow.transform.GetChild(0);
            UIRight.localPosition = new Vector3(absolute.x * UI_POS_X, absolute.y * UI_POS_Y, 0.0f);
            UIRight.eulerAngles = new Vector3(0.0f, 0.0f, Vector3.Angle(Vector3.left, absolute) * -Vector3.Dot(Vector3.one, absolute));
            UIRight.GetChild(0).rotation = Quaternion.identity;
        }
        // 最初のステージではない
        if (stage.StageNum != 1)
        {
            Vector3 bef = m_stageList[Mathf.Clamp(m_zoomObj.StageNum - 2, 0, m_stageList.Count - 1)].transform.position;
            distance = bef - stage.transform.position;
            // Y座標は比較しない
            distance.y = distance.z;
            distance.z = 0.0f;
            // 変化が一番大きい要素が１の変数を取得
            absolute = Utility.MyMath.GetMaxDirectSign(distance);
            Transform UILeft = m_uiArrow.transform.GetChild(1);
            UILeft.localPosition = new Vector3(absolute.x * UI_POS_X, absolute.y * UI_POS_Y, 0.0f);
            UILeft.eulerAngles = new Vector3(0.0f, 0.0f, Vector3.Angle(Vector3.left, absolute) * -Vector3.Dot(Vector3.one, absolute));
            UILeft.GetChild(0).rotation = Quaternion.identity;
        }

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