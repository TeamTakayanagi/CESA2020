using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCube : MonoBehaviour
{
    [SerializeField]
    Cube cubePrefab;
    LinkedList<Cube> m_cubeList = new LinkedList<Cube>();
    Vector3 mousePos;

    // Start is called before the first frame update
    void Awake()
    {
        gameObject.GetComponent<Renderer>().material.color = Color.green;

        // フィールドオブジェクトの取得
        GameObject[] _cubes = GameObject.FindGameObjectsWithTag(ConstDefine.TagName.Player);
        foreach(GameObject obj in _cubes)
        {
            Cube _cube = obj.GetComponent<Cube>();
            AddCubeList(_cube);
        }
    }

    private void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // マウス座標をワールド座標で取得
        {
            Vector3 screen = Camera.main.WorldToScreenPoint(transform.position);
            mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screen.z);
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        }

        // 生成場所を取得
        transform.position = FindNearPosision(mousePos);

        // キューブ作成
        if (Input.GetMouseButtonDown(0))
        {
            Cube _cube = Instantiate(cubePrefab, transform.position, Quaternion.identity);
            _cube.transform.parent = transform.parent;
            AddCubeList(_cube);
        }
    }

    private Vector3 FindNearPosision(Vector3 mousePos)
    {
        // 一番近くにあるオブジェクト探索用変数
        Cube nearObj = m_cubeList.First.Value;
        Vector3 objPos = Vector3.zero;

        // マウスのワールド座標に一番近いオブジェクトを取得
        foreach (Cube cube in m_cubeList)
        {
            // 2回目以降もしくは、距離を比べて遠ければ
            if (Vector3.Distance(nearObj.transform.position, mousePos) <
                Vector3.Distance(cube.transform.position, mousePos))
                continue;

            nearObj = cube;
        }

        if (nearObj == null)
            return Vector3.zero;

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
            else if(Mathf.Abs(disY) > Mathf.Abs(disZ))
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

        Vector3 stageMax = GameMgr.Instance.StageSizeMax;
        Vector3 stageMin = GameMgr.Instance.StageSizeMin;

        foreach (Cube cube in m_cubeList)
        {
            // 2回目以降もしくは、距離を比べて遠ければ
            if (cube.transform.position != objPos &&

            // 画面外処理
             objPos.x <= stageMax.x && objPos.x >= stageMin.x &&
             objPos.y <= stageMax.y && objPos.y >= stageMin.y &&
             objPos.z <= stageMax.z && objPos.z >= stageMin.z)
                continue;

            return transform.position;
        }
        return objPos;
    }

    private void AddCubeList(Cube cube)
    {
        // キューブのリストに追加
        m_cubeList.AddLast(cube);
    }
}
