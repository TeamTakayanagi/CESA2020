using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage : MonoBehaviour
{
    private Canvas m_stageCanvas = null;
    private MainCamera m_camera = null;

    // Start is called before the first frame update
    void Start()
    {
        m_stageCanvas = StageMgr.Instance.GetComponent<Canvas>();
        m_camera = Camera.main.GetComponent<MainCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (TitleMgr.Instance.Step < 7) 
            return;

        if (m_camera.Zoom == 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit _hit = new RaycastHit();
                float max_distance = 500f;

                bool is_hit = Physics.Raycast(_ray, out _hit, max_distance);

                if (is_hit)
                {
                    if (_hit.transform == transform)
                    {
                        m_camera.ZoomIn(transform.position);
                        SelectMgr.Instance.ZoomObj = gameObject;
                        //m_popUP = Instantiate(m_popUpPrefab, m_stageCanvas.transform.position, m_stageCanvas.transform.rotation, m_stageCanvas.transform);
                    }
                }
            }
        }
    }
}
