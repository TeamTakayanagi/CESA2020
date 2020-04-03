using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : SingletonMonoBehaviour<GameMgr>
{
    private Vector3 OUTPOS = new Vector3(-20, -20, -20);

    [SerializeField]
    private Vector3 m_stageSizeMax = Vector3.zero;
    [SerializeField]
    private Vector3 m_stageSizeMin = Vector3.zero;
    private Vector3 m_mousePos = Vector3.zero;
    private LinkedList<Fuse> m_fieldFuse = new LinkedList<Fuse>();
    private LinkedList<Fuse> m_uiFuse = new LinkedList<Fuse>();
    private Fuse m_selectFuse = null;
    private Vector3 m_createPos = Vector3.zero;

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
        GameObject[] _cubes = GameObject.FindGameObjectsWithTag(ConstDefine.TagName.Fuse);
        foreach (GameObject obj in _cubes)
        {
            Fuse _cube = obj.GetComponent<Fuse>();
            if (_cube.Type == Fuse.FuseType.UI)
                m_uiFuse.AddLast(_cube);
            else
                m_fieldFuse.AddLast(_cube);

        }
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

        if (m_selectFuse)
        {
            // UI
            if (Input.mousePosition.x > Screen.width * 0.8f)
                m_selectFuse.BackDefault(false);
            // ゲーム
            else
                m_selectFuse.transform.position = m_createPos;
        }

        // キューブ探索
        if (Input.GetMouseButtonDown(0))
        {
            // UI
            if (Input.mousePosition.x > Screen.width * 0.8f)
            {
                // サブカメラ取得
                RaycastHit hit = new RaycastHit();
                Ray ray = GameObject.FindGameObjectWithTag(ConstDefine.TagName.UICamera).GetComponent<Camera>().
                    ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    if (!m_selectFuse || m_selectFuse.gameObject != hit.collider.gameObject)
                    {
                        Fuse _cube = hit.collider.gameObject.GetComponent<Fuse>();
                        if (_cube.Type == Fuse.FuseType.UI)
                        {
                            m_selectFuse = _cube;
                            foreach (Fuse _fuse in m_uiFuse)
                                _fuse.GetComponent<Renderer>().material.color = Color.yellow;

                            m_selectFuse.gameObject.GetComponent<Renderer>().material.color = Color.cyan;
                        }
                    }
                    else
                    {
                        m_selectFuse.gameObject.GetComponent<Renderer>().material.color = Color.yellow;
                        m_selectFuse = null;
                    }
                }
            }
            // ゲーム
            else if(m_selectFuse)
            {
                m_selectFuse.gameObject.GetComponent<Renderer>().material.color = Color.white;
                m_selectFuse.Type = Fuse.FuseType.Fuse;
                m_uiFuse.Remove(m_selectFuse);
                m_fieldFuse.AddLast(m_selectFuse);
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
        foreach (Fuse cube in m_fieldFuse)
        {
            // 2回目以降もしくは、距離を比べて遠ければ
            if (Vector3.Distance(nearObj.transform.position, mousePos) <
                Vector3.Distance(cube.transform.position, mousePos))
                continue;

            nearObj = cube;
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

        foreach (Fuse cube in m_fieldFuse)
        {
            // 2回目以降もしくは、距離を比べて遠ければ
            if (cube.transform.position != objPos &&

            // 画面外処理
             objPos.x <= stageMax.x && objPos.x >= stageMin.x &&
             objPos.y <= stageMax.y && objPos.y >= stageMin.y &&
             objPos.z <= stageMax.z && objPos.z >= stageMin.z)
                continue;

            return m_createPos;
        }
        return objPos;
    }
}
