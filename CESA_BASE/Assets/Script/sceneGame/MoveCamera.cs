using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField]
    private GameObject target;

    GameObject result;
    private Result resultScript;

    // Start is called before the first frame update
    void Start()
    {
        result = GameObject.Find("ResultController");
        resultScript = result.GetComponent<Result>();
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.LookAt(target.transform);

        // 花火が一定の高さに達したらUI表示
        if (target.transform.position.y > 30.0f)
        {
            resultScript.SetClear();
        }
    }
}
