using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SparkMove
{
    public class SparkMove : MonoBehaviour
    {
        public GameObject m_fuseBoxObject = null;                               // 導火線キューブのゲームオブジェクト
        private Bounds m_fuseBoxBounds = new Bounds(Vector3.zero, Vector3.zero);   // 導火線キューブのBounds

        private GameObject[] m_fuseObject = null;
        private Vector3 m_fuseExtents = Vector3.zero;


        private Vector3 m_posOffset = new Vector3(0, 0, -0.2f);                 // 座標オフセット
        private float m_moveSpeed = 0.002f;                                     // 移動速度
        private Vector3 m_move = Vector3.zero;


        // Start is called before the first frame update
        void Start()
        {
            m_fuseBoxBounds = m_fuseBoxObject.GetComponent<MeshFilter>().mesh.bounds;

            m_posOffset = new Vector3(-(m_fuseBoxBounds.extents.x * m_fuseBoxObject.transform.localScale.x), 0, -0.2f);

            // 初期座標
            this.gameObject.transform.position = m_fuseBoxObject.transform.position + m_posOffset;

            // 初期移動量
            m_move = new Vector3(m_moveSpeed, 0, 0);
        }

        // Update is called once per frame
        void Update()
        {
           

            // 移動
            this.gameObject.transform.position += m_move;

            
        }

        void OnTriggerEnter(Collider collider)
        {
            int _fuseCount = 0;
            while (true)
            {
                if (m_fuseObject[_fuseCount] != null)
                {
                    _fuseCount++;
                    continue;
                }

                m_fuseObject[_fuseCount] = collider.GetComponent<GameObject>();
                m_fuseExtents = collider.GetComponent<BoxCollider>().bounds.extents;
                if (m_fuseExtents.x > m_fuseExtents.y)
                {
                    if (m_fuseExtents.x > m_fuseExtents.z)
                    {
                        
                    }
                    else
                    {

                    }
                }
                else
                {
                    if (m_fuseExtents.y > m_fuseExtents.z)
                    {

                    }
                    else
                    {

                    }
                }
            }
        }
    }
}
