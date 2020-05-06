using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGimmick : MonoBehaviour
{
    public enum GimmickType
    {
        Goal,
        Water,
        Wall
    }

    [SerializeField]
    private GimmickType m_type = GimmickType.Goal;
    private bool m_isUI = false;

    // 水
    [SerializeField]
    private float m_gimmickValue = 0.0f;      // 水の長さ


    public GimmickType Type
    {
        get
        {
            return m_type;
        }
        set
        {
            m_type = value;
        }
    }
    public bool UI
    {
        get
        {
            return m_isUI;
        }
        set
        {
            m_isUI = value;
        }
    }
    public float Value
    {
        get
        {
            return m_gimmickValue;
        }
        set
        {
            m_gimmickValue = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_type == GimmickType.Water)
            StartCoroutine(GimmickWater());
    }

    public IEnumerator  GimmickWater()
    {
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(transform.position, Vector3.forward, out hit, m_gimmickValue))
        {
            if (hit.collider.gameObject.CompareTag("Fuse"))
            {
                hit.collider.gameObject.GetComponent<Fuse>().FuseWet();
            }
        }

        yield break;
    }
}
