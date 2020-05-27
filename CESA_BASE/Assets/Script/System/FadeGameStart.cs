using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeGameStart : MonoBehaviour
{
    private readonly AnimationCurve m_animCurve = AnimationCurve.Linear(0, 0, 1, 1);    // sprite移動用
    
    private enum FadeStep
    {
        None = 0,
        StepFuse,   // 導火線がスライド
        StepIdle,   // 待機（ここでシーンロード？
        StepMouse,  // ネズミがかじる
        StepEnd,
        Max
    }

    private enum SpriteType
    {
        Fuse,
        Mouse
    }

    private FadeStep m_step = FadeStep.None;
    [SerializeField]
    private SpriteType m_Sprite = SpriteType.Fuse;

    private Vector3 m_StartPos = Vector3.zero;
    private Vector3 m_EndPos = Vector3.zero;

    private int m_fadeCnt;
    private bool m_fadeFlag;

    [SerializeField]
    private ParticleSystem m_particle = null;

    // Start is called before the first frame update
    void Start()
    {
        m_particle.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        // 動作確認用キー
        if (Input.GetKeyDown(KeyCode.F))
            FadeOut();
        if (Input.GetKeyDown(KeyCode.G))
            FadeIn();


        if (m_fadeFlag == true)
        {
            m_fadeCnt++;
            if (m_fadeCnt > 60)
            {
                m_step += 1;
                m_fadeCnt = 0;
            }
        }

        if (m_step == FadeStep.None)
            return;

        if (m_step == FadeStep.StepFuse)
        {
            if (m_Sprite == SpriteType.Fuse)
            {
                m_StartPos = transform.localPosition;
                m_EndPos = transform.localPosition;
                m_EndPos.x = 0f;

                StartCoroutine(FadeUpdate(m_StartPos, m_EndPos));
            }
        }

        if(m_step == FadeStep.StepIdle)
        {
            m_fadeFlag = false;
            // シーン遷移とか？
        }

        if (m_step == FadeStep.StepMouse)
        {
            if (m_Sprite == SpriteType.Mouse)
            {
                m_particle.Play();
                m_StartPos = transform.localPosition;
                m_EndPos = transform.localPosition;
                m_EndPos.x *= -1;
                StartCoroutine(FadeUpdate(m_StartPos, m_EndPos));
            }
            m_step = FadeStep.StepEnd;
        }

        if (m_step == FadeStep.StepEnd)
        {
            m_fadeFlag = false;
        }

    }

    // 呼び出し
    public void FadeOut()
    {
        m_step = FadeStep.StepFuse;
        m_fadeFlag = true;
    }

    public void FadeIn()
    {
        m_step = FadeStep.StepMouse;
        m_fadeFlag = true;
    }


    public IEnumerator FadeUpdate(Vector3 start, Vector3 target)
    {
        float startTime = Time.time;             // 開始時間
        Vector3 moveDistance;        // 移動距離および方向
        moveDistance = target - start;

        while ((Time.time - startTime) < AdjustParameter.Fade_Constant.FADE_DURATION)
        {
            transform.localPosition = start + moveDistance * m_animCurve.Evaluate((Time.time - startTime) / AdjustParameter.Fade_Constant.FADE_DURATION);
            yield return 0;
        }
    }

}
