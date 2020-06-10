using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Effekseer;
using DG.Tweening;

public class Fireworks : EffekseerEmitter
{
    private enum State
    {
        launch = 0,
        create,
    }

    private float m_createWait;
    private State m_staet;
    private bool m_isSE = false;

    new void Start()
    {
        m_moveVector = new Vector3(0.0f, m_target.y - transform.position.y, 0.0f);
        m_createWait = AdjustParameter.Production_Constant.WAIT_TIME;
        m_staet = State.launch;
        if(m_isSE)
            Sound.Instance.PlaySE("se_hanabi_bef", gameObject.GetInstanceID());
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
                m_createWait -= Time.deltaTime;
                if(m_createWait <= 0.0f)
                {
                    EffectManager.Instance.EffectCreate(EffectManager.Instance.GetFireworks(), transform.position, transform.localScale, Quaternion.identity);
                    if (m_isSE)
                        Sound.Instance.PlaySE("se_hanabi_aft", gameObject.GetInstanceID());
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
        fire.m_isSE = isSE;

        return fire;
    }

}
