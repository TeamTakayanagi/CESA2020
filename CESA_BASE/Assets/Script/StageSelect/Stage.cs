using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    [SerializeField]
    private PopUp m_popUpPrefab = null;

    private PopUp m_popUP = null;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (TitleMgr.Instance.Step < 7)     return;

        if (Input.GetMouseButtonUp(0))
        {
            Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit _hit = new RaycastHit();
            float max_distance = 500f;

            bool is_hit = Physics.Raycast(_ray, out _hit, max_distance);

            if (is_hit)
            {
                if (_hit.transform.tag == "Stage")
                {
                    m_popUP = Instantiate(m_popUpPrefab, transform.root.position, Quaternion.identity, transform.root);
                }
            }
        }
    }
}
