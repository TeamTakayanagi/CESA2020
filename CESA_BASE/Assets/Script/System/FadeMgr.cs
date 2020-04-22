using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeMgr : SingletonMonoBehaviour<FadeMgr>
{
    [SerializeField]
    private GameObject FadePrefab = null;
    private GameObject m_fade;

    private bool m_fadeFlg;

    // Start is called before the first frame update
    void Start()
    {
        m_fade = Instantiate(FadePrefab, transform);

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartFade(string _str = "null")
    {
        switch (_str)
        {
            case "Alpha":
                m_fade.GetComponent<FadeAlpha>().StartFade();
                break;

            default:
                m_fade.GetComponent<FadeAlpha>().StartFade();
                break;
        }
    }
}
