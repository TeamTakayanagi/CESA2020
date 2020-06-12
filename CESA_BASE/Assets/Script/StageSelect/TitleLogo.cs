using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleLogo : MonoBehaviour
{
    private enum LogoChild
    {
        BackToGround = 0,
        Fireworks,
        BackToLetter,
        Letter,
        Rat,
        ALL
    }

    public enum LogoStep
    {
        None = 0,
        BackLetter,
        Rat,
        Letter,
        Fireworks,
        StepLast
    }
    private LogoStep m_step = 0;
    public LogoStep Step
    {
        get
        {
            return m_step;
        }
    }

    private Image[] m_logoChild = null;

    private float m_alphaSpeed = 1;
    private float m_LetterMove = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        m_logoChild = GetComponentsInChildren<Image>();

        // 文字背景のアルファ値を抜く
        m_logoChild[(int)LogoChild.BackToLetter].color = Color.clear;
        // タイトルの文字
        m_logoChild[(int)LogoChild.Letter].transform.position =
            new Vector3(m_logoChild[(int)LogoChild.BackToLetter].transform.position.x,
                        m_logoChild[(int)LogoChild.BackToLetter].transform.position.y,
                        m_logoChild[(int)LogoChild.BackToLetter].transform.position.z - 2);
        // ネズミ
        m_logoChild[(int)LogoChild.Rat].transform.localScale = Vector3.zero;
        // 花火
        m_logoChild[(int)LogoChild.Fireworks].color = Color.clear;
        m_logoChild[(int)LogoChild.Fireworks].transform.rotation = Quaternion.Euler(transform.root.rotation.x, 0, 45);
    }

    // Update is called once per frame
    void Update()
    {
        if (TitleMgr.Instance.Step != TitleMgr.TitleStep.Charm)
            return;

        // 背景のみ描画されている
        if (m_step == LogoStep.None)
        {
            m_step = LogoStep.BackLetter;
        }

        // 文字の背景が現れる
        // アルファ値で管理
        if (m_step == LogoStep.BackLetter)
        {
            m_logoChild[(int)LogoChild.BackToLetter].color += Color.white * m_alphaSpeed * Time.deltaTime;
            if (m_logoChild[(int)LogoChild.BackToLetter].color.a >= 1)
            {
                m_step = LogoStep.Rat;
            }

        }

        // ネズミが出てくる
        // あわよくば少し回転させる
        else if (m_step == LogoStep.Rat)
        {
            m_logoChild[(int)LogoChild.Rat].transform.localScale += Vector3.one * Time.deltaTime;
            if (m_logoChild[(int)LogoChild.Rat].transform.localScale.x > 1)
            {
                m_logoChild[(int)LogoChild.Rat].transform.localScale = Vector3.one;

                m_step = LogoStep.Letter;
            }
        }

        // タイトルの文字を出す
        // world空間を利用する
        else if (m_step == LogoStep.Letter)
        {
            m_logoChild[(int)LogoChild.Letter].transform.position += Vector3.forward * m_LetterMove;
            if (m_logoChild[(int)LogoChild.Letter].transform.position.z > m_logoChild[(int)LogoChild.BackToLetter].transform.position.z)
            {
                m_logoChild[(int)LogoChild.Letter].transform.position =
                        new Vector3(m_logoChild[(int)LogoChild.BackToLetter].transform.position.x,
                                    m_logoChild[(int)LogoChild.BackToLetter].transform.position.y,
                                    m_logoChild[(int)LogoChild.BackToLetter].transform.position.z);

                m_step = LogoStep.Fireworks;
            }
        }

        // タイトルが定位置に行ったら花火が出る
        // アルファ値と回転を利用
        else if (m_step == LogoStep.Fireworks)
        {
            // 回転処理
            float _rotZ = m_logoChild[(int)LogoChild.Fireworks].transform.rotation.z;
            _rotZ = Mathf.Lerp(_rotZ, 0, Time.deltaTime);
            m_logoChild[(int)LogoChild.Fireworks].transform.rotation = new Quaternion(transform.root.rotation.x, 0, _rotZ, 1);

            // アルファ値処理
            float _alpha = m_logoChild[(int)LogoChild.Fireworks].color.a;
            _alpha = Mathf.Clamp(Mathf.Lerp(_alpha, 1.5f, Time.deltaTime), 0, 1);
            m_logoChild[(int)LogoChild.Fireworks].color = new Color(1, 1, 1, _alpha);

            if (m_logoChild[(int)LogoChild.Fireworks].color.a == 1)
            {
                TitleMgr.Instance.Step = TitleMgr.TitleStep.LogoUp;
            }
        }
    }

    private void LateUpdate()
    {
        if (TitleMgr.Instance.Step == TitleMgr.TitleStep.MoveSkip)
        {
            SkipMovement(); 
        }
    }

    private void SkipMovement()
    {
        m_logoChild[(int)LogoChild.Fireworks].transform.rotation = new Quaternion(transform.root.rotation.x, 0, 0, 1);
        m_logoChild[(int)LogoChild.Fireworks].color = Color.white;
        m_logoChild[(int)LogoChild.BackToLetter].color = Color.white;
        m_logoChild[(int)LogoChild.Letter].transform.position = m_logoChild[(int)LogoChild.BackToLetter].transform.position;
        m_logoChild[(int)LogoChild.Rat].transform.localScale = Vector3.one;

        TitleMgr.Instance.Step = TitleMgr.TitleStep.Wite;
    }
    
    public bool AddAlpha(bool _flg = true)
    {
        float _alpha = 0;
        float _time = Time.deltaTime;
        for (int i = 0; i < (int)LogoChild.ALL; i++)
        {
            _alpha = m_logoChild[i].color.a;
            _alpha = Mathf.Clamp(Mathf.Lerp(_alpha, (_flg ? 1.5f : -0.5f), _time), 0, 1);
            m_logoChild[i].color = Color.white * _alpha;
        }

        return _alpha == 0 || _alpha == 1 ? true : false;
    }
}
