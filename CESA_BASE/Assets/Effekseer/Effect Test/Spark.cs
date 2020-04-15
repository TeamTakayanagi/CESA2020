using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spark
{
    public class Spark : MonoBehaviour
    {
        public GameObject m_fuseObject = null;                                  // ゲームオブジェクト取得用
        private Bounds m_fuseBouns = new Bounds(Vector3.zero, Vector3.zero);    // モデルのサイズ取得用
        private Vector3 m_posOffset = new Vector3(0, 0, -0.2f);                 // 座標オフセット
        private Vector3 m_moveSpeed = new Vector3(0.009f, 0, 0);                // 移動速度

        // Start is called before the first frame update
        void Start()
        {
            // サイズ取得
            m_fuseBouns = m_fuseObject.GetComponent<MeshFilter>().mesh.bounds;

            // オフセット設定
            m_posOffset += new Vector3(-(m_fuseBouns.extents.z * m_fuseObject.transform.localScale.z), 0, 0);
        }

        // Update is called once per frame
        void Update()
        {
            // 座標代入
            this.gameObject.transform.position = m_fuseObject.transform.position + m_posOffset;

            // 移動
            m_posOffset += m_moveSpeed;
        }
    }
}
