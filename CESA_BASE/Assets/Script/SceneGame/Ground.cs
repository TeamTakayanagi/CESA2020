using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    [SerializeField]
    GameObject m_blockPrefab = null;
    [SerializeField]
    int width = 0;
    [SerializeField]
    int height = 0;

    // Start is called before the first frame update
    void Start()
    {
        float underPosY = GameMgr.Instance.StageSizeMin.y - 1;
        Vector2 half = new Vector2(Mathf.Ceil(width / 2), Mathf.Ceil(height / 2));
        for (int i = 0; i < height; ++i)
        {
            for (int j = 0; j < width; ++j)
            {
                GameObject obj = Instantiate(m_blockPrefab, new Vector3(j - half.x, underPosY, i - half.y), Quaternion.identity);
                obj.transform.parent = transform;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
