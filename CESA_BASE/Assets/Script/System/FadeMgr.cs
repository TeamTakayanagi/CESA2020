using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeMgr : SingletonMonoBehaviour<FadeMgr>
{
    private delegate void FadeStateFunc();
    public enum FadeType
    {
        Rat,
        Alpha,
    }

    public enum FadeState
    {
        None,
        FadeIn,
        FadeOut,
    }


    private string m_nextScene;
    private FadeType m_type;
    private FadeState m_state;

    [SerializeField]
    private ParticleSystem m_particleRat = null;
    private readonly AnimationCurve m_animCurve = AnimationCurve.Linear(0, 0, 1, 1);    // sprite移動用

    private FadeStateFunc m_fadeFunc;
    private FadeStateFunc m_fadeIn;
    private FadeStateFunc m_fadeOut;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        m_type = FadeType.Rat;
        m_state = FadeState.None;
        m_fadeFunc = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
            FadeIn("");

        if (m_state == FadeState.None || m_fadeFunc == null)
            return;

        m_fadeFunc();
    }

    public void FadeIn(string nextScene)
    {
        m_state = FadeState.FadeIn;
        m_nextScene = nextScene;

        switch (m_type)
        {
            case FadeType.Rat:
                m_fadeFunc = RatFadeIn;
                break;
            case FadeType.Alpha:
                break;
            default:
                break;
        }
    }

    public void FadeOut()
    {
        m_state = FadeState.FadeOut;
        switch (m_type)
        {
            case FadeType.Rat:
                break;
            case FadeType.Alpha:
                break;
            default:
                break;
        }
    }

    ////////////////////////////////////////
    /// デリゲートの関数

    void FadeEnd()
    {
        m_state = FadeState.None;
    }

    void RatFadeIn()
    {
        Transform fuse = transform.GetChild((int)FadeType.Rat).GetChild(1);
        for (int i = 0; i < fuse.childCount; ++i)
        {
            RectTransform trans = fuse.GetChild(i).GetComponent<RectTransform>();
            trans.DOMoveX(fuse.position.x, 5.0f);
            if ((int)Mathf.Ceil(trans.position.x) == (int)fuse.position.x)
            {
                m_fadeFunc = RatFadeOut;
                m_state = FadeState.FadeOut;
                break;
            }
        }
    }
    void RatFadeOut()
    {
        m_particleRat.Play();

        Transform rat = transform.GetChild((int)FadeType.Rat).GetChild(0);
        for (int i = 0; i < rat.childCount; ++i)
        {
            RectTransform trans = rat.GetChild(i).GetComponent<RectTransform>();
            trans.DOMoveX(rat.position.x, 5.0f);
        }
    }
    private IEnumerator FadeUpdate(Transform start, Vector3 target, FadeStateFunc func)
    {
        float startTime = Time.time;             // 開始時間
        Vector3 moveDistance;        // 移動距離および方向
        moveDistance = target - start.localPosition;

        while ((Time.time - startTime) < AdjustParameter.Fade_Constant.FADE_DURATION)
        {
            start.localPosition += moveDistance * m_animCurve.Evaluate((Time.time - startTime) / AdjustParameter.Fade_Constant.FADE_DURATION);
            m_fadeFunc = func;
            yield return 0;
        }

        yield break;
    }
}
