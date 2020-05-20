using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Effekseer;

public class Spark : EffekseerEmitter
{
    private int m_effectNum = 0;                                            // 導火線から見てこのエフェクトが何個目か
    private Vector3 m_moveVector = Vector3.zero;                            // 移動方向
    private Fuse m_fuseClass = null;                                        // 導火線キューブ取得用
    private BoxCollider m_enterCollider = null;                             // 進入方向の導火線コライダ
    private List<BoxCollider> m_fuseCollider = new List<BoxCollider>();     // 進入方向以外導火線のコライダ

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        // コライダ取得
        m_fuseCollider.AddRange(m_fuseClass.GetComponents<BoxCollider>());

        // その導火線の進行方向のコライダーを取得
        for (int i = 0; i < m_fuseCollider.Count; ++i)
        {
            if (Vector3.Dot(m_moveVector, m_fuseCollider[i].size) < 1.0f)
                continue;

            m_enterCollider = m_fuseCollider[i];
            m_fuseCollider.Remove(m_enterCollider);
            break;
        }
    }

    // Update is called once per frame
    new void Update()
    {
        // エフェクシアの更新処理
        base.Update();

        if (m_fuseClass == null)
            return;

        Transform fuseTarget = m_fuseClass.ChildTarget;

        // 導火線が燃え立つ来たのを確認して自信を即削除
        if (m_fuseClass.State == Fuse.FuseState.Out && fuseTarget.localScale.x >= 1.0f)
        {
            DestroyImmediate(gameObject);
            return;
        }

        // 移動量計算
        Vector3 move = m_moveVector * Time.deltaTime / AdjustParameter.Fuse_Constant.BURN_MAX_TIME;
        Vector3 afterPos = transform.position;

        // 移動
        if (m_fuseClass.State == Fuse.FuseState.Burn && fuseTarget.localScale.x != AdjustParameter.Fuse_Constant.BURN_MAX_TIME)
            afterPos += move;

        // 中心に来た時
        if (m_effectNum == 0 && afterPos != Vector3.zero &&
            (Vector3.Dot(afterPos - m_fuseClass.transform.position, transform.position - m_fuseClass.transform.position) *
                Vector3.Dot(move, m_moveVector)) < 0)
        {
            // 導火線の
            SparkBranch(m_fuseClass.HaveEffect(this));
        }

        transform.position = afterPos;
    }

    private void SparkBranch(List<BoxCollider> coliderList)
    {
        for (int i = 0; i < m_fuseCollider.Count; i++)
        {
            bool _skip = false;
            for (int j = 0; j < coliderList.Count; j++)
            {
                if (Vector3.Cross(m_fuseCollider[i].size, coliderList[j].size) != Vector3.zero)
                    continue;

                _skip = true;
            }
            if (_skip)
                continue;

            CreateBranchEffect(m_fuseCollider[i]);

            // 進入方向が短いコライダの場合
            if (m_enterCollider.center != Vector3.zero)
                DestroyImmediate(gameObject);           // 進行中のエフェクトを削除
        }
    }

    private void CreateBranchEffect(BoxCollider _collider)
    {
        if (_collider.center == Vector3.zero)
        {// 長いコライダ
            float _mostLong = Mathf.Max(_collider.bounds.size.x,
                                        _collider.bounds.size.y,
                                        _collider.bounds.size.z);

            if (_mostLong == _collider.bounds.size.x)
            {
                Instantiate(transform.position, Vector3.right, m_fuseClass, -1);
                Instantiate(transform.position, Vector3.left, m_fuseClass, -1);
            }
            else if (_mostLong == _collider.bounds.size.y)
            {
                Instantiate(transform.position, Vector3.up, m_fuseClass, -1);
                Instantiate(transform.position, Vector3.down, m_fuseClass, -1);
            }
            else if (_mostLong == _collider.bounds.size.z)
            {
                Instantiate(transform.position, Vector3.forward, m_fuseClass, -1);
                Instantiate(transform.position, Vector3.back, m_fuseClass, -1);
            }
        }
        else
        {// 短いコライダ
            Vector3 _moveVector = m_fuseClass.transform.position - _collider.bounds.center;
            float _judgeVector = Mathf.Max(Mathf.Abs(_moveVector.x), Mathf.Abs(_moveVector.y), Mathf.Abs(_moveVector.z));

            if (_judgeVector == Mathf.Abs(_moveVector.x))
            {
                Instantiate(transform.position, new Vector3(-_moveVector.x, 0.0f, 0.0f), m_fuseClass, -1);
            }
            else if (_judgeVector == Mathf.Abs(_moveVector.y))
            {
                Instantiate(transform.position, new Vector3(0.0f, -_moveVector.y, 0.0f), m_fuseClass, -1);
            }
            else if (_judgeVector == Mathf.Abs(_moveVector.z))
            {
                Instantiate(transform.position, new Vector3(0.0f, 0.0f, -_moveVector.z), m_fuseClass, -1);
            }
        }

    }

    /// <summary>
    /// 火花のエフェクト作成
    /// </summary>
    /// <param name="pos">座標</param>
    /// <param name="move">移動量</param>
    /// <param name="fuse">エフェクトのある導火線</param>
    /// <param name="haveEffect">導火線の何個目のエフェクトか（追加生成はー１）</param>
    /// <returns></returns>
    static public Spark Instantiate(Vector3 pos, Vector3 move, Fuse fuse, int haveEffect)
    {
        Spark spark = EffectManager.Instance.EffectCreate(EffectType.Spark, pos, Quaternion.identity).GetComponent<Spark>();
        spark.m_moveVector = move;
        spark.m_fuseClass = fuse;
        spark.m_effectNum = haveEffect;

        return spark;
    }
}
