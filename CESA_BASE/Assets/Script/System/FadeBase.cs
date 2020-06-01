using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public abstract class FadeBase : MonoBehaviour
{
    protected delegate void FadeStateFunc();

    public enum FadeState
    {
        None,
        FadeIn,
        FadeOut,
    }

    protected string m_nextScene;
    protected FadeStateFunc m_func;
    protected FadeStateFunc m_funcNext;
    protected FadeState m_state;
    protected FadeState m_stateNext;
    protected FadeMgr.FadeType m_type;

    public FadeMgr.FadeType FadeType
    {
        get
        {
            return m_type;
        }
    }
    public FadeState State
    {
        get
        {
            return m_state;
        }
        set
        {
            m_state = value;
        }
    }
    public string NextScene
    {
        set
        {
            m_nextScene = value;
        }
    }

    protected void Start()
    {
        m_func = null;
        m_state = FadeState.None;
        Draw(false);
    }

    protected void Update()
    {
        if (m_state == FadeState.None || m_func == null)
            return;

        m_func();

        if (FadeCheack())
        {
            m_state = m_stateNext;
            m_func = m_funcNext;

            if (m_state == FadeState.None || m_func == null)
            {
                Draw(false);
                EffectManager.Instance.Create = true;
            }
            else
            {
                SceneManager.LoadScene(m_nextScene);
            }
        }
    }

    public void FadeStart(string nextScene)
    {
        m_state = FadeState.FadeIn;
        m_nextScene = nextScene;
        m_func = FadeIn;
        Draw(true);
    }

    /// <summary>
    /// フェード開始処理
    /// </summary>
    protected virtual void FadeIn()
    {
        m_stateNext = FadeState.FadeOut;
        m_funcNext = FadeOut;
    }

    /// <summary>
    /// フェード終了処理
    /// </summary>
    protected virtual void FadeOut()
    {
        m_stateNext = FadeState.None;
        m_funcNext = null;
    }

    /// <summary>
    /// フェード遷移条件
    /// </summary>
    /// <returns>その条件を満たしたか</returns>
    protected abstract bool FadeCheack();

    /// <summary>
    /// 描画変更
    /// </summary>
    /// <param name="isDraw">描画するか否か</param>
    protected abstract void Draw(bool isDraw);
}
