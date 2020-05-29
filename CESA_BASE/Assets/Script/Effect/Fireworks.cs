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

    float m_createWait;
    State m_staet;

    new void Start()
    {
        m_moveVector = new Vector3(0.0f, AdjustParameter.Production_Constant.END_FIRE_POS_Y - transform.position.y, 0.0f);
        m_createWait = AdjustParameter.Production_Constant.WAIT_TIME;
        m_staet = State.launch;
        Sound.Instance.PlaySE("se_hanabi_bef", gameObject.GetInstanceID());
        base.Start();
    }

    new void Update()
    {
        switch (m_staet)
        {
            // 打ち上げた際の処理
            case State.launch:
                transform.position += m_moveVector * Time.deltaTime * Vector3.Dot(transform.localScale, Vector3.one);
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
                    Sound.Instance.PlaySE("se_hanabi_aft", gameObject.GetInstanceID());
                    DestroyImmediate(gameObject);
                }
                break;
        }
        base.Update();
    }
}
