using UnityEngine;

public class TitleMgr : SingletonMonoBehaviour<TitleMgr>
{
    // 後からコンスト定数に持っていく
    private const float CHARM_TIME = 1.5f;
    private const float GUID_TIME = 1.0f;
    private const float UP_SPEED = 0.5f;

    private readonly Vector3 InitCameraPos = new Vector3(0.0f, 80.0f, -10.0f);
    private readonly Vector3 InitLogoPos = new Vector3(0.0f, -20.0f, 0.0f);
    private readonly Vector3 m_initGuidPos = new Vector3(0.0f, -50.0f, -20.0f);
    private readonly Vector3 LogoUpPos = Vector3.zero;

    private readonly Quaternion InitCameraRot = Quaternion.Euler(new Vector3(-60, 0, 0));
    private readonly Quaternion InitObjRot = Quaternion.Euler(new Vector3(30, 0, 0));
    private readonly Quaternion LastCameraRot = Quaternion.Euler(new Vector3(30, 0, 0)); 

    public enum TitleStep
    {
        Scroll = 0,
        Charm,
        LogoUp,
        Wite,
        Retreat,
        Select,
        Max
    }


    [SerializeField]
    private GameObject m_guidPrefab = null;
    private TitleStep m_step = TitleStep.Scroll;

    private float m_delayCounter = 0;
    private GameObject m_camera = null;
    private GameObject m_logo = null;
    private GameObject m_guid = null;
    private Canvas m_logoCanvas = null;

    public TitleStep Step
    {
        get
        {
            return m_step;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_logo = transform.GetChild(0).gameObject;
        m_logoCanvas = transform.GetComponent<Canvas>();
        m_camera = Camera.main.gameObject;
        // マウス制御クラスにカメラの情報を渡す
        InputMouse.RoadCamera();

        m_camera.transform.position = InitCameraPos;
        m_camera.transform.rotation = InitCameraRot;
        m_logo.transform.rotation = InitObjRot;
        m_logo.transform.position = m_logoCanvas.transform.position + m_logo.transform.rotation * InitLogoPos;    
    }

    // Update is called once per frame
    void Update()
    {
        // カメラが回転してくる
        if (m_step == TitleStep.Scroll)
        {
            m_camera.transform.Rotate(InitCameraRot * Vector3.right * 0.3f);

            if (m_camera.transform.rotation.x >= LastCameraRot.x)
            {
                m_camera.transform.rotation = LastCameraRot;
                m_step = TitleStep.Charm;
            }
        }

        // タイトルロゴを魅せる
        else if (m_step == TitleStep.Charm)
        {
            m_delayCounter += Time.deltaTime;
            if (m_delayCounter > CHARM_TIME)
            {
                m_delayCounter = 0;
                m_step = TitleStep.LogoUp;
            }
        }

        // タイトルロゴを少し上にずらす
        else if (m_step == TitleStep.LogoUp)
        {
            m_logo.transform.Translate(Vector3.up * UP_SPEED);

            if (m_logo.transform.position.y >= (m_logoCanvas.transform.position + m_logo.transform.rotation * LogoUpPos).y)
            {
                m_logo.transform.position = m_logoCanvas.transform.position + m_logo.transform.rotation * LogoUpPos;
                m_delayCounter += Time.deltaTime;

                if (m_delayCounter >= GUID_TIME)
                {
                    m_guid = Instantiate(m_guidPrefab, m_logoCanvas.transform.transform.position + InitObjRot * m_initGuidPos,
                        InitObjRot, transform);
                    m_step = TitleStep.Wite;
                }
            }
        }

        // クリック待機
        else if (m_step == TitleStep.Wite)
        {
            if (Input.GetMouseButtonUp(0))
            {
                m_step = TitleStep.Retreat;
            }
        }

        // クリック時
        else if (m_step == TitleStep.Retreat)
        {
            m_logo.transform.Translate(Vector3.up * UP_SPEED);
            m_guid.transform.Translate(Vector3.back * UP_SPEED);

            if (!m_logo.GetComponent<OutsideCanvas>().isVisible &&
                !m_guid.GetComponent<OutsideCanvas>().isVisible)
            {
                Destroy(m_logo.gameObject);
                Destroy(m_guid.gameObject);
                //Destroy(m_logoCanvas.gameObject);
                m_step = TitleStep.Select;
            }
        }

        // タイトル演出スキップ
        if (m_step < TitleStep.Wite && Input.GetMouseButtonUp(0))
        {
            m_step = TitleStep.Wite;
            m_camera.transform.rotation = LastCameraRot;
            m_logo.transform.position = transform.position + m_logo.transform.rotation * LogoUpPos;
            m_guid = Instantiate(m_guidPrefab, transform.position + InitObjRot * m_initGuidPos, InitObjRot, transform);
        }
    }
}
