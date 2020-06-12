using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioControll : MonoBehaviour
{

    private AudioSource m_audio;
    // Start is called before the first frame update
    void Start()
    {
        m_audio = GetComponent<AudioSource>();
    }

    private IEnumerator EndSE()
    {
        while(true)
        {
            yield return new WaitForFixedUpdate();
            if (!m_audio.isPlaying)
            {
                m_audio.clip = null;
                yield break;
            }
        }
    }
}
