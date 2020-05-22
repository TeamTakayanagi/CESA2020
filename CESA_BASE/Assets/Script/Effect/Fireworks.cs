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

    // Start is called before the first frame update
    new void Start()
    {
        m_moveVector = Vector3.up;
        m_createWait = AdjustParameter.Result_Constant.WAIT_TIME;
        m_staet = State.launch;
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        switch (m_staet)
        {
            // 打ち上げた際の処理
            case State.launch:
                //transform.DOMoveY(AdjustParameter.Result_Constant.END_FIRE_POS_Y, AdjustParameter.Result_Constant.LAUNCH_TIME);
                transform.position = Vector3.Lerp(transform.position, new Vector3(
                    transform.position.x, AdjustParameter.Result_Constant.END_FIRE_POS_Y, transform.position.z), Time.deltaTime);
                if (Mathf.Ceil(transform.position.y) == AdjustParameter.Result_Constant.END_FIRE_POS_Y)
                {
                    m_staet = State.create;
                }
                break;

            // 生成までの待ち時間
            case State.create:
                m_createWait -= Time.deltaTime;
                if(m_createWait <= 0.0f)
                {
                    EffectManager.Instance.EffectCreate(EffectType.fireworks, transform.position, Quaternion.identity);
                    DestroyImmediate(gameObject);

                }
                break;
        }

    }
}
