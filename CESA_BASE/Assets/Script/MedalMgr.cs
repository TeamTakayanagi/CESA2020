using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedalMgr : MonoBehaviour
{
    [SerializeField]
    private Star MedalPrefab = null;

    private RectTransform m_myRectTrans = null;
    private Star m_medal = null;

    //StageState

    // Start is called before the first frame update
    void Start()
    {
        m_myRectTrans = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MedalCreate(Sprite _sprite, float _offsetNum, float _scale = 1.0f)
    {
        Vector3 _offset = m_myRectTrans.sizeDelta * _offsetNum;
        m_medal = Instantiate(MedalPrefab, transform.position + _offset, Quaternion.identity, transform);
        m_medal.SetParam(_sprite, Vector3.one * _scale);
    }
}
