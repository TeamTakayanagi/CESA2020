using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectMgr : SingletonMonoBehaviour<SelectMgr>
{
    private static int ms_selectStage = -1;          // 直前に遊んだステージ
    private static int m_tryStage = 1;               //

    private MainCamera m_camera = null;
    private GameObject m_uiArrow = null;
    private GameObject m_uiStartBack = null;
    
    private List<Stage> m_stages = new List<Stage>();
    public Stage m_zoomObj = null;

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
        if(ms_saveData.data == null)
            ms_saveData = Utility.CSVFile.LoadBin("SaveData", m_stages.Count);

        // ステージデータを保存
        m_stages.AddRange(GameObject.FindGameObjectWithTag("StageParent").GetComponentsInChildren<Stage>());
        for(int i = 0; i < m_stages.Count; ++i)
        {
            m_stages[i].StageNum = i + 1;
            m_stages[i].ClearState = int.Parse(ms_saveData.data[i]);
        }

        // 
        GameObject fuseParent = GameObject.FindGameObjectWithTag("fuseParent");

        if (m_tryStage <= ms_selectStage)
        {
            // 
            for (int i = m_tryStage; i < ms_selectStage; ++i)
            {
                Transform _fuseGroup = fuseParent.transform.GetChild(i - 1);
                // まとまりごとに開放していく

            }
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        m_uiArrow = transform.GetChild(0).gameObject;
        m_uiStartBack = transform.GetChild(1).gameObject;
        m_camera = Camera.main.GetComponent<MainCamera>();
        m_camera.Type = MainCamera.CameraType.SwipeMove;
        m_camera.Control = true;


        m_uiArrow.SetActive(false);
        m_uiStartBack.SetActive(false);

        // ステージ番号順にソート
        m_stages.Sort((a, b) => a.StageNum - b.StageNum);
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

        // 選択テキストの上下移動呼び出し
        if(m_zoomObj)
        StartCoroutine(m_zoomObj.MoveText());

    }

    /// <summary>
    /// 矢印をクリック
    /// </summary>
    /// <param name="direct">右（1）左（-1）</param>
    public void ClickArrow(int direct)
    {
        if (FadeMgr.Instance.State != FadeBase.FadeState.None)
            return;

        m_zoomObj = m_stages[Mathf.Clamp(m_zoomObj.StageNum - 1 + direct, 0, m_stages.Count - 1)];
        m_camera.StartZoomIn(m_zoomObj.transform.position);
    }

    public void ZoomOut()
    {
        if (FadeMgr.Instance.State != FadeBase.FadeState.None)
            return;

        m_uiArrow.SetActive(false);
        m_uiStartBack.SetActive(false);
        m_camera.StartZoomOut();

        m_zoomObj = null;   // 
    }

    public void SceneLoad()
    {
        if (FadeMgr.Instance.State != FadeBase.FadeState.None)
            return;

        if (int.Parse(ms_saveData.data[m_zoomObj.StageNum - 1]) > 0)
        {
            m_camera.StartZoomFade(m_zoomObj.transform.position);
            m_tryStage = ms_selectStage = m_zoomObj.StageNum;
            // ステージセレクト→ゲーム のフェード
            FadeMgr.Instance.StartFade(FadeMgr.FadeType.Scale, ProcessedtParameter.Game_Scene.GAME_MAIN);
        }
    }
}
