using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title : MonoBehaviour
{
    // 後からコンスト定数に持っていく
    private const float m_charmTime = 3;
    private const float m_guidTime = 1.0f;
    // ==============================

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
    [SerializeField]
    private GameObject m_stagePrefab = null;

    private Canvas m_logoCanvas = null;
    private GameObject m_camera = null;
    private GameObject m_logo = null;
    private GameObject m_guid = null;
    private GameObject m_stageCanvas = null;

    private Vector3 m_initPos = new Vector3(0, 200, -10);
    private Vector3 m_charmPos = new Vector3(0, 0, -10);
    private Vector3 m_logoupPos = new Vector3(0, 20, 85);
    private Vector3 m_guidPos = new Vector3(0, -25, 85);

    private Quaternion m_initRot = new Quaternion();

    private float m_upSpeed = 0.5f;
    private float m_downSpeed = 1.0f;
    private float m_delay = 0;

    private void Awake()
    {
        m_initRot = Quaternion.Euler(new Vector3(-60, 0, 0));
    }

    // Start is called before the first frame update
    void Start()
    {
        m_logo = GameObject.FindGameObjectWithTag("UI/Logo");
        m_logoCanvas = m_logo.transform.root.GetComponent<Canvas>();
        m_camera = GameObject.FindGameObjectWithTag("MainCamera");
        m_stageCanvas = GameObject.FindGameObjectWithTag("UI/Stage");

    }

    // Update is called once per frame
    void Update()
    {
        // 
        if (m_step == TitleStep.Start)
        {
            //m_camera.transform.position = m_initPos;
            m_camera.transform.position = m_charmPos;
            m_camera.transform.rotation = m_initRot;

            m_step++;
        }

        // カメラが回転してくる
        if (m_step == TitleStep.Scroll)
        {
            m_camera.transform.Rotate(m_initRot * Vector3.right * 0.3f * Time.deltaTime * 60);

            if (m_camera.transform.rotation.x >= 0)
            {
                m_camera.transform.rotation = Quaternion.identity;
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

            if (m_logo.transform.position.y >= m_logoupPos.y)
            {
                m_logo.transform.position = m_logoupPos;
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
            m_guid = Instantiate(m_guidPrefab, m_guidPos, Quaternion.identity, transform);
            
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
                m_step++;
            }
        }

        // ステージ選択画面
        if (m_step == TitleStep.Select)
        {
            Instantiate(m_stagePrefab, m_stageCanvas.transform);

            m_step++;
        }

        // タイトル演出スキップ
        if (m_step < TitleStep.Guid && Input.GetMouseButtonUp(0))
        {
            m_step = TitleStep.Wite;
            m_camera.transform.rotation = Quaternion.identity;
            m_logo.transform.position = m_logoupPos;
            m_guid = Instantiate(m_guidPrefab, m_guidPos, Quaternion.identity, transform);

        }

    }

    private void LateUpdate()
    {


    }
}
