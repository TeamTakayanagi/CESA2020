using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartProduction : MonoBehaviour
{
    GameObject m_fireworks = null;
    GameObject m_fuse = null;
    GameObject m_fire = null;

    // Start is called before the first frame update
    void Start()
    {
        m_fireworks = transform.GetChild(0).gameObject;
        m_fuse = transform.GetChild(1).gameObject;
        m_fire = transform.GetChild(2).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        m_fire.transform.DOMove(m_fireworks.transform.position, 5.0f);
    }
}
