using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCamera : MonoBehaviour
{
    [SerializeField]
    private GameObject mainCamera = null;
    [SerializeField]
    private GameObject resultCamera = null;

    private GameMgr GM = null;

    [SerializeField]
    private GameObject stage = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // テスト用
        if(Input.GetKeyDown(KeyCode.C))
        {
            ChangeCam();
        }
    }

    // 呼び出し
    // カメラをクリア演出用に切り替えて GameMgr を切ります
    public void ChangeCam()
    {
        mainCamera.SetActive(false);
        resultCamera.SetActive(true);

        GM = stage.GetComponent<GameMgr>();
        GM.enabled = false;
    }
}
