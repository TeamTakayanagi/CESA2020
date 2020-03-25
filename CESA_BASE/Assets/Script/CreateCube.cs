using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateCube : MonoBehaviour
{
   // LinkedList<GameObject> m_cube = new LinkedList<GameObject>();
    //GameObject[] cubes;
    LinkedList<Cube> m_cubeList = new LinkedList<Cube>();
    List<Vector3> m_outPos = new List<Vector3>();
    [SerializeField]
    Cube cubePrefab;
    Vector3 mousePos;
    float MaxPosX = 5.0f;
    float MinPosX = -4.0f;
    float MaxPosY = 5.0f;
    float MinPosY = -4.0f;

    // 親の回転角度が欲しい！！
    Transform m_parentTrans = null;

    // Start is called before the first frame update
    void Awake()
    {
        gameObject.GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.25f);

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
        // マウス座標をワールド座標で取得
        mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f);
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        // 
        transform.position = FindNearPosision(new Vector3(mousePos.x, mousePos.y, 0.0f));

        if (Input.GetMouseButtonDown(0))
        {
            Cube _cube = Cube.Instantiate(cubePrefab, transform.position, m_parentTrans.rotation, m_parentTrans);
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
            if (Vector3.Distance(nearObj.transform.position, mousePos) < Vector3.Distance(cube.transform.position, mousePos))
                continue;

                nearObj = cube;
        }

        if(nearObj == null)
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
                    objPos = nearObj.transform.position + m_parentTrans.rotation * new Vector3(ConstDefine.ConstParameter.CubeScele, 0.0f, 0.0f);
                else
                    objPos = nearObj.transform.position - m_parentTrans.rotation * new Vector3(ConstDefine.ConstParameter.CubeScele, 0.0f, 0.0f);
            }
            // Z座標のが近い
            else
            {
                if (disY >= 0)
                    objPos = nearObj.transform.position + m_parentTrans.rotation * new Vector3(0.0f, ConstDefine.ConstParameter.CubeScele, 0.0f);
                else
                    objPos = nearObj.transform.position - m_parentTrans.rotation * new Vector3(0.0f, ConstDefine.ConstParameter.CubeScele, 0.0f);
            }
        }

        foreach (Cube cube in m_cubeList)
        {
            // 2回目以降もしくは、距離を比べて遠ければ
            if (CheakListPos(objPos) || (cube.transform.position != objPos && 
                // 画面外処理
                objPos.x <= MaxPosX && objPos.x >= MinPosX &&
                objPos.y <= MaxPosY && objPos.y >= MinPosY))
                continue;

            // 画面外、もしくは生成位置にすでにオブジェクトがあれば
#if RE
            objPos = FindNearPosision(mousePos);
            m_outPos.Add(objPos);
            break;
#else
            return transform.position;
#endif
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
