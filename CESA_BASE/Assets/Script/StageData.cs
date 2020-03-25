using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StagaData : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer m_notopenSprite;
    [SerializeField]
    private SpriteRenderer m_openSprite;
    [SerializeField]
    private SpriteRenderer m_crearSprite;
    [SerializeField]
    private SpriteRenderer m_completeSprite;

    private SpriteRenderer m_mySprite;

    

    // Start is called before the first frame update
    void Start()
    {
        m_mySprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // 
    }

    
}
