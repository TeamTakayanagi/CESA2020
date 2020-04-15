using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ConstDefine;

public class Fuse : MonoBehaviour
{
    public enum FuseType
    {
        Fuse,
        Start, 
        Goal,
        UI
    }

    [SerializeField]
    private FuseType m_type = FuseType.Fuse;
    [SerializeField]
    private float m_BurnSpeed = ConstDefine.ConstParameter.BURN_SPEED;
    [SerializeField]
    private float m_burnRate = 0.0f;

    // 燃えているか
    private bool m_isBurn = false;
    private Vector3 m_defaultPos = Vector3.zero;
    private Vector3 m_defaultRot = Vector3.zero;

    public FuseType Type
    {
        get
        {
            return m_type;
        }
        set
        {
            m_type = value;
        }
    }
    public Vector3 DefaultRot
    {
        get
        {
            return m_defaultRot;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_defaultPos = transform.position;
        m_defaultRot = transform.localEulerAngles;
        switch (m_type)
        {
            case FuseType.Start:
                {
                    gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                    m_isBurn = true;
                    break;
                }
            case FuseType.Fuse:
                {
                    gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
                    m_isBurn = false;
                    break;
                }
            case FuseType.Goal:
                {
                    gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
                    m_isBurn = false;
                    break;
                }
            case FuseType.UI:
                {
                    gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
                    m_isBurn = false;
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(m_type == FuseType.UI)
        {
            //transform.localEulerAngles = - m_defaultRot - Camera.main.transform.localEulerAngles;
        }
    }

    private void LateUpdate()
    {
        //if (m_isBurn)
        //{
        //    m_burnRate += Time.deltaTime / m_BurnSpeed;
        //    if (m_burnRate >= 1.0f)
        //    {
        //        m_burnRate = 1.0f;
        //    }
        //}
    }


    // 
    private void OnTriggerStay(Collider other)
    {
        // UIかつ燃えていないものなら
        if (m_type == FuseType.UI || !m_isBurn)
            return;

        // 導火線との判定
        if (other.transform.tag == TagName.Fuse)
        {
            //if (m_burnRate > 0.0f)
            {
                Fuse cube = other.gameObject.GetComponent<Fuse>();
                // 相手が燃えているなら処理を飛ばす
                if (cube.m_isBurn || cube.m_type == FuseType.UI)
                    return;

                cube.m_isBurn = true;
                cube.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                // 
                if (cube.m_type == FuseType.Goal)
                {
                    SceneManager.LoadScene(ConstDefine.Scene.Clear);
                }
            }
        }
    }

    public void BackDefault(bool color)
    {
        transform.position = m_defaultPos;
        if(color)
            gameObject.GetComponent<Renderer>().material.color = Color.yellow;
    }
}
