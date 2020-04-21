using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    private GameObject m_hitTarget = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
                    Debug.Log("ヒット");
                }
            }
        }
    }
}
