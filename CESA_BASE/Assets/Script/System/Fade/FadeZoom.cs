using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FadeZoom : FadeBase
{
    private Image m_sprite = null;
    private Material m_material = null;
    private float m_radius = 0.0f;
    private Color m_color = new Color(0.0f, 0.0f, 0.0f, 0.0f);

    new void Start()
    {
        // 
        Transform circleText = transform.GetChild(0);

        m_sprite = circleText.GetComponent<Image>();
        m_material = m_sprite.material;
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    override protected void FadeIn()
    {
        StartCoroutine(DoScaleMaterial(0.0f, AdjustParameter.Camera_Constant.FADE_DURATION));
        base.FadeIn();
    }

    override protected void FadeOut()
    {
        StartCoroutine(DoColorMaterial(new Color(0.0f, 0.0f, 0.0f, 0.0f), AdjustParameter.Camera_Constant.FADE_DURATION));
        base.FadeOut();
    }

    override protected bool FadeCheack()
    {
        if (m_state == FadeState.FadeIn)
        {
            if(m_radius <= 0.0f)
                return true;
            return false;
        }
        else
            return m_color.a <= 0.0f;
    }

    override protected void Draw(bool isDraw)
    {
        if (isDraw)
        {
            m_color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
            m_material.SetFloat("_Alpha", m_color.a);
        }
        else
        {
            m_radius = 1.0f;
            m_material.SetFloat("_Radius", m_radius);
        }

        m_sprite.enabled = true;
    }

    private IEnumerator DoScaleMaterial(float target, float time)
    {
        float value = (target - m_radius) / time;
        float timeCounter = 0.0f;
        while(true)
        {
            m_radius += value * Time.deltaTime;
            if(timeCounter >= time)
            {
                m_radius = target;
                m_material.SetFloat("_Radius", m_radius);
                break;
            }

            m_material.SetFloat("_Radius", m_radius);
            timeCounter += Time.deltaTime;
            yield return null;
       }
        yield break;
    }
    private IEnumerator DoColorMaterial(Color target, float time)
    {
        Color value = (target - m_color) / time;
        float timeCounter = 0.0f;
        while(true)
        {
            m_color += value * Time.deltaTime;
            if(timeCounter >= time)
            {
                m_color = target;
                m_material.SetFloat("_Alpha", m_color.a);
                break;
            }

            m_material.SetFloat("_Alpha", m_color.a);
            timeCounter += Time.deltaTime;
            yield return null;
       }
        yield break;
    }
}
