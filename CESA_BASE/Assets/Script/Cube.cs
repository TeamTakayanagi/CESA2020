using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ConstDefine;

public class Cube : MonoBehaviour
{
    enum CubeType
    {
        Start, 
        Cube,
        Goal
    }

    [SerializeField]
    private CubeType m_type;
    // 燃えているか
    private bool m_isBurn;

    public bool Burn
    {
        get
        {
            return m_isBurn;
        }
        set
        {
            m_isBurn = value;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        switch(m_type)
        {
            case CubeType.Start:
                {
                    gameObject.GetComponent<Renderer>().material.color = Color.red;
                    m_isBurn = true;
                    break;
                }
            case CubeType.Cube:
                {
                    gameObject.GetComponent<Renderer>().material.color = Color.white;
                    m_isBurn = false;
                    break;
                }
            case CubeType.Goal:
                {
                    gameObject.GetComponent<Renderer>().material.color = Color.blue;
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
        
    }

    private void OnTriggerStay(Collider other)
    {
        // 燃えていないなら
        if (!m_isBurn)
            return;

        // 
        if (other.transform.tag == TagName.Player)
        {
            Cube cube = other.gameObject.GetComponent<Cube>();
            if (cube.m_isBurn)
                return;

            cube.m_isBurn = true;
            cube.gameObject.GetComponent<Renderer>().material.color = Color.red;
            // 
            if(cube.m_type == CubeType.Goal)
            {
                SceneManager.LoadScene(ConstDefine.Scene.Clear);
            }
        }
    }

    public static Cube Instantiate(Cube prefab, Vector3 pos)
    {
        Cube _cube = Instantiate(prefab, pos, Quaternion.identity) as Cube;
        _cube.m_isBurn = false;
        _cube.m_type = CubeType.Cube;
        return _cube;
    }
}
