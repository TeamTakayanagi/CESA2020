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
        m_moveVector = new Vector3(0.0f, AdjustParameter.Result_Constant.END_FIRE_POS_Y - transform.position.y, 0.0f);
        m_createWait = AdjustParameter.Result_Constant.WAIT_TIME;
        m_staet = State.launch;
        base.Start();
    }

    new void Update()
    {
        switch (m_staet)
        {
            // 打ち上げた際の処理
            case State.launch:
                transform.position += m_moveVector * Time.deltaTime;
                if (transform.position.y >= AdjustParameter.Result_Constant.END_FIRE_POS_Y)
                {
                    m_staet = State.create;
                }
                break;

            // 生成までの待ち時間
            case State.create:
                m_createWait -= Time.deltaTime;
                if(m_createWait <= 0.0f)
                {
                    EffectManager.Instance.EffectCreate(EffectManager.Instance.GetFireworks(), transform.position, Quaternion.identity);
                    DestroyImmediate(gameObject);
                }
                break;
        }
        base.Update();
    }
}
