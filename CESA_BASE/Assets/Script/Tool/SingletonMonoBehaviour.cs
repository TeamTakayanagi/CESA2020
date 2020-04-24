using UnityEngine;
using System;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{

    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                Type t = typeof(T);

                instance = (T)FindObjectOfType(t);
                if (instance == null)
                {
                    Debug.LogError(t + " をアタッチしているGameObjectはありません");
                    return null;
                }
            }
            return instance;
        }
    }

    virtual protected void Awake()
    {
        // 他のゲームオブジェクトにアタッチされているか調べる
        // アタッチされている場合は破棄する。
        CheckInstance();
    }

    virtual protected void OnDestroy()
    {
        instance = null;
    }


    protected void CheckInstance()
    {
        if (instance == null)
        {
            instance = this as T;
            return;
        }
        else if (Instance == this)
        {
            return;
        }
        Destroy(this);
    }
}