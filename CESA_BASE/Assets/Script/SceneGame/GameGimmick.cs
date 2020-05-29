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
    private bool m_isGimmickStart = false;
    private GameObject m_fountain = null;
    private GameObject m_particle = null;
    private GameObject childParticle = null;


    private bool m_isMoved = false;
    private bool m_isRotate = false;

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
    public bool GimmickStart
    {
        get
        {
            return m_isGimmickStart;
        }
        set
        {
            m_isGimmickStart = value;
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

    void Start()
    {
        if (m_type == GimmickType.Water)
        {
            m_fountain = (GameObject)Resources.Load("Fountain");
            m_particle = Instantiate(m_fountain, transform.position, transform.rotation);
            childParticle = m_particle.transform.GetChild(0).gameObject;

            m_isGimmickStart = true;
        }
        else
            m_isGimmickStart = false;
        //m_gimmickValue = 0.0f;
    }

    void Update()
    {
        if (!m_isGimmickStart)
            return;

        if (m_type == GimmickType.Water)
            StartCoroutine(GimmickWater());

        else if(m_type == GimmickType.Goal)
        {
            m_gimmickValue += Time.deltaTime * GameMgr.Instance.GameSpeed;
            if(m_gimmickValue >= AdjustParameter.Fuse_Constant.BURN_MAX_TIME)
            {
                m_gimmickValue = AdjustParameter.Fuse_Constant.BURN_MAX_TIME;
                GameMgr.Instance.BurnCount -= 1;
                GameMgr.Instance.FireGoal(this, true);
                m_isGimmickStart = false;
            }
        }
    }

    public IEnumerator GimmickWater()
    {
        RaycastHit hit = new RaycastHit();
        Vector3 rot = new Vector3(transform.rotation.x / 90,
            transform.rotation.y / 90, transform.rotation.z / 90);

        if (Physics.Raycast(transform.position, transform.rotation * Vector3.forward , out hit, m_gimmickValue))
        {
            if (Utility.TagSeparate.getParentTagName(hit.collider.tag) == NameDefine.TagName.Fuse)
            {
                hit.collider.gameObject.GetComponent<Fuse>().FuseWet();

                m_isMoved = hit.collider.gameObject.GetComponent<Fuse>().SetMoveFrag();
                m_isRotate = hit.collider.gameObject.GetComponent<Fuse>().SetRotFrag();

                m_particle.transform.localScale = new Vector3(0.3f, 0.3f, hit.distance * 0.2f);

                if (m_isMoved || m_isRotate)
                {
                    childParticle.gameObject.SetActive(false);
                }
                else
                {
                    childParticle.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            m_particle.transform.localScale = new Vector3(0.3f, 0.3f, m_gimmickValue * 0.2f);
        }


        Debug.DrawRay(transform.position, transform.rotation * Vector3.forward, Color.blue, 5, false);


        yield break;
    }
}
