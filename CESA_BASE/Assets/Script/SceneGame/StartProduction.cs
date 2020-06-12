using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartProduction : MonoBehaviour
{
    public enum Production
    {
        wait,
        moveY, 
        moveX, 
        end
    }

    GameObject m_fireworks = null;
    Image m_click = null;
    UiFunction m_start = null;
    Image m_fuse = null;
    Effekseer.EffekseerEmitter m_fire = null;
    Vector3 m_defaultPos;
    Vector3 m_target;
    Vector3 m_size;
    Production m_state = Production.wait;

    public Production State
    {
        set
        {
            if (m_state == Production.wait && m_click)
            {
                m_click.enabled = false;
                m_fire.Play();
            }
            m_state = value;
        }
        get
        {
            return m_state;
        }
    }

    void Awake()
    {
        m_state = Production.wait;
    }

    void Start()
    {
        // 演出用の子オブジェクト取得
        GameObject _fuse = transform.GetChild(0).gameObject;
        m_fuse = _fuse.GetComponent<Image>();
        m_fireworks = transform.GetChild(1).gameObject;
        m_fire = transform.GetChild(2).GetComponent<Effekseer.EffekseerEmitter>();
        m_click = transform.GetChild(3).gameObject.GetComponent<Image>();
        GameObject _start = GameObject.FindGameObjectWithTag(NameDefine.TagName.UIStart);
        m_start = _start.GetComponent<UiFunction>();
        m_start.Stop = true;
        m_start.gameObject.SetActive(false);

        RectTransform rectFireworks = m_fireworks.GetComponent<RectTransform>();
        m_size = new Vector3(-rectFireworks.rect.width * 0.5f, rectFireworks.rect.height * 0.25f, 0.0f);
       // 必要情報を格納
        m_target = m_fireworks.transform.localPosition + m_size;
        m_defaultPos = m_fire.transform.localPosition;

        // シェーダーを初期化
        m_fuse.material.SetFloat("_MaskX", 0);
        m_fuse.material.SetFloat("_MaskY", 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_fire || (m_state != Production.moveX && m_state != Production.moveY))
            return;

        if (m_state == Production.moveY)
        {
            m_fire.transform.localPosition += new Vector3(0.0f, AdjustParameter.Production_Constant.START_FUSE_MOVE, 0.0f) * Time.deltaTime;
            if (m_fire.transform.localPosition.y >= m_target.y)
            {
                m_state = Production.moveX;
            }
            m_fuse.material.SetFloat("_MaskY",
                Mathf.Clamp01((m_fire.transform.localPosition - m_defaultPos).y / (m_target + m_size - m_defaultPos).y));
        }
        else if(m_state == Production.moveX)
        {
            m_fire.transform.localPosition += new Vector3(AdjustParameter.Production_Constant.START_FUSE_MOVE, 0.0f, 0.0f) * Time.deltaTime;
            if (m_fire.transform.localPosition.x >= m_target.x)
            {
                m_state = Production.end;
                m_fire.Stop();
                m_start.gameObject.SetActive(true);
                m_start.Stop = false;
            }
        }
    }
}
