using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayPoint : MonoBehaviour
{
    public enum PointPlace
    {
        // カメラの子供の順番に
        right,
        up,
        left,
        bottm
    }

    [SerializeField]
    private PointPlace m_placeType = PointPlace.bottm;

    // Start is called before the first frame update
    void Start()
    {
        float distance = Vector3.Distance(Camera.main.transform.position, Vector3.zero);
        transform.position = Camera.main.transform.position;
        switch (m_placeType)
        {
            case PointPlace.right:
                transform.position += new Vector3(distance, 0.0f, distance);
                break;
            case PointPlace.up:
                transform.position += new Vector3(0.0f, distance, distance);
                break;
            case PointPlace.left:
                transform.position += new Vector3(-distance, 0.0f, distance);
                break;
            case PointPlace.bottm:
                transform.position += new Vector3(0.0f, -distance, distance);
                break;
        }
    }
}
