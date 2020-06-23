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
        m_state = FadeState.None;
        Draw(false);
    }

    protected void Update()
    {
        if (m_state == FadeState.None)
            return;


        if (FadeCheack())
        {
            m_state = m_stateNext;

            if (m_state == FadeState.None)
            {
                Draw(false);
            }
            else
            {
                FadeOut();
                SceneManager.LoadSceneAsync(m_nextScene);
                EffectManager.Instance.Create = true;
            }
        }
    }

    public void FadeStart(string nextScene)
    {
        m_state = FadeState.FadeIn;
        m_nextScene = nextScene;
        FadeIn();
        Draw(true);
    }

    /// <summary>
    /// フェード開始処理
    /// </summary>
    protected virtual void FadeIn()
    {
        m_stateNext = FadeState.FadeOut;
    }

    /// <summary>
    /// フェード終了処理
    /// </summary>
    protected virtual void FadeOut()
    {
        m_stateNext = FadeState.None;
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
