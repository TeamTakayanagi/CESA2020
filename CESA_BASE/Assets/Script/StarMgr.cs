using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarMgr : MonoBehaviour
{
    private enum StageState
    {
        NOT_OPEN = 0,       // ステージ未開放
        OPEN,               // ステージ選択可能
        CREAR,              // ステージクリア(目標未達成)
        COMPLETE,           // ステージクリア(目標達成)
        STATE_MAX
    }

    // ステージ数
    [SerializeField]
    private int m_stageNum;

    [SerializeField]
    private GameObject popupPrefab;
    [SerializeField]
    private GameObject StarPrefab;
    [SerializeField]
    private Vector3[] m_starOffset = new Vector3[3];
    [SerializeField]
    private int m_starNum = 3;

    private SpriteRenderer m_mySprite = null;

    private GameObject m_popup;
    private GameObject m_star;

    //StageState

    public bool PopUp
    {
        get
        {
            return m_popup;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < m_starNum; i++)
        {
            m_star = Instantiate(StarPrefab, transform.position + m_starOffset[i], Quaternion.identity, transform);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetParam(int _setNum, int _spriteState)
    {
        m_stageNum = _setNum;
       // m_
    }

    public void Click()
    {
        if (!transform.GetComponentInParent<StageMgr>().popFlg)
        {
            m_popup = Instantiate(popupPrefab, transform.root.position, Quaternion.identity, transform.root);
            transform.GetComponentInParent<StageMgr>().popFlg = true;
        }
    }

    //private void SetImage(StageState _state, SpriteRenderer _sprite)
    //{
    //    switch (_state)
    //    {
    //        case StageState.NOT_OPEN:
    //            m_mySprite = m_notopenSprite;
    //            break;

    //        case StageState.OPEN:
    //            m_mySprite = m_openSprite;
    //            break;

    //        case StageState.CREAR:
    //            m_mySprite = m_crearSprite;
    //            break;

    //        case StageState.COMPLETE:
    //            m_mySprite = m_completeSprite;
    //            break;

    //        default:
    //            m_mySprite = m_notopenSprite;
    //            break;
    //    }
    //}
}
