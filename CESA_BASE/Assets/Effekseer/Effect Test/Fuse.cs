//////////
// 導火線
//////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fuse
{
    public class Fuse : MonoBehaviour
    {
        // 変数
        private Material material;           // マテリアル取得用
        private float m_border = 0.0f;    // 境界値

        // Start is called before the first frame update
        void Start()
        {
            this.material = GetComponent<Renderer>().material;  // マテリアル取得
        }

        // Update is called once per frame
        void Update()
        {
            // 境界値を徐々に上げていく
            if (m_border < 1.0f)
                m_border += 1.0f / 900.0f;
            else
                m_border = 1.0f;

            this.material.SetFloat("_Border", m_border);    // シェーダーに値を渡す
        }
    }
}

