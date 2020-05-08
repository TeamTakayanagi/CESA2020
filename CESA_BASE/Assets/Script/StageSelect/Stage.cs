using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage : MonoBehaviour
{
    [SerializeField]
    private int m_stageNum = 0;
    private bool m_isClear;
    private bool m_isSelect;


    private MainCamera m_camera = null;

    public int StageNum
    {
        get
        {
            return m_stageNum;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_camera = Camera.main.GetComponent<MainCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (TitleMgr.Instance.Step != TitleMgr.TitleStep.Select) 
            return;

        if (m_camera.Type == MainCamera.CameraType.SwipeMove)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit _hit = new RaycastHit();
                float max_distance = 500f;

                if (Physics.Raycast(_ray, out _hit, max_distance))
                {
                    if (_hit.transform == transform)
                    {
                        m_camera.ZoomIn(transform.position);
                        SelectMgr.Instance.ZoomObj = this;
                    }
                }
            }
        }
    }
}
