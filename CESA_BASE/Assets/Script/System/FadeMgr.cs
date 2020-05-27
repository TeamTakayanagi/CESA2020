using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeMgr : SingletonMonoBehaviour<FadeMgr>
{
    public enum FadeType
    {
        Game_Start,
        Alpha,
    }

    public enum FadeState
    {
        FadeIn,
        SceneLoad,
        FadeOut,
    }


    private string m_nextScene;
    private FadeType m_type;
    private FadeState m_state;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FadeIn(string nextScene)
    {
        m_state = FadeState.FadeIn;
        m_nextScene = nextScene;
    }

    public void FadeOut()
    {
        m_state = FadeState.FadeOut;
    }
}
