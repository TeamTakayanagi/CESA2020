using Effekseer;
using UnityEngine;

public class Fireworks : EffekseerEmitter
{
    private enum State
    {
        launch = 0,
        create,
    }

    private State m_staet;
    private bool m_isSound = false;         // 音を再生するかどうか
    private float m_waitCount;             // 

    new void Start()
    {
        m_moveVector = new Vector3(0.0f, m_target.y - transform.position.y, 0.0f);
        m_waitCount = AdjustParameter.Production_Constant.WAIT_TIME;
        m_staet = State.launch;
        if(m_isSound)
            Sound.Instance.PlaySE(Audio.SE.FireWorks_Before, gameObject.GetInstanceID());
        base.Start();
    }

    new void Update()
    {
        switch (m_staet)
        {
            // 打ち上げた際の処理
            case State.launch:
                transform.position += m_moveVector * Time.deltaTime;
                if (transform.position.y >= m_target.y)
                {
                    m_staet = State.create;
                }
                break;

            // 生成までの待ち時間
            case State.create:
                m_waitCount -= Time.deltaTime;
                if(m_waitCount <= 0.0f)
                {
                    EffectManager.Instance.EffectCreate(EffectManager.Instance.GetFireworks(), transform.position, transform.localScale, Quaternion.identity);
                    if (m_isSound)
                        Sound.Instance.PlaySE(Audio.SE.FireWorks_After, gameObject.GetInstanceID());
                    DestroyImmediate(gameObject);
                }
                break;
        }
        base.Update();
    }

    public static Fireworks Instantiate(EffectType type, Vector3 pos, Vector3 move, Vector3 scale, Quaternion rot, bool isSE)
    {
        EffekseerEmitter effect = EffectManager.Instance.EffectCreate(type, pos, move, scale, rot) as EffekseerEmitter;
        if (!effect)
            return null;

        Fireworks fire = effect.GetComponent<Fireworks>();
        fire.m_isSound = isSE;

        return fire;
    }

}
