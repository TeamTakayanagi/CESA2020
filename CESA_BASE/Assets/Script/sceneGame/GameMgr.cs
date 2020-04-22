using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameMgr : SingletonMonoBehaviour<GameMgr>
{
    // デリゲート宣言
    delegate void GameStep();

    [SerializeField]
    private GameObject[] m_fusePrefab = null;
    private Texture2D cursorDefault = null;
    [SerializeField]
    private Texture2D cursorCatch = null;
    [SerializeField]
    private Vector3 m_stageSizeMax = Vector3.zero;
    [SerializeField]
    private Vector3 m_stageSizeMin = Vector3.zero;

    private LinkedList<Fuse> m_fieldFuse = new LinkedList<Fuse>();
    private LinkedList<Fuse> m_uiFuse = new LinkedList<Fuse>();
    private Fuse m_selectFuse = null;
    private Vector3 m_createPos = Vector3.zero;
    private GameStep m_gameStep = null;
    private GameObject m_saveObj = null;
    private int m_burnCount = 1;            // 燃えている導火線の数
    private int m_gameSpeed = 1;            // ゲーム加速処理
    private Vector3 OUTPOS = new Vector3(-50, -50, -50);

    private CSVScript m_csvScript = null;

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
    public Vector3 StageSizeMax
    {
        get
        {
            return m_stageSizeMax;
        }
    }
    public Vector3 StageSizeMin
    {
        get
        {
            return m_stageSizeMin;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        // マウスカーソル用の画像をデフォルトに変更
        Cursor.SetCursor(cursorDefault, Vector2.zero, CursorMode.Auto);

        m_createPos = OUTPOS;
        // 開始演出準備
        GameObject canvas = GameObject.FindGameObjectWithTag(ConstDefine.TagName.UICanvas);
        m_saveObj = canvas.transform.GetChild(0).gameObject;
        m_gameStep = GameStart;

        // フィールドオブジェクトの取得
        GameObject[] _fuseList = GameObject.FindGameObjectsWithTag
            (Utility.TagUtility.getParentTagName(ConstDefine.TagName.Fuse));
        foreach (GameObject _fuse in _fuseList)
        {
            Fuse _cube = _fuse.GetComponent<Fuse>();
            if (_cube.Type == Fuse.FuseType.UI)
                m_uiFuse.AddLast(_cube);
            else
                m_fieldFuse.AddLast(_cube);
        }

        m_csvScript = GameObject.FindGameObjectWithTag(ConstDefine.TagName.SceneMgr).GetComponent<CSVScript>();
        CreateStage();

        // スタート演出のため導火線の更新処理停止（ステージエディタ完成後修正予定）
        _cube.enabled = false;
    }
    //Sound.Instance.PlayBGM(ConstDefine.Audio.BGM.GameMain);

    // Update is called once per frame
    void Update()
    {
        m_gameStep();
    }

    // スタート演出
    void GameStart()
    {
        Number number = m_saveObj.GetComponent<Number>();
        if (number.TexCount < 0)
        {
            Destroy(m_saveObj);
            m_gameStep = GameMain;
            // 導火線の更新処理再開
            GameObject[] _cubes = GameObject.FindGameObjectsWithTag
                (Utility.TagUtility.getParentTagName(ConstDefine.TagName.Fuse));
            foreach (GameObject obj in _cubes)
            {
                Fuse _cube = obj.GetComponent<Fuse>();
                _cube.enabled = true;
            }
        }
    }

    // ゲームメイン処理
    void GameMain()
    {
        // マウス座標をワールド座標で取得
        Vector3 mousePos = Vector3.zero;
        Vector3 screen = Camera.main.WorldToScreenPoint(transform.position);
        mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screen.z);
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        // 生成場所を取得
        m_createPos = FindNearPosision(mousePos);

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
                Ray ray = GameObject.FindGameObjectWithTag(ConstDefine.TagName.UICamera).GetComponent<Camera>().
                    ScreenPointToRay(Input.mousePosition);

                // 導火線を選択
                if (Physics.Raycast(ray, out hit))
                {
                    // 新規選択
                    if (!m_selectFuse || m_selectFuse.gameObject != hit.collider.transform.parent.gameObject)
                    {
                        Fuse _cube = hit.collider.transform.parent.GetComponent<Fuse>();
                        if (!_cube)
                            return;
                        if (_cube.Type == Fuse.FuseType.UI)
                        {
                            m_selectFuse = _cube;
                            foreach (Fuse _fuse in m_uiFuse)
                                _fuse.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);

                            m_selectFuse.GetComponent<Renderer>().material.SetColor("_Color", Color.cyan);
                            // マウスカーソル用の画像を選択時に変更
                            Cursor.SetCursor(cursorCatch, Vector2.zero, CursorMode.Auto);
                        }
                    }
                    // 選択解除
                    else
                    {
                        m_selectFuse.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
                        m_selectFuse = null;
                        // マウスカーソル用の画像をデフォルトに変更
                        Cursor.SetCursor(cursorDefault, Vector2.zero, CursorMode.Auto);
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
                    m_uiFuse.Remove(m_selectFuse);
                    m_fieldFuse.AddLast(m_selectFuse);
                    m_selectFuse.transform.localEulerAngles = m_selectFuse.DefaultRot;
                    m_selectFuse.transform.parent = transform;

                    // UI選択用の子供オブジェクトを削除
                    GameObject chid = m_selectFuse.transform.GetChild(m_selectFuse.transform.childCount - 1).gameObject;
                    Destroy(chid);

                    m_selectFuse = null;
                    m_createPos = OUTPOS;
                    // マウスカーソル用の画像をデフォルトに変更
                    Cursor.SetCursor(cursorDefault, Vector2.zero, CursorMode.Auto);
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

    // ゲームクリア処理
    void GameClear()
    {

    }

    private Vector3 FindNearPosision(Vector3 mousePos)
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
                    objPos = nearObj.transform.position + new Vector3(ConstDefine.ConstParameter.CUBE_SCALE, 0.0f, 0.0f);
                else
                    objPos = nearObj.transform.position - new Vector3(ConstDefine.ConstParameter.CUBE_SCALE, 0.0f, 0.0f);
            }
            // Y座標のが近い
            else if (Mathf.Abs(disY) > Mathf.Abs(disZ))
            {
                if (disY >= 0)
                    objPos = nearObj.transform.position + new Vector3(0.0f, ConstDefine.ConstParameter.CUBE_SCALE, 0.0f);
                else
                    objPos = nearObj.transform.position - new Vector3(0.0f, ConstDefine.ConstParameter.CUBE_SCALE, 0.0f);
            }
            // Z座標のが近い
            else
            {
                if (disZ >= 0)
                    objPos = nearObj.transform.position + new Vector3(0.0f, 0.0f, ConstDefine.ConstParameter.CUBE_SCALE);
                else
                    objPos = nearObj.transform.position - new Vector3(0.0f, 0.0f, ConstDefine.ConstParameter.CUBE_SCALE);
            }
        }

        Vector3 stageMax = m_stageSizeMax;
        Vector3 stageMin = m_stageSizeMin;

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

    private void CreateStage()
    {
        //m_csvScript.WriteCsv();
        m_csvScript.LoadCsv();

        for (int z = 0; z < m_csvScript.Stage.Count; z++)
        {
            for (int y = 0; y < m_csvScript.Stage[z].Count; y++)
            {
                for (int x = 0; x < m_csvScript.Stage[z][y].Length; x++)
                {
                    Vector3 _pos;
                    _pos = new Vector3(x - m_csvScript.Stage[z][y].Length * 0.5f,
                                        z - m_csvScript.Stage.Count * 0.5f,
                                        y - m_csvScript.Stage[z].Count * 0.5f);

                    switch (m_csvScript.Stage[z][m_csvScript.Stage[z].Count - 1 - y][x][0])
                    {
                        // I字の導火線
                        case 'A':
                            Instantiate(m_fusePrefab[0], _pos, Quaternion.identity, transform);
                            break;

                        // L字の導火線
                        case 'B':
                            Instantiate(m_fusePrefab[1], _pos, Quaternion.identity, transform);
                            break;

                        default:
                            break;
                    }
                }
            }
        }
    }

    public void BurnOutFuse(Fuse _fuse)
    {
        m_fieldFuse.Remove(_fuse);
        if (_fuse.Type == Fuse.FuseType.Goal)
        {
            m_gameStep = GameClear;
        }
        m_burnCount--;
        if (m_burnCount <= 0)
        {
            SceneManager.LoadScene(ConstDefine.Scene.Clear);
        }
    }
}
