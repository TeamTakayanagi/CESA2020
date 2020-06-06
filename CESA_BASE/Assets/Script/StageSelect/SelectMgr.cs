using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectMgr : SingletonMonoBehaviour<SelectMgr>
{
    private static int ms_selectStage = 0;          // 直前に遊んだステージ
    private static int ms_tryStage = -1;               //
    private int m_clearStage = 0;               //

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

    override protected void Awake()
    {
        // セーブデータを読み込む
        if (ms_saveData.data == null)
            ms_saveData = Utility.CSVFile.LoadBin("SaveData", m_stageList.Count);

        // ステージデータを保存
        m_stageList.AddRange(GameObject.FindGameObjectWithTag("StageParent").GetComponentsInChildren<Stage>());
        for (int i = 0, size = m_stageList.Count; i < size; ++i)
        {
            m_stageList[i].StageNum = i + 1;
            m_stageList[i].ClearState = int.Parse(ms_saveData.data[i]);
            // 最後までクリアしたか、もしくは次のステージがクリアされていないなら
            if (m_stageList[i].ClearState > 0)
            {
                // クリアした一番先のステージは
                m_clearStage = i + 1;
            }
        }
    }

    void Start()
    {
        Transform fuseParent = GameObject.FindGameObjectWithTag("fuseParent").transform;
        // 導火線の見た目を変化
        Transform _fuseGroup;
        SelectFuse[] _fuseList;
        SelectFuse _fuse;
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
            // クリアしていないステージの2つ目以降
            else if(i > m_clearStage - 1)
            {
                // 導火線を描画しない
                //_fuseGroup.gameObject.SetActive(false);
            }
            // 未クリアのステージをクリアした
            else if (i == m_clearStage - 1 && ms_selectStage >= m_clearStage)
            {
                // 1つ目には、ステージの座標を参照して進行向きを求める
                _fuseList[0].SetTarget(m_stageList[m_clearStage - 1].transform.position);
                // 先頭に着火
                _fuseList[0].BurnStart();
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

        m_camera = Camera.main.GetComponent<MainCamera>();
        m_camera.Type = MainCamera.CameraType.SwipeMove;

        // ステージ番号順にソート
        m_stageList.Sort((a, b) => a.StageNum - b.StageNum);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_camera.Type == MainCamera.CameraType.SwipeMove)
        {
            if (Input.GetMouseButtonDown(0))
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
                        m_camera.StartZoomIn(_stage.transform.position);

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
    }

    /// <summary>
    /// 矢印をクリック
    /// </summary>
    /// <param name="direct">右（1）左（-1）</param>
    public void ClickArrow(int direct)
    {
        if (FadeMgr.Instance.State != FadeBase.FadeState.None)
            return;

        m_zoomObj = m_stageList[Mathf.Clamp(m_zoomObj.StageNum - 1 + direct, 0, m_stageList.Count - 1)];
        m_camera.StartZoomIn(m_zoomObj.transform.position);
    }

    public void ZoomOut()
    {
        if (FadeMgr.Instance.State != FadeBase.FadeState.None)
            return;

        m_uiArrow.SetActive(false);
        m_uiStartBack.SetActive(false);
        m_camera.StartZoomOut();
    }

    public void SceneLoad()
    {
        if (FadeMgr.Instance.State != FadeBase.FadeState.None)
            return;

        // クリアしたステージの次のステージもしくはクリア済みのステージか
        if (m_zoomObj.StageNum == m_clearStage + 1 || int.Parse(ms_saveData.data[m_zoomObj.StageNum - 1]) > 0)
        {
            m_uiArrow.SetActive(false);
            m_uiStartBack.SetActive(false);
            m_camera.StartZoomFade(m_zoomObj.transform.position);
            ms_tryStage = ms_selectStage = m_zoomObj.StageNum;
            // ステージセレクト→ゲーム のフェード
            FadeMgr.Instance.StartFade(FadeMgr.FadeType.Scale, NameDefine.Game_Scene.GAME_MAIN);
        }
    }
}