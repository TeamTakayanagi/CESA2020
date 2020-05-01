using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuseFire : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 target = transform.GetChild(0).position;
        gameObject.GetComponent<Renderer>().material.SetVector("_Target", target);
    }
}
