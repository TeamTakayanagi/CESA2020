using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage : MonoBehaviour
{
    [SerializeField]
    private PopUp m_popUpPrefab = null;

    private PopUp m_popUP = null;

    private Canvas m_stageCanvas = null;

    // Start is called before the first frame update
    void Start()
    {
        m_stageCanvas = GameObject.FindGameObjectWithTag(ConstDefine.TagName.UIStage).GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        if (TitleMgr.Instance.Step < 7) return;

        if (Input.GetMouseButtonUp(0))
        {
            Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit _hit = new RaycastHit();
            float max_distance = 500f;

            bool is_hit = Physics.Raycast(_ray, out _hit, max_distance);

            if (is_hit)
            {
                if (_hit.transform == transform)
                {
                    m_popUP = Instantiate(m_popUpPrefab, m_stageCanvas.transform.position, m_stageCanvas.transform.rotation, m_stageCanvas.transform);
                }
            }
        }
    }
}
