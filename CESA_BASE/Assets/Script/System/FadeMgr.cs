using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeMgr : SingletonMonoBehaviour<FadeMgr>
{
    public enum FadeType
    {
        Rat,
        Alpha,
    }

    private List<FadeBase> m_fadeList = new List<FadeBase>();
    FadeBase m_fade = null;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        for(int i = 0; i < transform.childCount; ++i)
        {
            m_fadeList.Add(transform.GetChild(i).GetComponent<FadeBase>());
        }
        m_fadeList.Sort((a, b) => a.FadeType - b.FadeType);

    }
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.G))
            StartFade(FadeType.Rat, "");
    }


    public void StartFade(FadeType type, string nextScene)
    {
        m_fade = m_fadeList[(int)type].GetComponent<FadeBase>();
        m_fade.FadeStart(nextScene);
    }
}
