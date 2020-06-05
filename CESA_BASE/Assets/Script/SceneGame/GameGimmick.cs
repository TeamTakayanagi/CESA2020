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
    private float m_materialValue = 0.0f;      // 水の長さ
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
            // Resourcesからパーティクル取得
            m_fountain = (GameObject)Resources.Load("Fountain");
            // 位置調整用
            Vector3 instantPos = transform.position + (transform.rotation * Vector3.forward) / 2
                + (transform.rotation * Vector3.down) / 8;
            m_particle = Instantiate(m_fountain, instantPos, transform.rotation);
            childParticle = m_particle.transform.GetChild(0).gameObject;

            m_isGimmickStart = true;
        }
        else if (m_type == GimmickType.Goal)
        {
            m_materialValue = -0.5f;
            transform.GetChild(0).GetComponent<Renderer>().material.SetFloat("_OutTime", transform.localPosition.y + m_materialValue);
            m_isGimmickStart = false;
        }
        else
            m_isGimmickStart = false;
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
            m_materialValue += Time.deltaTime * GameMgr.Instance.GameSpeed / AdjustParameter.Fuse_Constant.BURN_MAX_TIME;
            transform.GetChild(0).GetComponent<Renderer>().material.SetFloat("_OutTime", transform.localPosition.y + m_materialValue);
            if (m_gimmickValue >= AdjustParameter.Fuse_Constant.BURN_MAX_TIME)
            {
                m_gimmickValue = AdjustParameter.Fuse_Constant.BURN_MAX_TIME;
                GameMgr.Instance.BurnCount -= 1;
                Effekseer.EffekseerEmitter effect = EffectManager.Instance.EffectCreate(Effekseer.EffekseerEmitter.EffectType.fireworks_core,
                    transform.position,
                    new Vector3(transform.position.x, AdjustParameter.Production_Constant.END_FIRE_POS_Y, transform.position.z),
                    Vector3.one, Quaternion.identity);
                GameMgr.Instance.FireGoal(true, effect.gameObject);
                m_isGimmickStart = false;
            }
        }
    }

    public IEnumerator GimmickWater()
    {
        RaycastHit hit = new RaycastHit();
        
        if (Physics.Raycast(transform.position, transform.rotation * Vector3.forward, out hit, m_gimmickValue))
        {
            if (Utility.TagSeparate.getParentTagName(hit.collider.tag) == NameDefine.TagName.Fuse)
            {
                hit.collider.gameObject.GetComponent<Fuse>().FuseWet();

                // Fuseがギミック動作中か取得
                m_isMoved = hit.collider.gameObject.GetComponent<Fuse>().SetMoveFrag();
                m_isRotate = hit.collider.gameObject.GetComponent<Fuse>().SetRotFrag();

                // 水が当たってる時の長さ
                m_particle.transform.localScale = new Vector3(0.3f, 0.3f, hit.distance * 0.2f);

                // water drop のパーティクルが暴れるのでFuseのギミック中は表示しない
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
            childParticle.gameObject.SetActive(true);
            // 当たっていないときの長さ
            m_particle.transform.localScale = new Vector3(0.3f, 0.3f, m_gimmickValue * 0.3f - 0.1f);
        }

        // レイ表示
        //Debug.DrawRay(transform.position, transform.rotation * Vector3.forward, Color.blue, 5, false);


        yield break;
    }
}
