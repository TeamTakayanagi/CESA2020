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
    [SerializeField]
    private float m_gimmickValue = 0.0f;

    private bool m_isOnce = false;              // 一度きり判定用
    private bool m_isGimmickStart = false;
    private bool m_isUI = false;

    private GameObject m_fountain = null;
    private GameObject m_particle = null;
    private GameObject childParticle = null;

    private ObjectFunction m_function = null;

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
            m_function.Stop = value;
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

    void Start()
    {
        m_isOnce = false;
        m_function = transform.GetChild(0).GetComponent<ObjectFunction>();

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
            m_function.Stop = true;
        }
        else if (m_type == GimmickType.Goal)
        {
            m_isGimmickStart = false;
            m_function.Stop = false;
        }
        else
        {
            m_isGimmickStart = false;
        }
    }

    void Update()
    {
        if (!m_isGimmickStart || m_isOnce)
            return;

        if (m_type == GimmickType.Water)
        {
                StartCoroutine(GimmickWater());
        }
        else if (m_type == GimmickType.Goal)
        {
            m_gimmickValue += Time.deltaTime * GameMgr.Instance.GameSpeed;

            if (m_gimmickValue >= AdjustParameter.Fuse_Constant.BURN_MAX_TIME)
            {
                m_gimmickValue = AdjustParameter.Fuse_Constant.BURN_MAX_TIME;

                Effekseer.EffekseerEmitter effect = Fireworks.Instantiate(
                    Effekseer.EffekseerEmitter.EffectType.fireworks_core, transform.position,
                    new Vector3(transform.position.x, AdjustParameter.Production_Constant.END_FIRE_POS_Y, transform.position.z),
                    Vector3.one, Quaternion.identity, true);
                // 継続して花火を打ち上げ
                StartCoroutine("FireWorks");

                // ゲーム終了の合図
                GameMgr.Instance.BurnCount -= 1;
                GameMgr.Instance.FireGoal(true, effect.gameObject);

                m_isOnce = true;
                m_isGimmickStart = false;
            }
        }
    }

    private IEnumerator FireWorks()
    {
        float _launchTiming = (Random.Range(0, 600) + Time.deltaTime * 30) / 60;
        yield return new WaitForSeconds(ProcessedtParameter.LaunchTiming.GAME + _launchTiming);
        while (true)
        {
            Fireworks.Instantiate(
                Effekseer.EffekseerEmitter.EffectType.fireworks_core, transform.position,
                new Vector3(transform.position.x, AdjustParameter.Production_Constant.END_FIRE_POS_Y, transform.position.z),
                Vector3.one, Quaternion.identity, true); _launchTiming = (Random.Range(0, 600) + Time.deltaTime * 30) / 60;

            yield return new WaitForSeconds(ProcessedtParameter.LaunchTiming.GAME + _launchTiming);
        }
    }

    public IEnumerator GimmickWater()
    {
        RaycastHit hit = new RaycastHit();
        Vector3 dir = transform.rotation * Vector3.forward;
        int layerMask = ~(1 << 9);

        if (Physics.Raycast(transform.position, transform.rotation * Vector3.forward, out hit, m_gimmickValue, layerMask))
        {
            // 水が当たってる時の長さ
            if (Utility.TagSeparate.getParentTagName(hit.collider.tag) == NameDefine.TagName.Fuse)
            {
                GameFuse _fuse = hit.collider.gameObject.GetComponent<GameFuse>();
                _fuse.FuseWet();
                m_particle.transform.localScale = new Vector3(0.3f, 0.3f, hit.distance * 0.2f);

                // water drop のパーティクルが暴れるのでFuseのギミック中は表示しない
                if (_fuse.Moved || _fuse.Rotate)
                    childParticle.gameObject.SetActive(false);
                else
                    childParticle.gameObject.SetActive(true);
            }
            else
                m_particle.transform.localScale = new Vector3(0.3f, 0.3f, hit.distance * 0.3f - 0.1f);
        }
        else
        {
            childParticle.gameObject.SetActive(true);
            // 当たっていないときの長さ
            m_particle.transform.localScale = new Vector3(0.3f, 0.3f, m_gimmickValue * 0.3f - 0.1f);

        }

        yield break;
    }
}
