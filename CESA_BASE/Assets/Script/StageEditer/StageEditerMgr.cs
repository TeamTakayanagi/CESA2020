using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageEditerMgr : SingletonMonoBehaviour<StageEditerMgr>
{
    [SerializeField]
    int m_stageSizeX = 0;
    [SerializeField]
    int m_stageSizeY = 0;
    [SerializeField]
    int m_stageSizeZ = 0;
    [SerializeField]
    GameObject m_feildPrefab = null;

    Fuse m_selectFuse = null;
    GameObject m_cursorTouchObj = null;
    Vector3 m_cameraPos = Vector3.zero;
    Quaternion m_cameraRot = Quaternion.identity;
    Vector3 m_createRot = Vector3.zero;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        // UIの導火線仮選択
        m_selectFuse = GameObject.FindGameObjectWithTag(ConstDefine.TagName.Fuse).GetComponent<Fuse>();
        m_selectFuse.GetComponent<Renderer>().material.SetColor("_Color", Color.cyan);

        // カメラの初期情報保存
        m_cameraPos = Camera.main.transform.position;
        m_cameraRot = Camera.main.transform.rotation;

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
            ray = GameObject.FindGameObjectWithTag(ConstDefine.TagName.UICamera).GetComponent<Camera>().
                    ScreenPointToRay(Input.mousePosition);

            // 設置場所を選択
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.transform.parent.tag == ConstDefine.TagName.Fuse)
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
                if (hit.collider.tag == ConstDefine.TagName.Player)
                {
                    // 導火線設置
                    if (Input.GetMouseButtonDown(0) && m_selectFuse)
                    {
                        // その場所にまだ導火線がないなら
                        Fuse selectClone = Instantiate(m_selectFuse);      // 複製
                        selectClone.transform.position = hit.collider.transform.position;
                        selectClone.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
                        selectClone.Type = Fuse.FuseType.Fuse;
                        selectClone.transform.localEulerAngles = m_createRot;

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
                else if (hit.collider.tag == ConstDefine.TagName.Fuse)
                {
                    // 角度変更
                }
            }
        }
    }

    // 空のボックスを用いてステージの枠を生成
    void CreateStage()
    {
        GameObject[] _objList = GameObject.FindGameObjectsWithTag(ConstDefine.TagName.Player);
        int difference = m_stageSizeX * m_stageSizeY * m_stageSizeZ - _objList.Length;

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
                            obj.transform.tag = ConstDefine.TagName.Player;
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

/// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// ボタンの処理

    // 実際のステージを確認
    public void ViewPlayStage()
    {
        GameObject[] _objList = GameObject.FindGameObjectsWithTag(ConstDefine.TagName.Player);
        bool isSet = _objList[0].GetComponent<MeshRenderer>().enabled ^ true;
        foreach (GameObject _obj in _objList)
        {
            _obj.GetComponent<MeshRenderer>().enabled = isSet;
        }

        CameraDefault();
    }

    // カメラを初期位置に戻す
    public void CameraDefault()
    {
        Camera.main.transform.position = m_cameraPos;
        Camera.main.transform.rotation = m_cameraRot;
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
    }
}
