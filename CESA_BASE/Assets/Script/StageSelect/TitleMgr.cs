using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleMgr : SingletonMonoBehaviour<TitleMgr>
{
    // 後からコンスト定数に持っていく
    private const float m_charmTime = 3;
    private const float m_guidTime = 1.0f;

    private enum TitleStep
    {
        Start = 0,
        Scroll,
        Charm,
        LogoUp,
        Guid,
        Wite,
        Retreat,
        Select,
        Max
    }
    private TitleStep m_step;

    [SerializeField]
    private GameObject m_guidPrefab = null;

    private Canvas m_logoCanvas = null;
    private GameObject m_camera = null;
    private GameObject m_logo = null;
    private GameObject m_guid = null;
    private GameObject m_stageCanvas = null;

    private Vector3 m_initCameraPos = new Vector3(0, 80, -10);
    private Vector3 m_initLogoPos = new Vector3(0, -20, 0);
    private Quaternion m_initCameraRot = Quaternion.Euler(new Vector3(-60, 0, 0));
    private Quaternion m_initLogoRot = Quaternion.Euler(new Vector3(30, 0, 0));
    private Vector3 m_initGuidPos = new Vector3(0, -50, -20);
    private Quaternion m_initGuidRot = Quaternion.Euler(new Vector3(30, 0, 0));

    private Quaternion m_lastCameraRot = Quaternion.Euler(30, 0, 0);
    
    private Vector3 m_logoupPos = new Vector3(0, 0, 0);

    //---その打ち消す-------------------
    private Vector3 m_initPos = new Vector3(0, 200, -10);
    private Vector3 m_charmPos = new Vector3(0, 0, 0);
    private Vector3 m_cameraLastPos = new Vector3(0, 80, -10);
    private Vector3 m_guidPos = new Vector3(0, -25, 85);
    //----------------------------------

    private float m_upSpeed = 0.5f;
    //private float m_downSpeed = 1.0f;
    private float m_delay = 0;

    public int Step
    {
        get
        {
            return (int)m_step;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_logo = GameObject.FindGameObjectWithTag("UI/Logo");
        m_logoCanvas = m_logo.transform.root.GetComponent<Canvas>();
        m_camera = GameObject.FindGameObjectWithTag("MainCamera");
        m_stageCanvas = GameObject.FindGameObjectWithTag("UI/Stage");

        m_camera.transform.position = m_initCameraPos;
        m_camera.transform.rotation = m_initCameraRot;
        m_logo.transform.rotation = m_initLogoRot;
        m_logo.transform.position = m_logoCanvas.transform.position + m_logo.transform.rotation * m_initLogoPos;
        
    }

    // Update is called once per frame
    void Update()
    {
        // 
        if (m_step == TitleStep.Start)
        {
            m_step++;
        }

        // カメラが回転してくる
        if (m_step == TitleStep.Scroll)
        {
            m_camera.transform.Rotate(m_initCameraRot * Vector3.right * 0.3f * Time.deltaTime * 60);

            if (m_camera.transform.rotation.x >= m_lastCameraRot.x)
            {
                m_camera.transform.rotation = m_lastCameraRot;
                m_step++;
            }
        }

        // タイトルロゴを魅せる
        if (m_step == TitleStep.Charm)
        {
            m_delay += Time.deltaTime;

            if (m_delay > m_charmTime)
            {
                m_delay = 0;
                m_step++;
            }
        }

        // タイトルロゴを少し上にずらす
        if (m_step == TitleStep.LogoUp)
        {
            m_logo.transform.Translate(Vector3.up * m_upSpeed * Time.deltaTime * 60);

            if (m_logo.transform.position.y >= (m_logoCanvas.transform.position + m_logo.transform.rotation * m_logoupPos).y)
            {
                m_logo.transform.position = m_logoCanvas.transform.position + m_logo.transform.rotation * m_logoupPos;
                m_delay += Time.deltaTime;

                if (m_delay >= m_guidTime)
                {
                    m_step++;
                }
            }
        }

        // 
        if (m_step == TitleStep.Guid)
        {
            m_guid = Instantiate(m_guidPrefab, m_logoCanvas.transform.transform.position + m_initGuidRot * m_initGuidPos, Quaternion.Euler(30, 0, 0), transform);
            //m_guid.transform.rotation = m_initGuidRot;

            m_step++;
        }

        // クリック待機
        if (m_step == TitleStep.Wite)
        {
            if (Input.GetMouseButtonUp(0))
            {
                m_step++;
            }
        }

        // クリック時
        if (m_step == TitleStep.Retreat)
        {
            m_logo.transform.Translate(Vector3.up * m_upSpeed * Time.deltaTime*60);
            m_guid.transform.Translate(Vector3.back * m_upSpeed * Time.deltaTime*60);

            if (!m_logo.GetComponent<OutsideCanvas>().isVisible && !m_guid.GetComponent<OutsideCanvas>().isVisible)
            {
                Destroy(m_logo.gameObject);
                Destroy(m_guid.gameObject);

                m_step++;
            }
        }

        // ステージ選択画面
        //if (m_step == TitleStep.Select)
        //{
        //    Instantiate(m_stagePrefab, m_stageCanvas.transform);

        //    m_step++;
        //}

        // タイトル演出スキップ
        if (m_step < TitleStep.Guid && Input.GetMouseButtonUp(0))
        {
            m_step = TitleStep.Wite;
            m_camera.transform.rotation = m_lastCameraRot;
            m_logo.transform.position = m_logoCanvas.transform.position + m_logo.transform.rotation * m_logoupPos;
            m_guid = Instantiate(m_guidPrefab, m_logoCanvas.transform.transform.position + m_initGuidRot * m_initGuidPos, m_initGuidRot, transform);

        }

    }

    private void LateUpdate()
    {


    }
}
