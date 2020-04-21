using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Result : MonoBehaviour
{
    public GameObject resultClear;
    public GameObject resultGameover;

    public GameObject clearText;
    public GameObject gameoverText;

    public GameObject clearButton;
    public GameObject gameoverButton;



    public AnimationCurve animCurve = AnimationCurve.Linear(0, 0, 1, 1);

    public Vector3 inPos_text;        // スライドイン後の位置(クリア、ゲームオーバー
    public Vector3 inPos_button;      // スライドイン後の位置（ボタン
    public float duration = 1.0f;    // スライド時間（秒）


    // Start is called before the first frame update
    void Start()
    {
        resultClear.SetActive(false);
        resultGameover.SetActive(false);
        clearButton.SetActive(false);
        gameoverButton.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        // テスト用
        if (Input.GetKeyDown(KeyCode.M))
        {
            SetGameover();
        }

    }

    //--- 呼び出し用 ---
    public void SetClear()
    {
        // Clear UI 表示
        resultClear.SetActive(true);
        clearButton.SetActive(true);

        StartCoroutine(StartSlidePanel(true));
    }

    public void SetGameover()
    {
        resultGameover.SetActive(true);
        gameoverButton.SetActive(true);

        StartCoroutine(StartSlidePanel(false));        
    }

    //--- テキストのスライド ---
    private IEnumerator StartSlidePanel(bool isClear)
    {     

        float startTime = Time.time;    // 開始時間
        Vector3 moveDistance_text;            // 移動距離および方向
        Vector3 moveDistance_button;            // 移動距離および方向

        if (isClear)
        {
            // クリア
            Vector3 startPos_text = clearText.transform.localPosition;  // 開始位置
            Vector3 startPos_button = clearButton.transform.localPosition;  // 開始位置

            moveDistance_text = (inPos_text - startPos_text);
            moveDistance_button = (inPos_button - startPos_button);

            while ((Time.time - startTime) < duration)
            {
                clearText.transform.localPosition = startPos_text + moveDistance_text * animCurve.Evaluate((Time.time - startTime) / duration);
                clearButton.transform.localPosition = startPos_button + moveDistance_button * animCurve.Evaluate((Time.time - startTime) / duration);
                yield return 0;        // 1フレーム後、再開

            }
        }
        else
        {
            // ゲームオ―バー

            Vector3 startPos = gameoverText.transform.localPosition;  // 開始位置
            Vector3 startPos_button = gameoverButton.transform.localPosition;  // 開始位置
            
            moveDistance_text = (inPos_text - startPos);
            moveDistance_button = (inPos_button - startPos_button);

            while ((Time.time - startTime) < duration)
            {
                gameoverText.transform.localPosition = startPos + moveDistance_text * animCurve.Evaluate((Time.time - startTime) / duration);
                gameoverButton.transform.localPosition = startPos_button + moveDistance_button * animCurve.Evaluate((Time.time - startTime) / duration);

                yield return 0;        // 1フレーム後、再開

            }
        }
    }

}
