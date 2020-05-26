﻿using System.Collections;
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
        DontDestroyOnLoad(gameObject);
    }

    public EffekseerEmitter EffectCreate(EffekseerEmitter.EffectType type, Vector3 pos, Quaternion rot)
    {
        EffekseerEmitter effect = Instantiate(m_effectList[(int)type]) as EffekseerEmitter;
        effect.transform.parent = transform;
        effect.transform.localPosition = pos;
        effect.transform.localRotation = rot;

        return effect;
    }
    public EffekseerEmitter EffectCreate(EffekseerEmitter.EffectType type, Vector3 pos, Quaternion rot, Transform parent)
    {
        EffekseerEmitter effect = Instantiate(m_effectList[(int)type]) as EffekseerEmitter;
        effect.transform.SetParent(parent, true);
        effect.transform.position = pos;
        effect.transform.localRotation = rot;

        return effect;
    }
    public void DestoryEffects()
    {
        EffekseerEmitter[] effectList = FindObjectsOfType<EffekseerEmitter>();
        for(int i = 0; i < effectList.Length; ++i)
        {
            EffekseerEmitter effect = effectList[i];
            //if (effect.effectType == EffekseerEmitter.EffectType.Click)
            //    continue;

            DestroyImmediate(effect.gameObject);
        }
    }
    public EffekseerEmitter.EffectType GetFireworks()
    {
        return (EffekseerEmitter.EffectType)Random.Range(
            (int)EffekseerEmitter.EffectType.fireworks_blue, (int)EffekseerEmitter.EffectType.fireworks_orange + 1);
    }
}
