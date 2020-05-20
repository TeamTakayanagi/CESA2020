using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Effekseer;

public class EffectManager : SingletonMonoBehaviour<EffectManager>
{
    [SerializeField]
    private List<EffekseerEmitter> m_effectList = new List<EffekseerEmitter>();                 // エフェクトプレハブ

    // Start is called before the first frame update
    void Start()
    {
        m_effectList.Sort((a, b) => a.effectType - b.effectType);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public EffekseerEmitter EffectCreate(EffekseerEmitter.EffectType type, Vector3 pos, Quaternion rot)
    {
        EffekseerEmitter effect = Instantiate(m_effectList[(int)type]) as EffekseerEmitter;
        effect.transform.parent = transform;
        effect.transform.localPosition = pos;
        effect.transform.localRotation = rot;

        return effect;
    }
}
