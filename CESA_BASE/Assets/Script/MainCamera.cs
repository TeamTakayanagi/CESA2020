using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    bool isSceoll;
    Vector3 savePos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if(!isSceoll && Input.GetMouseButtonDown(1))
        {
            isSceoll = true;
            savePos = Input.mousePosition;
        }
        else if(isSceoll && Input.GetMouseButtonUp(1))
        {
            isSceoll = false;
        }
        else if(isSceoll && Input.GetMouseButton(1))
        {
            Vector3 difference = Input.mousePosition - savePos;

            if (difference.x > 0.0f)
            {
                transform.RotateAround(Vector3.zero, transform.up, difference.x * Time.deltaTime * 10);
                savePos = Input.mousePosition;
            }
            else if (difference.x < 0.0f)
            {
                transform.RotateAround(Vector3.zero, transform.up, difference.x * Time.deltaTime * 10);
                savePos = Input.mousePosition;
            }
            if (difference.y > 0.0f)
            {
                transform.RotateAround(Vector3.zero, transform.right, -difference.y * Time.deltaTime * 10);
                savePos = Input.mousePosition;
            }
            else if (difference.y < 0.0f)
            {
                transform.RotateAround(Vector3.zero, transform.right, -difference.y * Time.deltaTime * 10);
                savePos = Input.mousePosition;
            }
        }
        // カメラ移動
        else if(scroll != 0.0f)
        {
            Vector3 _pos = transform.position + transform.forward * scroll * ConstDefine.ConstParameter.VALUE_CAMERA;
            float dis = Vector3.Distance(_pos, Vector3.zero);
            if (dis > ConstDefine.ConstParameter.CAMERA_NEAR &&
                dis < ConstDefine.ConstParameter.CAMERA_FAR)
            {
                transform.position = _pos;
            }
        }
    }
}
