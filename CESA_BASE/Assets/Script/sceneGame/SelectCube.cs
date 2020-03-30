using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCube : MonoBehaviour
{
    LinkedList<Cube> m_cubeList = new LinkedList<Cube>();
    List<Vector3> m_outPos = new List<Vector3>();
    [SerializeField]
    Cube cubePrefab;
    Vector3 mousePos;
    float MaxPosX = 5.0f;
    float MinPosX = -4.0f;
    float MaxPosY = 5.0f;
    float MinPosY = -4.0f;
    Vector3 m_v3SaveMouse = Vector3.zero;
    Vector3 m_saveRot;

    // 親の回転角度が欲しい！！
    Transform m_parentTrans = null;

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
        m_parentTrans = transform.root;
    }

    // Update is called once per frame
    void Update()
    {
        // 必要なら、手間ののブロックを探索する処理
        {

        }

        // マウス座標をワールド座標で取得
        Vector3 screen =  Camera.main.WorldToScreenPoint(transform.position);
        mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screen.z);
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        // 
        transform.position = FindNearPosision(mousePos);

        // キューブ作成
        if (Input.GetMouseButtonDown(0))
        {
            Cube _cube = Instantiate(cubePrefab, transform.position, m_parentTrans.rotation, m_parentTrans);
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
            float disX, disY;
            disX = mousePos.x - nearObj.transform.position.x;
            disY = mousePos.y - nearObj.transform.position.y;

            // X座標のが大きい
            if (Mathf.Abs(disX) > Mathf.Abs(disY))
            {
                if (disX >= 0)
                    objPos = nearObj.transform.position + new Vector3(ConstDefine.ConstParameter.CUBE_SCALE, 0.0f, 0.0f);
                else
                    objPos = nearObj.transform.position - new Vector3(ConstDefine.ConstParameter.CUBE_SCALE, 0.0f, 0.0f);
            }
            // Z座標のが近い
            else
            {
                if (disY >= 0)
                    objPos = nearObj.transform.position + new Vector3(0.0f, ConstDefine.ConstParameter.CUBE_SCALE, 0.0f);
                else
                    objPos = nearObj.transform.position - new Vector3(0.0f, ConstDefine.ConstParameter.CUBE_SCALE, 0.0f);
            }
        }

        foreach (Cube cube in m_cubeList)
        {
            // 2回目以降もしくは、距離を比べて遠ければ
            if (cube.transform.position != objPos &&

            // 画面外処理
             (objPos.x <= MaxPosX && objPos.x >= MinPosX &&
                objPos.y <= MaxPosY && objPos.y >= MinPosY))
                continue;

            return transform.position;
        }
        return objPos;
    }

    private bool CheakListPos(Vector3 pos)
    {
        for (int i = 0; i < m_outPos.Count;  ++i)
        {
            if(m_outPos[i] == pos)
                return true;
        }
        return false;
    }

    private void AddCubeList(Cube cube)
    {
        // キューブのリストに追加
        m_cubeList.AddLast(cube);
        // 新たにキューブを設置できないポジションに追加
        m_outPos.Add(cube.transform.position);
    }
}
