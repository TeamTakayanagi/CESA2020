using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FadeZoom : FadeBase
{
    private readonly Vector3 MASK_SCALE = new Vector3(450, 450, 1);

    private MainCamera m_mainCamera = null;
    private SpriteRenderer m_sprite = null;
    private SpriteMask m_maskTexture = null;

    new void Start()
    {
        m_mainCamera = Camera.main.GetComponent<MainCamera>();
        m_sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        m_maskTexture = transform.GetChild(1).GetComponent<SpriteMask>();
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    override protected void FadeIn()
    {
        m_maskTexture.transform.DOScale(Vector3.zero, AdjustParameter.Camera_Constant.FADE_DURATION);
        base.FadeIn();
    }

    override protected void FadeOut()
    {
        m_sprite.DOColor(new Color(0.0f, 0.0f, 0.0f, 0.0f), AdjustParameter.Camera_Constant.FADE_DURATION) ;
        base.FadeOut();
    }
    override protected bool FadeCheack()
    {
        if (m_state == FadeState.FadeIn)
        {
            if(Mathf.Floor(m_maskTexture.transform.localScale.x) <= 0.0f)
            {
                m_maskTexture.enabled = false;
                return true;
            }
            return false;
        }
        else
            return m_sprite.color.a <= 0.0f;
    }

    override protected void Draw(bool isDraw)
    {
        if (isDraw)
        {
            m_sprite.color = new Color(m_sprite.color.r, m_sprite.color.g, m_sprite.color.b, 1.0f);
        }
        else
        {
            m_maskTexture.transform.DOPause();
            m_maskTexture.transform.localScale = MASK_SCALE;
        }

        m_sprite.enabled = isDraw;
        m_maskTexture.enabled = isDraw;
    }
}
