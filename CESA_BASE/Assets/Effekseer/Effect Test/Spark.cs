using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spark
{
    public class Spark : MonoBehaviour
    {
        // フェーズ
        private enum FASE {
            FASE_1,
            FASE_2,
            FASE_MAX,
        }

        public GameObject m_fuseObject = null;                                  // ゲームオブジェクト取得用
        private Bounds m_fuseBounds = new Bounds(Vector3.zero, Vector3.zero);   // 導火線キューブのBounds取得用
        private Vector3 m_fuseExtents = Vector3.zero;                           // 導火線キューブのサイズ(1/2)
        private Vector3 m_posOffset = new Vector3(0, 0, -0.2f);                 // 座標オフセット
        private float m_moveSpeed = 0.002f;                                     // 移動速度
        private Vector3 m_move = Vector3.zero;
        private FASE m_fase = FASE.FASE_1;
        private bool m_faseChange = false;

        float kakudo = 0.0f;

        // Start is called before the first frame update
        void Start()
        {
            // サイズ取得
            m_fuseBounds = m_fuseObject.GetComponent<MeshFilter>().mesh.bounds;
            m_fuseExtents = new Vector3(m_fuseBounds.extents.x * m_fuseObject.transform.localScale.x,
                                        m_fuseBounds.extents.y * m_fuseObject.transform.localScale.y,
                                        m_fuseBounds.extents.z * m_fuseObject.transform.localScale.z);


            // オフセット設定
            m_posOffset = new Vector3(m_fuseExtents.x * Mathf.Cos(Mathf.Deg2Rad * (m_fuseObject.transform.localRotation.z + 180)),
                                      m_fuseExtents.x * Mathf.Sin(Mathf.Deg2Rad * (m_fuseObject.transform.localRotation.z + 180)),
                                      -0.2f);
            
            // 初期座標
            this.gameObject.transform.position = m_fuseObject.transform.position + m_posOffset;

            // 初期移動量
            m_move = new Vector3(0, 0, 0);
        }

        // Update is called once per frame
        void Update()
        {
            // フェーズチェック
            //if (this.gameObject.transform.position.x >= m_fuseObject.transform.position.x && m_fase == FASE.FASE_1)
            //{
            //    m_posOffset = new Vector3(0, 0, -0.2f);
            //    this.gameObject.transform.position = m_fuseObject.transform.position + m_posOffset;
            //    m_fase = FASE.FASE_2;
            //    m_move = new Vector3(0, m_moveSpeed, 0);
            //    m_faseChange = true;
            //}

            // フェーズ変更
            //if (m_faseChange)
            //{
            //    switch (m_fase)
            //    {
            //        case FASE.FASE_1:
            //            //m_move = new Vector3(m_moveSpeed, 0, 0);
            //            break;

            //        case FASE.FASE_2:
            //            m_move = new Vector3(0, m_moveSpeed, 0);
            //            break;

            //        default:
            //            break;
            //    }

            //    m_faseChange = false;
            //}

            // 移動
            //this.gameObject.transform.position += m_move;

            

            m_posOffset = new Vector3(m_fuseExtents.x * Mathf.Cos(Mathf.Deg2Rad * (kakudo + 180)),
                                      m_fuseExtents.x * Mathf.Sin(Mathf.Deg2Rad * (kakudo + 180)),
                                      -0.2f);

            this.gameObject.transform.position = m_fuseObject.transform.position + m_posOffset;

            kakudo++;
            if (kakudo >= 360)
                kakudo = 0.0f;
        }
    }
}
