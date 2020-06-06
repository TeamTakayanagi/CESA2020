using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuseBase : MonoBehaviour
{
    protected const float childScale = 0.1f;

    public enum FuseState
    {
        None,       // 場に置かれている
        UI,         // UIになっている
        Burn,       // 燃えている
        Out,        // 燃え尽きた後
        Wet         // 濡れている
    }

    [SerializeField]
    protected Texture2D m_fuseTex = null;

    protected FuseState m_state = FuseState.None;
    protected Vector3 m_targetDistance = Vector3.zero;


    protected Transform m_childModel = null;
    protected Transform m_childTarget = null;
    protected Renderer m_childRenderer = null;
    protected List<Spark> m_haveEffect = new List<Spark>();                   // この導火線についているエフェクト   
    public List<BoxCollider> HaveEffect(Spark spark)
    {
        List<Spark> sparkList = m_haveEffect;
        //sparkList.Remove(spark);
        List<BoxCollider> collList = new List<BoxCollider>();
        for (int i = 0; i < sparkList.Count; ++i)
        {
            collList.AddRange(sparkList[i].GetComponents<BoxCollider>());
        }
        return collList;
    }
    public FuseState State
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
    public Transform ChildTarget
    {
        get
        {
            return m_childTarget;
        }
        set
        {
            m_childTarget = value;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
