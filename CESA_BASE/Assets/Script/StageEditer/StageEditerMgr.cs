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
    private GameObject m_selectObj = null;
    private TerrainCreate m_terrainCreate = null;

    private Dictionary<Vector3, string> m_fuseData = new Dictionary<Vector3, string>();

    override protected void Awake()
    {
        // デバッグログを無効化
        Debug.unityLogger.logEnabled = false;
        // カメラ操作を可能に
        Camera.main.GetComponent<MainCamera>().Control = true;
        m_isPreview = false; 
        base.Awake();
    }

    void Start()
    {

        Fuse[] _fuseList = FindObjectsOfType<Fuse>();
        // UIの導火線仮選択
        m_selectObj = _fuseList[0].gameObject;
        m_selectObj.GetComponent<Renderer>().material.SetColor("_Color", Color.cyan);
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

        // 空ボックス生成
        CreateStage();
    }

    void Update()
    {
        // プレビュー中は操作不可
        if (m_isPreview)
            return;

        RaycastHit hit = new RaycastHit();
        Ray ray;

        // UI部分 
        if (Input.mousePosition.x > Screen.width * 0.8f)
        {
            ray = GameObject.FindGameObjectWithTag(NameDefine.TagName.SubCamera).GetComponent<Camera>().
                    ScreenPointToRay(Input.mousePosition);

            // 設置場所を選択
            if (Physics.Raycast(ray, out hit))
            {
                if (Utility.TagSeparate.getParentTagName(hit.collider.tag) == NameDefine.TagName.Fuse ||
                    Utility.TagSeparate.getParentTagName(hit.collider.tag) == NameDefine.TagName.Gimmick)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (m_selectObj)
                            m_selectObj.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
                        m_selectObj = hit.collider.gameObject;
                        m_selectObj.GetComponent<Renderer>().material.SetColor("_Color", Color.cyan);
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
                if (hit.collider.tag == NameDefine.TagName.Player)
                {
                    // 導火線設置
                    if (Input.GetMouseButtonDown(0) && m_selectObj)
                    {

                        GameObject selectClone = Instantiate(m_selectObj, hit.collider.transform.position, Quaternion.identity);      // 複製
                        SetObjData(selectClone);
                        Destroy(hit.collider.gameObject);
                    }
                    // 設置位置の色の変更
                    else
                    {
                        if (m_cursorTouchObj)
                            m_cursorTouchObj.GetComponent<Renderer>().material.SetColor("_Color", Color.white);

                        m_cursorTouchObj = hit.collider.gameObject;
                        m_cursorTouchObj.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
                    }
                }
                // 設置済みの導火線にタッチしているなら
                else if (Utility.TagSeparate.getParentTagName(hit.collider.tag) == NameDefine.TagName.Fuse)
                {
                    Fuse _fuse = hit.collider.GetComponent<Fuse>();
                    // 導火線削除
                    if (Input.GetMouseButtonDown(0))
                    {
                        // 空オブジェクト
                        GameObject obj = Instantiate(m_feildPrefab, hit.collider.transform.position, Quaternion.identity);
                        obj.transform.parent = transform.GetChild(0);
                        obj.transform.tag = NameDefine.TagName.Player;
                        // 削除
                        Destroy(hit.collider.gameObject);
                        m_fuseData.Remove(hit.transform.position);
                    }
                    // 導火線モデルのオブジェクトに追加情報を付与・剥奪
                    else if (Input.GetMouseButtonDown(2))
                    {
                        if (_fuse.Type != Fuse.FuseType.Start)
                        {
                            _fuse.Type = Fuse.FuseType.Start;
                            _fuse.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                            m_cursorTouchObj = null;
                        }
                        else
                        {
                            _fuse.Type = Fuse.FuseType.Normal;
                            _fuse.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
                            m_cursorTouchObj = _fuse.gameObject;
                        }

                        m_fuseData[_fuse.transform.position] = (int)_fuse.Type + m_fuseData[_fuse.transform.position].Substring(1, m_fuseData[_fuse.transform.position].Length - 1);
                    }
                    // 設置位置の色の変更
                    else if (_fuse.Type != Fuse.FuseType.Start)
                    {

                        if (m_cursorTouchObj)
                            m_cursorTouchObj.GetComponent<Renderer>().material.SetColor("_Color", Color.white);

                        m_cursorTouchObj = hit.collider.gameObject;
                        m_cursorTouchObj.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
                    }
                }
                // 設置済みのギミックにタッチしているなら
                else if (Utility.TagSeparate.getParentTagName(hit.collider.tag) == NameDefine.TagName.Gimmick)
                {
                    // 導火線削除
                    if (Input.GetMouseButtonDown(0))
                    {
                        // 空オブジェクト
                        GameObject obj = Instantiate(m_feildPrefab, hit.collider.transform.position, Quaternion.identity);
                        obj.transform.parent = transform.GetChild(0);
                        obj.transform.tag = NameDefine.TagName.Player;
                        // 削除
                        Destroy(hit.collider.gameObject);
                        m_fuseData.Remove(hit.transform.position);
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

        GameObject[] _objList = GameObject.FindGameObjectsWithTag(NameDefine.TagName.Player);
        int difference = _objList.Length - _stageSizeX * _stageSizeY * _stageSizeZ;

        // 変更後のほうが設置可能数が多い（同数含む）なら
        if (difference <= 0)
        {
            Vector3 half = new Vector3(_stageSizeX / 2, _stageSizeY / 2, _stageSizeZ / 2);
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
                            obj.transform.tag = NameDefine.TagName.Player;
                            obj.layer = NameDefine.Layer.Trans;
                        }
                    }
        }
        // 変更前のほうが設置可能数が多いなら
        else
        {
            // 差分を解放
            for (int i = 0; i < difference; ++i)
                Destroy(_objList[i]);

            Vector3 half = new Vector3(_stageSizeX / 2, _stageSizeY / 2, _stageSizeZ / 2);
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

    void SetObjData(GameObject _createObj)
    {
        // その場所にまだ導火線がないなら
        int createRotX = inputFieldInt.GetInputFieldInt(inputFieldInt.FieldType.createRotX);
        int createRotY = inputFieldInt.GetInputFieldInt(inputFieldInt.FieldType.createRotY);
        int createRotZ = inputFieldInt.GetInputFieldInt(inputFieldInt.FieldType.createRotZ);
        if (createRotX == ProcessedtParameter.System_Constant.ERROR_INT ||
            createRotY == ProcessedtParameter.System_Constant.ERROR_INT ||
            createRotZ == ProcessedtParameter.System_Constant.ERROR_INT)
            return;

        _createObj.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
        _createObj.transform.parent = transform.GetChild(0);
        string objID = "";
        if (Utility.TagSeparate.getParentTagName(_createObj.tag) == NameDefine.TagName.Fuse)
        {
            Fuse _fuse = _createObj.GetComponent<Fuse>();
            if (m_selectObj)
            {
                Fuse _select = m_selectObj.GetComponent<Fuse>();
                _fuse.Type = _select.Type;
                _fuse.transform.localEulerAngles = new Vector3(createRotX, createRotY, createRotZ);
            }
            objID = (int)_fuse.Type + Utility.TagSeparate.getChildTagName(_createObj.tag).
                Substring(0, ProcessedtParameter.CSV_Constant.OBJECT_WORD_COUNT);
        }
        else if (Utility.TagSeparate.getParentTagName(_createObj.tag) == NameDefine.TagName.Gimmick)
        {
            GameGimmick _gimmick = _createObj.GetComponent<GameGimmick>();
            if (m_selectObj)
            {
                GameGimmick _select = m_selectObj.GetComponent<GameGimmick>();
                _gimmick.Type = _select.Type;
                _gimmick.transform.localEulerAngles = new Vector3(createRotX, createRotY, createRotZ);
            }
            objID = (int)_gimmick.Value % 10 + Utility.TagSeparate.getChildTagName(_createObj.tag).
                Substring(0, ProcessedtParameter.CSV_Constant.OBJECT_WORD_COUNT);
        }

        // ステージ配列に情報追加
        m_fuseData.Add(_createObj.transform.position, objID + 
            _createObj.transform.localEulerAngles.x / 90 + _createObj.transform.localEulerAngles.y / 90 + _createObj.transform.localEulerAngles.z / 90);
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // ボタンの処理

    /// <summary>
    /// 実際のステージを確認
    /// </summary>
    public void ViewPlayStage()
    {
        GameObject[] _objList = GameObject.FindGameObjectsWithTag(NameDefine.TagName.Player);

        AllFuseDefault();
        foreach (GameObject _obj in _objList)
        {
            MeshRenderer _mesh = _obj.GetComponent<MeshRenderer>();

            if(_mesh)
                _mesh.enabled = m_isPreview;
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
            m_terrainCreate.CreateGround(_stageSizeX, _stageSizeZ, -_stageSizeY / 2 - 1);
            m_terrainCreate.CreateWall(_stageSizeY);
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
        // プレビュー中は操作不可
        if (m_isPreview)
            return;

        GameObject _stage = transform.GetChild(0).gameObject;
        for (int i = 0; i < _stage.transform.childCount; ++i)
        {
            GameObject _stageObj = _stage.transform.GetChild(i).gameObject;
            MeshRenderer[] _meshList = _stageObj.GetComponentsInChildren<MeshRenderer>();
            for (int j = 0; j < _meshList.Length; ++j)
            {
                MeshRenderer _mesh = _meshList[j];
                if (_mesh)
                    _mesh.enabled = true;
            }

            if (_stageObj.transform.childCount == 0)
                _stageObj.gameObject.layer = NameDefine.Layer.Trans;
            else
                _stageObj.gameObject.layer = NameDefine.Layer.Default;
        }
    }

    /// <summary>
    /// ステージ保存
    /// </summary>
    public void StageSave()
    {
        // これからプレビュー状態にする
        m_isPreview = false;
        ViewPlayStage();

        int _stageSizeX = inputFieldInt.GetInputFieldInt(inputFieldInt.FieldType.stageSizeX);
        int _stageSizeY = inputFieldInt.GetInputFieldInt(inputFieldInt.FieldType.stageSizeY);
        int _stageSizeZ = inputFieldInt.GetInputFieldInt(inputFieldInt.FieldType.stageSizeZ);
        int _stageNum = inputFieldInt.GetInputFieldInt(inputFieldInt.FieldType.stageNum);

        if (_stageSizeX == ProcessedtParameter.System_Constant.ERROR_INT ||
            _stageSizeY == ProcessedtParameter.System_Constant.ERROR_INT ||
            _stageSizeZ == ProcessedtParameter.System_Constant.ERROR_INT ||
            _stageNum == ProcessedtParameter.System_Constant.ERROR_INT)
            return;


        string letter = "";
        List<string> stageList = new List<string>();
        for (int i = 0; i < ProcessedtParameter.CSV_Constant.STAGE_DATA_COUNT; ++i)
            letter += "-";

        // 初期化
        for(int i= 0; i < _stageSizeX * _stageSizeY * _stageSizeZ; ++i)
        {
            stageList.Add(letter);
        }

        // ステージの導火線やギミックの追加
        Vector3 half = new Vector3(_stageSizeX / 2, _stageSizeY / 2, _stageSizeZ / 2);
        foreach(KeyValuePair<Vector3, string> keyValue in m_fuseData)
        {
            stageList[Utility.CSVFile.PosToIndex(keyValue.Key + half, _stageSizeX, _stageSizeY)] = keyValue.Value;
        }

        // プレビューで追加された壁の追加
        Transform _wallParent =  m_terrainCreate.transform.GetChild((int)TerrainCreate.TerrainChild.Wall);
        for (int i = 0; i < _wallParent.childCount; ++i)
        {
            Transform _obj = _wallParent.GetChild(i);
            GameGimmick _gimmick = _obj.GetComponent<GameGimmick>();

            if (!_gimmick)
                continue;

            _gimmick.Type = GameGimmick.GimmickType.Wall;
            _gimmick.transform.localEulerAngles = Vector3.zero;

            int nIdx = Utility.CSVFile.PosToIndex(_gimmick.transform.position + half, _stageSizeX, _stageSizeY);
            stageList[nIdx] =
                // ギミックの値　＋　タグの名前の一部　＋　回転
                (int)_gimmick.Value % 10 + Utility.TagSeparate.getChildTagName(_gimmick.tag).
                Substring(0, ProcessedtParameter.CSV_Constant.OBJECT_WORD_COUNT) + 
                _gimmick.transform.localEulerAngles.x / 90 + _gimmick.transform.localEulerAngles.y / 90
                + _gimmick.transform.localEulerAngles.z / 90;
        }

        Utility.CSVFile.WriteCsv(stageList, ProcessedtParameter.CSV_Constant.STAGE_DATA_PATH + _stageNum, _stageSizeX, _stageSizeY);
    }

    public void LoadStage()
    {
        // 現在のステージ情報をすべてクリア
        m_fuseData.Clear();
        GameObject store = m_selectObj;
        m_selectObj = null;

        // これからプレビュー状態であれば解除
        if (m_isPreview)
            ViewPlayStage();

        // 現状のステージをすべて破棄
        Transform _stage = transform.GetChild(0);
        for (int i = 0; i < _stage.childCount; ++i)
        {
            GameObject _stageObj = _stage.GetChild(i).gameObject;
            Destroy(_stageObj);
        }
        string letter = "";
        for (int i = 0; i < ProcessedtParameter.CSV_Constant.STAGE_DATA_COUNT; ++i)
            letter += "-";

        int _stageNum = inputFieldInt.GetInputFieldInt(inputFieldInt.FieldType.stageNum);
        Utility.CSVFile.CSVData info = Utility.CSVFile.LoadCsv(ProcessedtParameter.CSV_Constant.STAGE_DATA_PATH + _stageNum);
        List<GameObject> _createList =  StageCreateMgr.Instance.CreateStage(_stage, info);

        foreach (GameObject _obj in _createList)
            SetObjData(_obj);

        for (int i = 0; i < info.data.Count; ++i)
        {
            if (info.data[i] != letter)
                continue;

            Vector3 pos = Utility.CSVFile.IndexToPos(i, info.size.x, info.size.y, info.size.z);
            GameObject obj = Instantiate(m_feildPrefab, pos, Quaternion.identity);
            obj.transform.parent = transform.GetChild(0);
            obj.transform.tag = NameDefine.TagName.Player;
            obj.layer = NameDefine.Layer.Trans;
        }

        // ステージのサイズをセット
        inputFieldInt.SetInputFieldInt(inputFieldInt.FieldType.stageSizeX, info.size.x);
        inputFieldInt.SetInputFieldInt(inputFieldInt.FieldType.stageSizeY, info.size.y);
        inputFieldInt.SetInputFieldInt(inputFieldInt.FieldType.stageSizeZ, info.size.z);
        m_selectObj =  store;

    }

    /// <summary>
    ///  ステージの表面を削除
    /// </summary>
    /// <param name="rayPlace">rayを飛ばしている場所がカメラから見てどの向きにあるか</param>
    public void CutBox(int rayPlace)
    {
        // プレビュー中は操作不可
        if (m_isPreview)
            return;

        RaycastHit hit = new RaycastHit();
        GameObject rayPoint = Camera.main.transform.GetChild(rayPlace).gameObject;
        Ray ray = new Ray(rayPoint.transform.position, -rayPoint.transform.position);

        if (!Physics.Raycast(ray, out hit) ||
            (hit.collider.transform.tag != NameDefine.TagName.Player &&
                Utility.TagSeparate.getParentTagName(hit.collider.transform.tag) != NameDefine.TagName.Fuse))
            return;

        float maxValue;
        Vector3 hitPos = hit.collider.transform.position;
        Vector3 difference = rayPoint.transform.position - hitPos;

        difference = new Vector3(Mathf.Abs(difference.x), Mathf.Abs(difference.y), Mathf.Abs(difference.z));
        maxValue = Mathf.Max(difference.x, difference.y, difference.z);

        // 最大値を出している方向を調査
        Vector3 cheak = new Vector3(maxValue, maxValue, maxValue) - difference;

        GameObject _stage = transform.GetChild(0).gameObject;
        for (int i = 0; i < _stage.transform.childCount; ++i)
        {
            GameObject _stageObj = _stage.transform.GetChild(i).gameObject;
            if ((cheak.x == 0.0f && _stageObj.transform.position.x == hitPos.x) ||
                (cheak.y == 0.0f && _stageObj.transform.position.y == hitPos.y) ||
                (cheak.z == 0.0f && _stageObj.transform.position.z == hitPos.z))
            {
                _stageObj.gameObject.layer = NameDefine.Layer.Ignore;

                MeshRenderer[] _meshList = _stageObj.GetComponentsInChildren<MeshRenderer>();
                for (int j = 0; j < _meshList.Length; ++j)
                {
                    MeshRenderer _mesh = _meshList[j];
                    if (_mesh)
                        _mesh.enabled = false;
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

        for(int i = 0; i < transform.GetChild(0).childCount; ++i)
        {
            Fuse _fuse = transform.GetChild(0).GetChild(i).gameObject.GetComponent<Fuse>();

            if (!_fuse || _fuse.State != Fuse.FuseState.UI)
                continue;

            _fuse.transform.localEulerAngles = new Vector3(
                            inputFieldInt.GetInputFieldInt(inputFieldInt.FieldType.createRotX),
                            inputFieldInt.GetInputFieldInt(inputFieldInt.FieldType.createRotY),
                            inputFieldInt.GetInputFieldInt(inputFieldInt.FieldType.createRotZ));
        } 

        for(int i = 0; i < transform.GetChild(1).childCount; ++i)
        {
            GameGimmick _gimmick = transform.GetChild(1).GetChild(i).gameObject.GetComponent<GameGimmick>();
            if (!_gimmick || !_gimmick.UI)
                continue;

            _gimmick.transform.localEulerAngles = new Vector3(
                            inputFieldInt.GetInputFieldInt(inputFieldInt.FieldType.createRotX),
                            inputFieldInt.GetInputFieldInt(inputFieldInt.FieldType.createRotY),
                            inputFieldInt.GetInputFieldInt(inputFieldInt.FieldType.createRotZ));
        }
    }
}
