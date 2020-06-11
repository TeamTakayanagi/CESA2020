using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.localEulerAngles = new Vector3(ProcessedtParameter.Camera_Constant.FIRST_ROT_X, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
