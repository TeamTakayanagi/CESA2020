using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageEditerMgr : SingletonMonoBehaviour<StageEditerMgr>
{
    [SerializeField]
    private int m_stageSizeX = 0;
    [SerializeField]
    private int m_stageSizeY = 0;
    [SerializeField]
    private int m_stageSizeZ = 0;
    [SerializeField]
    private GameObject m_feildPrefab = null;
    [SerializeField]
    private GameObject m_ground = null;

    private Fuse m_selectFuse = null;
    private GameObject m_cursorTouchObj = null;
    private Vector3 m_cameraPos = Vector3.zero;
    private Quaternion m_cameraRot = Quaternion.identity;
    private Vector3 m_createRot = Vector3.zero;
    private InputField m_inputField = null;
    private CSVScript m_CSVScript = null;

    private string[,,] m_stage = null;
    //private LinkedList<LinkedList<LinkedList<string>>> m_stageList = new LinkedList<LinkedList<LinkedList<string>>>();
    private List<Vector3> m_stagePos = new List<Vector3>();
    private List<string> m_stageType = new List<string>();
    private List<List<string[]>> m_stageLList = new List<List<string[]>>();

    //private void Awake()
    //{
    //}

    // Start is called before the first frame update
    void Start()
    {
        m_ground.SetActive(false);
        // UIの導火線仮選択
        m_selectFuse = GameObject.FindGameObjectWithTag(StringDefine.TagName.Fuse).GetComponent<Fuse>();
        m_selectFuse.GetComponent<Renderer>().material.SetColor("_Color", Color.cyan);

        // カメラの初期情報保存
        m_cameraPos = Camera.main.transform.position;
        m_cameraRot = Camera.main.transform.rotation;

        m_inputField = GameObject.FindGameObjectWithTag("Button/Input").GetComponent<InputField>();
        m_inputField.text = "StageDataXX";
        m_CSVScript = GetComponent<CSVScript>();

        // 空ボックス生成
        CreateStage();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit = new RaycastHit();
        Ray ray;

        // UI部分 
        if (Input.mousePosition.x > Screen.width * 0.8f)
        {
            ray = GameObject.FindGameObjectWithTag(StringDefine.TagName.UICamera).GetComponent<Camera>().
                    ScreenPointToRay(Input.mousePosition);

            // 設置場所を選択
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.transform.parent.tag == StringDefine.TagName.Fuse)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (m_selectFuse)
                            m_selectFuse.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
                        m_selectFuse = hit.collider.transform.parent.GetComponent<Fuse>();
                        m_selectFuse.GetComponent<Renderer>().material.SetColor("_Color", Color.cyan);
                    }
                }
            }
        }
        // ゲーム部分
        else
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // 設置場所を選択
            if (Physics.Raycast(ray, out hit))
            {
                // 空ボックスなら
                if (hit.collider.tag == StringDefine.TagName.Player)
                {
                    // 導火線設置
                    if (Input.GetMouseButtonDown(0) && m_selectFuse)
                    {
                        // その場所にまだ導火線がないなら
                        Fuse selectClone = null;
                        selectClone = Instantiate(m_selectFuse);      // 複製
                        selectClone.transform.position = hit.collider.transform.position;
                        selectClone.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
                        selectClone.Type = Fuse.FuseType.Fuse;
                        selectClone.transform.localEulerAngles = m_createRot;
                        selectClone.transform.parent = transform;

                        // ステージ配列に情報追加
                        Vector3 _pos = hit.transform.position;
                        
                        switch (m_selectFuse.tag)
                        {
                            case ConstDefine.Fuse.FuseI:
                                m_stagePos.Add(_pos);
                                m_stageType.Add("Fuse-I000");
                                break;

                            case ConstDefine.Fuse.FuseL:
                                m_stagePos.Add(_pos);
                                m_stageType.Add("Fuse-L000");
                                break;

                            case ConstDefine.Fuse.FuseT:
                                m_stagePos.Add(_pos);
                                m_stageType.Add("Fuse-T000");
                                break;

                            case ConstDefine.Fuse.FuseX:
                                m_stagePos.Add(_pos);
                                m_stageType.Add("Fuse-X000");
                                break;

                            case ConstDefine.Fuse.FuseLL:
                                m_stagePos.Add(_pos);
                                m_stageType.Add("FuseLL000");
                                break;

                            case ConstDefine.Fuse.FuseTT:
                                m_stagePos.Add(_pos);
                                m_stageType.Add("FuseTT000");
                                break;

                            case ConstDefine.Fuse.FuseXX:
                                m_stagePos.Add(_pos);
                                m_stageType.Add("FuseXX000");
                                break;

                        }

                        Destroy(hit.collider.gameObject);
                    }
                    // 設置位置の色の変更
                    else
                    {
                        if (m_cursorTouchObj)
                            m_cursorTouchObj.GetComponent<Renderer>().material.SetColor("_Color", Color.white);

                        m_cursorTouchObj = hit.collider.gameObject;
                        m_cursorTouchObj.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                    }
                }
                // 設置済みの導火線にタッチしているなら
                else if (hit.collider.transform.parent.tag == StringDefine.TagName.Fuse)
                {
                    // 導火線設置
                    if (Input.GetMouseButtonDown(0))
                    {
                        // 削除
                        GameObject obj = Instantiate(m_feildPrefab, hit.collider.transform.parent.position, Quaternion.identity);
                        obj.transform.parent = transform.GetChild(0);
                        obj.transform.tag = StringDefine.TagName.Player;
                        Destroy(hit.collider.transform.parent.gameObject);

                        // 削除情報で上書き
                        Vector3 _pos = hit.transform.position;
                        m_stagePos.Add(_pos);
                        m_stageType.Add("FuseXX000");
                    }
                    // 設置位置の色の変更
                    else
                    {
                        if (m_cursorTouchObj)
                            m_cursorTouchObj.GetComponent<Renderer>().material.SetColor("_Color", Color.white);

                        m_cursorTouchObj = hit.collider.transform.parent.gameObject;
                        m_cursorTouchObj.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
                    }
                }
            }
        }


    }

    // 空のボックスを用いてステージの枠を生成
    void CreateStage()
    {
        GameObject[] _objList = GameObject.FindGameObjectsWithTag(StringDefine.TagName.Player);
        int difference = m_stageSizeX * m_stageSizeY * m_stageSizeZ - _objList.Length;
        m_ground.transform.localPosition = new Vector3(m_ground.transform.localPosition.x, -Mathf.Ceil(m_stageSizeY / 2), m_ground.transform.localPosition.z);

        // 変更後のほうが設置可能数が多い（同数含む）なら
        if (difference >= 0)
        {
            Vector3 half = new Vector3(Mathf.Ceil(m_stageSizeX / 2), Mathf.Ceil(m_stageSizeY / 2), Mathf.Ceil(m_stageSizeZ / 2));
            for (int z = 0; z < m_stageSizeZ; ++z)
            {
                for (int y = 0; y < m_stageSizeY; ++y)
                {
                    for (int x = 0; x < m_stageSizeX; ++x)
                    {
                        int idx = m_stageSizeY * m_stageSizeX * z + m_stageSizeX * y + x;
                        GameObject obj;
                        // 現状あるもの配置変更
                        if (_objList.Length > idx)
                        {
                            obj = _objList[idx];
                            obj.transform.position = new Vector3(x - half.x, y - half.y, z - half.z);
                        }
                        // 差分を作成
                        else
                        {
                            obj = Instantiate(m_feildPrefab, new Vector3(x - half.x, y - half.y, z - half.z), Quaternion.identity);
                            obj.transform.parent = transform.GetChild(0);
                            obj.transform.tag = StringDefine.TagName.Player;
                        }
                    }
                }
            }
        }
        // 変更前のほうが設置可能数が多いなら
        else
        {
            difference = Mathf.Abs(difference);
            // 差分を解放
            for (int i = 0; i < difference; ++i)
                Destroy(_objList[i]);

            Vector3 half = new Vector3(Mathf.Ceil(m_stageSizeX / 2), Mathf.Ceil(m_stageSizeY / 2), Mathf.Ceil(m_stageSizeZ / 2));
            for (int z = 0; z < m_stageSizeZ; ++z)
            {
                for (int y = 0; y < m_stageSizeY; ++y)
                {
                    for (int x = 0; x < m_stageSizeX; ++x)
                    {
                        // 現状あるもの配置変更
                        GameObject obj = _objList[m_stageSizeY * m_stageSizeX * z + m_stageSizeX * y + x + difference];
                        obj.transform.position = new Vector3(x - half.x, y - half.y, z - half.z);
                    }
                }
            }
        }
    }

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// ボタンの処理

    // 実際のステージを確認
    public void ViewPlayStage()
    {
        GameObject[] _objList = GameObject.FindGameObjectsWithTag(StringDefine.TagName.Player);
        bool isSet = _objList[0].GetComponent<MeshRenderer>().enabled ^ true;

        AllFuseDefault();
        foreach (GameObject _obj in _objList)
        {
            _obj.GetComponent<MeshRenderer>().enabled = isSet;
        }
        m_ground.SetActive(!isSet);
        CameraDefault();
    }

    // カメラを初期位置に戻す
    public void CameraDefault()
    {
        Camera.main.transform.position = m_cameraPos;
        Camera.main.transform.rotation = m_cameraRot;
    }

    public void AllFuseDefault()
    {
        GameObject[] _fuseList = GameObject.FindGameObjectsWithTag(StringDefine.TagName.Fuse);
        foreach (GameObject _fuse in _fuseList)
        {
            _fuse.GetComponent<MeshRenderer>().enabled = true;
            _fuse.gameObject.layer = 0;
            for (int i = 0; i < _fuse.transform.childCount; ++i)
            {
                GameObject child = _fuse.transform.GetChild(i).gameObject;
                MeshRenderer mesh = child.GetComponent<MeshRenderer>();
                if (mesh)
                    mesh.enabled = true;
            }
        }

        GameObject[] _boxList = GameObject.FindGameObjectsWithTag(StringDefine.TagName.Player);
        foreach (GameObject _box in _boxList)
        {
            _box.GetComponent<MeshRenderer>().enabled = true;
            _box.gameObject.layer = 0;
        }
    }

    // ステージ保存
    public void StageSave()
    {
        if (m_inputField.text == null) return;

        // 初期化
        for (int z = 0; z < m_stageSizeZ; z++)
        {
            m_stageLList.Add(new List<string[]>());
            for (int y = 0; y < m_stageSizeY; y++)
            {
                m_stageLList[z].Add(new string[m_stageSizeY]);
                for (int x = 0; x < m_stageSizeX; x++)
                {
                    m_stageLList[z][y][x] = "---------";
                }
            }
        }

        for (int i = 0; i < m_stagePos.Count; i++)
        {
            Vector3 _pos = m_stagePos[i] + new Vector3(m_stageSizeX, m_stageSizeY, m_stageSizeZ) / 2;
            //m_stage[(int)_pos.x, (int)_pos.y, (int)_pos.z] = m_stageType[i];
            m_stageLList[(int)_pos.z][(int)_pos.y][(int)_pos.x] = m_stageType[i];
        }
        
        m_CSVScript.WriteCsv(m_stageLList, m_inputField.text, m_stageSizeY);
    }

    public void CutBoxR()
    {
        CutBox((int)RayPoint.PointPlace.right);
    }       
    public void CutBoxU()
    {
        CutBox((int)RayPoint.PointPlace.up);
    }
    public void CutBoxL()
    {
        CutBox((int)RayPoint.PointPlace.left);
    }
    public void CutBoxB()
    {
        CutBox((int)RayPoint.PointPlace.bottm);
    }

    public void CutBox(int rayPlace)
    {
        bool[] isCheak = { false, false, false };
        RaycastHit hit = new RaycastHit();
        GameObject rayPoint = Camera.main.transform.GetChild(rayPlace).gameObject;
        Ray ray = new Ray(rayPoint.transform.position, -rayPoint.transform.position);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.transform.tag == StringDefine.TagName.Player ||
                hit.collider.transform.tag == StringDefine.TagName.Fuse)
            {
                Vector3 hitPos = hit.collider.transform.position;
                Vector3 difference = rayPoint.transform.position - hitPos;
                difference = new Vector3(Mathf.Abs(difference.x), Mathf.Abs(difference.y), Mathf.Abs(difference.z));
                float maxValue = Mathf.Max(difference.x, difference.y, difference.z);
                if (!isCheak[0] && difference.x == maxValue)
                    isCheak[0] = true;
                else if (!isCheak[1] && difference.y == maxValue)
                    isCheak[1] = true;
                else if (!isCheak[2] && difference.z == maxValue)
                    isCheak[2] = true;

                GameObject[] _boxList = GameObject.FindGameObjectsWithTag(StringDefine.TagName.Player);
                GameObject[] _fuseList = GameObject.FindGameObjectsWithTag(StringDefine.TagName.Fuse);
                foreach(GameObject _box in _boxList)
                {
                    if((isCheak[0] && _box.transform.position.x == hitPos.x ) ||
                       (isCheak[1] && _box.transform.position.y == hitPos.y) ||
                       (isCheak[2] && _box.transform.position.z == hitPos.z))
                    {
                        _box.GetComponent<MeshRenderer>().enabled = false;
                        _box.gameObject.layer = 2;
                    }
                }
                foreach (GameObject _fuse in _fuseList)
                {
                    if ((isCheak[0] && _fuse.transform.position.x == hitPos.x) ||
                        (isCheak[1] && _fuse.transform.position.y == hitPos.y) ||
                        (isCheak[2] && _fuse.transform.position.z == hitPos.z))
                    {
                        _fuse.GetComponent<MeshRenderer>().enabled = false;
                        _fuse.gameObject.layer = 2;
                        for(int i = 0; i < _fuse.transform.childCount; ++i)
                        {
                            GameObject child = _fuse.transform.GetChild(i).gameObject;
                            MeshRenderer mesh = child.GetComponent<MeshRenderer>();
                            if(mesh)
                              mesh.enabled = false;
                        }
                    }
                }
            }
        }
    }




/// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// 数値テキストボックスの処理

    // ステージサイズ
    public int GetStageSize(char direction)
    {
        int _stageSize = 0;
        switch (direction)
        {
            case 'X':
                _stageSize = m_stageSizeX;
                break;
            case 'Y':
                _stageSize = m_stageSizeY;
                break;
            case 'Z':
                _stageSize = m_stageSizeZ;
                break;
        }
        return _stageSize;
    }
    public void SetStageSize(char direction, int num)
    {
        switch (direction)
        {
            case 'X':
                m_stageSizeX = num;
                CreateStage();
                break;
            case 'Y':
                m_stageSizeY = num;
                CreateStage();
                break;
            case 'Z':
                m_stageSizeZ = num;
                CreateStage();
                break;
        }
    }

    // 導火線生成角度
    public int GetCreateRot(char direction)
    {
        int _stageSize = 0;
        switch (direction)
        {
            case 'X':
                _stageSize = (int)m_createRot.x;
                break;
            case 'Y':
                _stageSize = (int)m_createRot.y;
                break;
            case 'Z':
                _stageSize = (int)m_createRot.z;
                break;
        }
        return _stageSize;
    }
    public void SetCreateRot(char direction, int num)
    {
        switch (direction)
        {
            case 'X':
                m_createRot.x = num;
                break;
            case 'Y':
                m_createRot.y = num;
                break;
            case 'Z':
                m_createRot.z = num;
                break;
        }

        GameObject[] _objList = GameObject.FindGameObjectsWithTag(StringDefine.TagName.Fuse);
        foreach(GameObject _obj in _objList)
        {
            Fuse _fuse = _obj.GetComponent<Fuse>();
            if(_fuse.Type == Fuse.FuseType.UI)
            {
                _fuse.transform.localEulerAngles = m_createRot;
            }
        }
    }
}
