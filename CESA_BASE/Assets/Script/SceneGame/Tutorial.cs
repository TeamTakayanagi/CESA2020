using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    private LinkedList<GameFuse> m_uiFuse = new LinkedList<GameFuse>();         // UI部分の導火線
    private LinkedList<GameObject> m_fieldObject = new LinkedList<GameObject>();      // ゲーム画面の導火線

    private int m_step;
    private bool m_tutorialFlg = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // スタート演出終了後

        if (m_tutorialFlg)
        {
            foreach (GameFuse _fuse in m_uiFuse)
                _fuse.enabled = false;
            foreach (GameObject _obj in m_fieldObject)
                _obj.GetComponent<Behaviour>().enabled = false;
        }
    }
}
