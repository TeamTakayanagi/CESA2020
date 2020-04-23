using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageEditerMgr : SingletonMonoBehaviour<StageEditerMgr>
{
    [SerializeField]
    private GameObject m_feildPrefab = null;

    private bool m_isPreview = false;
    private Vector3 m_cameraPos = Vector3.zero;
    private Quaternion m_cameraRot = Quaternion.identity;
    private GameObject m_cursorTouchObj = null;
    private Fuse m_selectFuse = null;
    private TerrainCreate m_terrainCreate = null;
    private CSVScript m_csvScript = null;

    private List<Vector3> m_stagePos = new List<Vector3>();
    private List<string> m_stageType = new List<string>();
    private List<List<string[]>> m_stageLList = new List<List<string[]>>();

    override protected void Awake()
    {
        Camera.main.GetComponent<MainCamera>().Control = true;
        m_isPreview = false; 
        base.Awake();
    }

    // Start is called before the first frame update
    void Start()
    {
        Fuse[] _fuseList = FindObjectsOfType<Fuse>();
        // UIの導火線仮選択
        m_selectFuse = _fuseList[0];
        m_selectFuse.GetComponent<Renderer>().material.SetColor("_Color", Color.cyan);
        // 導火線のコライダーを真四角に変更
        foreach(Fuse _fuse in _fuseList)
        {
            BoxCollider[] _box = _fuse.GetComponents<BoxCollider>();
            // 1つだけ真四角にして残し2つ目以降は削除
            _box[0].size = Vector3.one;
            _box[0].center = Vector3.zero;
            for(int i = 1; i < _box.Length; ++i)
            {
                Destroy(_box[i]);
            }

            // UI選択用のコライダーの削除
            Destroy(_fuse.transform.GetChild(_fuse.transform.childCount - 1).gameObject);
        }

        // カメラの初期情報保存
        m_cameraPos = Camera.main.transform.position;
        m_cameraRot = Camera.main.transform.rotation;

        // 地形生成オブジェクト取得
        m_terrainCreate = FindObjectOfType<TerrainCreate>();
        // CSV保存用関数
        m_csvScript = GetComponent<CSVScript>();

        // 空ボックス生成
        CreateStage();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isPreview)
            return;

        RaycastHit hit = new RaycastHit();
        Ray ray;

        // UI部分 
        if (Input.mousePosition.x > Screen.width * 0.8f)
        {
            ray = GameObject.FindGameObjectWithTag(StringDefine.TagName.SubCamera).GetComponent<Camera>().
                    ScreenPointToRay(Input.mousePosition);

            // 設置場所を選択
            if (Physics.Raycast(ray, out hit))
            {
                if (Utility.TagUtility.getParentTagName(hit.collider.tag) == StringDefine.TagName.Fuse)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (m_selectFuse)
                            m_selectFuse.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
                        m_selectFuse = hit.collider.GetComponent<Fuse>();
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
                        selectClone.transform.localEulerAngles = new Vector3(
                            inputFieldInt.GetInputFieldInt(inputFieldInt.FieldType.createRotX),
                            inputFieldInt.GetInputFieldInt(inputFieldInt.FieldType.createRotY),
                            inputFieldInt.GetInputFieldInt(inputFieldInt.FieldType.createRotZ));

                        selectClone.transform.parent = transform.GetChild(0);

                        // ステージ配列に情報追加
                        m_stagePos.Add(hit.transform.position);
                        
                        switch (m_selectFuse.tag)
                        {
                            case StringDefine.TagName.FuseI:
                                m_stageType.Add("Fuse-I000");
                                break;

                            case StringDefine.TagName.FuseL:
                                m_stageType.Add("Fuse-L000");
                                break;

                            case StringDefine.TagName.FuseT:
                                m_stageType.Add("Fuse-T000");
                                break;

                            case StringDefine.TagName.FuseX:
                                m_stageType.Add("Fuse-X000");
                                break;

                            case StringDefine.TagName.FuseLL:
                                m_stageType.Add("FuseLL000");
                                break;

                            case StringDefine.TagName.FuseTT:
                                m_stageType.Add("FuseTT000");
                                break;

                            case StringDefine.TagName.FuseXX:
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
                else if (Utility.TagUtility.getParentTagName(hit.collider.tag) == StringDefine.TagName.Fuse)
                {
                    // 導火線設置
                    if (Input.GetMouseButtonDown(0))
                    {
                        // 削除
                        GameObject obj = Instantiate(m_feildPrefab, hit.collider.transform.position, Quaternion.identity);
                        obj.transform.parent = transform.GetChild(0);
                        obj.transform.tag = StringDefine.TagName.Player;
                        Destroy(hit.collider.gameObject);

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

                        m_cursorTouchObj = hit.collider.gameObject;
                        m_cursorTouchObj.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 空のボックスを用いてステージの枠を生成
    /// </summary>
    void CreateStage()
    {
        int _stageSizeX = inputFieldInt.GetInputFieldInt(inputFieldInt.FieldType.stageSizeX);
        int _stageSizeY = inputFieldInt.GetInputFieldInt(inputFieldInt.FieldType.stageSizeY);
        int _stageSizeZ = inputFieldInt.GetInputFieldInt(inputFieldInt.FieldType.stageSizeZ);

        if (_stageSizeX == ProcessedtParameter.System_Constant.ERROR_INT ||
            _stageSizeY == ProcessedtParameter.System_Constant.ERROR_INT ||
            _stageSizeZ == ProcessedtParameter.System_Constant.ERROR_INT)
            return;

        GameObject[] _objList = GameObject.FindGameObjectsWithTag(StringDefine.TagName.Player);
        int difference = _objList.Length - _stageSizeX * _stageSizeY * _stageSizeZ;

        // 変更後のほうが設置可能数が多い（同数含む）なら
        if (difference <= 0)
        {
            Vector3 half = new Vector3(Mathf.Ceil(_stageSizeX / 2), Mathf.Ceil(_stageSizeY / 2), Mathf.Ceil(_stageSizeZ / 2));
            for (int z = 0; z < _stageSizeZ; ++z)
                for (int y = 0; y < _stageSizeY; ++y)
                    for (int x = 0; x < _stageSizeX; ++x)
                    {
                        int idx = _stageSizeY * _stageSizeX * z + _stageSizeX * y + x;
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
                            obj.layer = StringDefine.Layer.Trans;
                        }
                    }
        }
        // 変更前のほうが設置可能数が多いなら
        else
        {
            // 差分を解放
            for (int i = 0; i < difference; ++i)
                Destroy(_objList[i]);

            Vector3 half = new Vector3(Mathf.Ceil(_stageSizeX / 2), Mathf.Ceil(_stageSizeY / 2), Mathf.Ceil(_stageSizeZ / 2));
            for (int z = 0; z < _stageSizeZ; ++z)
                for (int y = 0; y < _stageSizeY; ++y)
                    for (int x = 0; x < _stageSizeX; ++x)
                    {
                        // 現状あるもの配置変更
                        GameObject obj = _objList[_stageSizeY * _stageSizeX * z + _stageSizeX * y + x + difference];
                        obj.transform.position = new Vector3(x - half.x, y - half.y, z - half.z);
                    }
        }
    }

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// ボタンの処理

    /// <summary>
    /// 実際のステージを確認
    /// </summary>
    public void ViewPlayStage()
    {
        GameObject[] _objList = GameObject.FindGameObjectsWithTag(StringDefine.TagName.Player);

        AllFuseDefault();
        foreach (GameObject _obj in _objList)
        {
            _obj.GetComponent<MeshRenderer>().enabled = m_isPreview;
        }

        // カメラをもとの位置に戻す 
        CameraDefault();
        int _stageSizeX = inputFieldInt.GetInputFieldInt(inputFieldInt.FieldType.stageSizeX);
        int _stageSizeY = inputFieldInt.GetInputFieldInt(inputFieldInt.FieldType.stageSizeY);
        int _stageSizeZ = inputFieldInt.GetInputFieldInt(inputFieldInt.FieldType.stageSizeZ);
        if (_stageSizeX == ProcessedtParameter.System_Constant.ERROR_INT ||
            _stageSizeY == ProcessedtParameter.System_Constant.ERROR_INT ||
            _stageSizeZ == ProcessedtParameter.System_Constant.ERROR_INT)
            return;

        // 地形生成
        if (!m_isPreview)
        {
            m_terrainCreate.gameObject.SetActive(true);
            m_terrainCreate.transform.GetChild((int)TerrainCreate.TerrainChild.Wall).gameObject.SetActive(true);
            m_terrainCreate.CreateGround(_stageSizeX, _stageSizeZ, -Mathf.Ceil(_stageSizeY / 2) - 1);
            m_terrainCreate.CreateWall();
        }
        else
        {
            m_terrainCreate.gameObject.SetActive(false);
        }
        m_isPreview ^= true;
    }

    /// <summary>
    /// カメラを初期位置に戻す
    /// </summary>
    public void CameraDefault()
    {
        Camera.main.transform.position = m_cameraPos;
        Camera.main.transform.rotation = m_cameraRot;
    }

    /// <summary>
    /// 削ったステージをもとに戻す
    /// </summary>
    public void AllFuseDefault()
    {
        GameObject _stage = transform.GetChild(0).gameObject;
        for (int i = 0; i < _stage.transform.childCount; ++i)
        {
            GameObject _stageObj = _stage.transform.GetChild(i).gameObject;
            _stageObj.GetComponent<MeshRenderer>().enabled = true;

            Fuse _fuse = _stageObj.GetComponent<Fuse>();
            if (!_fuse)
            {
                _stageObj.gameObject.layer = StringDefine.Layer.Trans;
                continue;
            }

            _stageObj.gameObject.layer = StringDefine.Layer.Default;

            for (int j = 0; j < _fuse.transform.childCount; ++j)
            {
                GameObject child = _fuse.transform.GetChild(j).gameObject;
                MeshRenderer mesh = child.GetComponent<MeshRenderer>();
                if (mesh)
                    mesh.enabled = true;
            }
        }
    }

    /// <summary>
    /// ステージ保存
    /// </summary>
    public void StageSave()
    {
        int _stageSizeX = inputFieldInt.GetInputFieldInt(inputFieldInt.FieldType.stageSizeX);
        int _stageSizeY = inputFieldInt.GetInputFieldInt(inputFieldInt.FieldType.stageSizeY);
        int _stageSizeZ = inputFieldInt.GetInputFieldInt(inputFieldInt.FieldType.stageSizeZ);

        if (_stageSizeX == ProcessedtParameter.System_Constant.ERROR_INT ||
            _stageSizeY == ProcessedtParameter.System_Constant.ERROR_INT ||
            _stageSizeZ == ProcessedtParameter.System_Constant.ERROR_INT)
            return;

        // 初期化
        for (int z = 0; z < _stageSizeZ; z++)
        {
            m_stageLList.Add(new List<string[]>());
            for (int y = 0; y < _stageSizeY; y++)
            {
                m_stageLList[z].Add(new string[_stageSizeY]);
                for (int x = 0; x < _stageSizeX; x++)
                {
                    m_stageLList[z][y][x] = "---------";
                }
            }
        }

        for (int i = 0; i < m_stagePos.Count; i++)
        {
            Vector3 _pos = m_stagePos[i] + new Vector3(_stageSizeX, _stageSizeY, _stageSizeZ) / 2;
            m_stageLList[(int)_pos.z][(int)_pos.y][(int)_pos.x] = m_stageType[i];
        }
        string stageName = "Stage";
        m_csvScript.WriteCsv(m_stageLList, stageName, _stageSizeY);
    }

    /// <summary>
    /// ステージの表面を削除
    /// </summary>
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

    /// <summary>
    ///  ステージの表面を削除（上記の関数から呼び出し）
    /// </summary>
    /// <param name="rayPlace">rayを飛ばしている場所がカメラから見てどの向きにあるか</param>
    public void CutBox(int rayPlace)
    {
        RaycastHit hit = new RaycastHit();
        GameObject rayPoint = Camera.main.transform.GetChild(rayPlace).gameObject;
        Ray ray = new Ray(rayPoint.transform.position, -rayPoint.transform.position);

        if (!Physics.Raycast(ray, out hit) ||
            (hit.collider.transform.tag != StringDefine.TagName.Player &&
                Utility.TagUtility.getParentTagName(hit.collider.transform.tag) != StringDefine.TagName.Fuse))
            return;

        float maxValue;
        Vector3 cheak = new Vector3Int(0, 0, 0);
        Vector3 hitPos = hit.collider.transform.position;
        Vector3 difference = rayPoint.transform.position - hitPos;

        difference = new Vector3(Mathf.Abs(difference.x), Mathf.Abs(difference.y), Mathf.Abs(difference.z));
        maxValue = Mathf.Max(difference.x, difference.y, difference.z);

        // 最大値を出している方向を調査
        cheak = new Vector3(maxValue, maxValue, maxValue) - difference;

        GameObject _stage = transform.GetChild(0).gameObject;
        for (int i = 0; i < _stage.transform.childCount; ++i)
        {
            GameObject _stageObj = _stage.transform.GetChild(i).gameObject;
            if ((cheak.x == 0.0f && _stageObj.transform.position.x == hitPos.x) ||
                (cheak.y == 0.0f && _stageObj.transform.position.y == hitPos.y) ||
                (cheak.z == 0.0f && _stageObj.transform.position.z == hitPos.z))
            {
                _stageObj.GetComponent<MeshRenderer>().enabled = false;
                _stageObj.gameObject.layer = StringDefine.Layer.Ignore;
                Fuse _fuse = _stageObj.GetComponent<Fuse>();
                if (_fuse)
                {
                    for (int j = 0; j < _fuse.transform.childCount; ++j)
                    {
                        GameObject child = _fuse.transform.GetChild(j).gameObject;
                        MeshRenderer mesh = child.GetComponent<MeshRenderer>();
                        if (mesh)
                            mesh.enabled = false;
                    }
                }
            }
        }

    }

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  イベント処理

    /// <summary>
    /// 他のオブジェクトの変数の変更毎に呼び出し
    /// </summary>

    public void StageSize()
    {
        int _stageSizeX = inputFieldInt.GetInputFieldInt(inputFieldInt.FieldType.stageSizeX);
        int _stageSizeY = inputFieldInt.GetInputFieldInt(inputFieldInt.FieldType.stageSizeY);
        int _stageSizeZ = inputFieldInt.GetInputFieldInt(inputFieldInt.FieldType.stageSizeZ);

        if (_stageSizeX == ProcessedtParameter.System_Constant.ERROR_INT ||
            _stageSizeY == ProcessedtParameter.System_Constant.ERROR_INT ||
            _stageSizeZ == ProcessedtParameter.System_Constant.ERROR_INT)
            return;

        CreateStage();
    }

    public void CreateRot()
    {
        int _createRotX = inputFieldInt.GetInputFieldInt(inputFieldInt.FieldType.createRotX);
        int _createRotY = inputFieldInt.GetInputFieldInt(inputFieldInt.FieldType.createRotY);
        int _createRotZ = inputFieldInt.GetInputFieldInt(inputFieldInt.FieldType.createRotZ);

        if (_createRotX == ProcessedtParameter.System_Constant.ERROR_INT ||
            _createRotY == ProcessedtParameter.System_Constant.ERROR_INT ||
            _createRotZ == ProcessedtParameter.System_Constant.ERROR_INT)
            return;

        Fuse[] _fuseList = FindObjectsOfType<Fuse>();
        foreach(Fuse _fuse in _fuseList)
        {
            if (_fuse.Type != Fuse.FuseType.UI)
                continue;

            _fuse.transform.localEulerAngles = new Vector3(
                            inputFieldInt.GetInputFieldInt(inputFieldInt.FieldType.createRotX),
                            inputFieldInt.GetInputFieldInt(inputFieldInt.FieldType.createRotY),
                            inputFieldInt.GetInputFieldInt(inputFieldInt.FieldType.createRotZ));
        }
    }
}
