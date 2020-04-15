using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : SingletonMonoBehaviour<GameMgr>
{
    private Vector3 OUTPOS = new Vector3(-50, -50, -50);

    [SerializeField]
    private GameObject[] m_fusePrefab = null;

    [SerializeField]
    private Vector3 m_stageSizeMax = Vector3.zero;
    [SerializeField]
    private Vector3 m_stageSizeMin = Vector3.zero;
    private Vector3 m_mousePos = Vector3.zero;
    private LinkedList<Fuse> m_fieldFuse = new LinkedList<Fuse>();
    private LinkedList<Fuse> m_uiFuse = new LinkedList<Fuse>();
    private Fuse m_selectFuse = null;
    private Vector3 m_createPos = Vector3.zero;

    private CSVScript m_csvScript = null;

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
        m_createPos = OUTPOS;
        // フィールドオブジェクトの取得
        GameObject[] _cubes = GameObject.FindGameObjectsWithTag
            (Utility.TagUtility.getParentTagName(ConstDefine.TagName.Fuse));
        foreach (GameObject obj in _cubes)
        {
            Fuse _cube = obj.GetComponent<Fuse>();
            if (_cube.Type == Fuse.FuseType.UI)
                m_uiFuse.AddLast(_cube);
            else
                m_fieldFuse.AddLast(_cube);
        }

        CreateStage();

    }

    // Update is called once per frame
    void Update()
    {
        // マウス座標をワールド座標で取得
        {
            Vector3 screen = Camera.main.WorldToScreenPoint(transform.position);
            m_mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screen.z);
            m_mousePos = Camera.main.ScreenToWorldPoint(m_mousePos);
        }

        // 生成場所を取得
        m_createPos = FindNearPosision(m_mousePos);
        
        // 導火線を選択しているなら
        if (m_selectFuse)
        {
            // UI画面
            if (Input.mousePosition.x > Screen.width * 0.8f)
                m_selectFuse.BackDefault(false);
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
                    if (!m_selectFuse || m_selectFuse.gameObject != hit.collider.gameObject)
                    {
                        Fuse _cube = hit.collider.gameObject.GetComponent<Fuse>();
                        if (_cube.Type == Fuse.FuseType.UI)
                        {
                            m_selectFuse = _cube;
                            foreach (Fuse _fuse in m_uiFuse)
                                _fuse.GetComponent<Renderer>().material.color = new Color(1, 1, 0, m_selectFuse.GetComponent<Renderer>().material.color.a);

                            m_selectFuse.gameObject.GetComponent<Renderer>().material.color = new Color(1, 0, 1, m_selectFuse.GetComponent<Renderer>().material.color.a);
                        }
                    }
                    // 選択解除
                    else
                    {
                        m_selectFuse.gameObject.GetComponent<Renderer>().material.color = new Color(1, 1, 0, m_selectFuse.GetComponent<Renderer>().material.color.a);
                        m_selectFuse = null;
                    }
                }
            }
            // ゲーム画面
            else if (m_selectFuse)
            {
                m_selectFuse.gameObject.GetComponent<Renderer>().material.color = new Color(1, 1, 1, m_selectFuse.GetComponent<Renderer>().material.color.a);
                m_selectFuse.Type = Fuse.FuseType.Fuse;
                m_uiFuse.Remove(m_selectFuse);
                m_fieldFuse.AddLast(m_selectFuse);
                m_selectFuse.transform.localEulerAngles = m_selectFuse.DefaultRot;
                m_selectFuse.transform.parent = transform;
                m_selectFuse = null;
            }
        }
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

        Vector3 stageMax = StageSizeMax;
        Vector3 stageMin = StageSizeMin;

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
        m_csvScript = GetComponent<CSVScript>();
        //m_csvScript.WriteCsv();
        m_csvScript.LoadCsv();

        for (int z = 0; z < m_csvScript.Stage.Count; z++)
        {
            //for (int y = 0; y < m_csvScript.Stage[z].Count; y++)
            for (int y = 0; y < m_csvScript.Stage[z].Count; y++)
            {
                for (int x = 0; x < m_csvScript.Stage[z][y].Length; x++)
                {
                    Vector3 _pos;
                    _pos = new Vector3(x - m_csvScript.Stage[z][y].Length * 0.5f,
                                        y - m_csvScript.Stage[z].Count * 0.5f,
                                        z - m_csvScript.Stage.Count * 0.5f);

                    switch(m_csvScript.Stage[y][m_csvScript.Stage.Count - 1 - z][x][0])
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
}
