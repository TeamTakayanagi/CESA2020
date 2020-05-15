using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spark : MonoBehaviour
{
    // フェーズ
    private enum FASE {
        FASE_1,
        FASE_2,
        FASE_MAX,
    }

    private EffectManager m_effectMgrClass = null;                          // エフェクトマネージャ
    private Fuse m_fuseClass = null;                                        // 導火線キューブ取得用
    private Bounds m_fuseBounds = new Bounds(Vector3.zero, Vector3.zero);   // 導火線キューブのBounds取得用
    private Vector3 m_fuseExtents = Vector3.zero;                           // 導火線キューブのサイズ(1/2)
    private Fuse m_nextFuseClass = null;                                    // 次の導火線キューブ
    private Vector3 m_startPos = Vector3.zero;                              // 初期座標
    private Vector3 m_posOffset = new Vector3(0, 0, -0.2f);                 // 座標オフセット
    private float m_moveSpeed = 0.0f;                                       // 移動速度
    private Vector3 m_moveVector = Vector3.zero;                            // 移動方向
    private Vector3 m_move = Vector3.zero;                                  // 移動用
    private Vector3 m_lastPos = Vector3.zero;                               // 過去座標
    private BoxCollider m_enterCollider = null;                             // 進入方向の導火線コライダ
    private BoxCollider[] m_fuseCollider = null;                            // 導火線のコライダ
    private List<BoxCollider> m_othersEnterCol = new List<BoxCollider>();   // 自分の導火線の中の他人のenterCollider
    private bool m_checkEnterCol = false;                                   // enterCol検索をしたかどうかのフラグ

    public Fuse FuseClass
    {
        get {
            return m_fuseClass;
        }
    }
    public Fuse NextFuseClass
    {
        get {
            return m_nextFuseClass;
        }
        set {
            m_nextFuseClass = value;
        }
    }
    public BoxCollider EnterCollider
    {
        get {
            return m_enterCollider;
        }
        set {
            m_enterCollider = value;
        }
    }
    public Vector3 StartPos
    {
        get {
            return m_startPos;
        }
        set {
            m_startPos = value;
        }
    }
    public Vector3 MoveVector
    {
        get {
            return m_moveVector;
        }
        set {
            m_moveVector = value;
        }
    }

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        // マネージャ
        m_effectMgrClass = FindObjectOfType<EffectManager>();

        // 参照する導火線
        m_fuseClass = m_nextFuseClass;

        // コライダ取得
        m_fuseCollider = m_fuseClass.GetComponents<BoxCollider>();

        // サイズ取得
        m_fuseBounds = m_fuseClass.GetComponent<MeshFilter>().mesh.bounds;
        m_fuseExtents = new Vector3(m_fuseBounds.extents.x * m_fuseClass.transform.localScale.x,
                                    m_fuseBounds.extents.y * m_fuseClass.transform.localScale.y,
                                    m_fuseBounds.extents.z * m_fuseClass.transform.localScale.z);

        if (m_fuseClass.Type == Fuse.FuseType.Start)
        {
            // オフセット設定
            m_posOffset = new Vector3(m_fuseExtents.x * Mathf.Cos(Mathf.Deg2Rad * (m_fuseClass.transform.localEulerAngles.z + 270)),
                                      m_fuseExtents.x * Mathf.Sin(Mathf.Deg2Rad * (m_fuseClass.transform.localEulerAngles.z + 270)),
                                      0);
            // 初期座標
            gameObject.transform.position = m_fuseClass.transform.position + m_posOffset;

            m_moveVector = new Vector3(SignZero(m_fuseClass.transform.position.x - transform.position.x),
                                       SignZero(m_fuseClass.transform.position.y - transform.position.y),
                                       SignZero(m_fuseClass.transform.position.z - transform.position.z));

            // 仮
            m_enterCollider = m_fuseCollider[0];  

        }
        else
        {
            gameObject.transform.position = m_startPos;
            // m_moveVectorはCreate時に入れる
        }

        m_checkEnterCol = true;

        // 終了
        m_nextFuseClass = null;
    }

    // Update is called once per frame
    void Update()
    {
        Transform fuseTarget = m_fuseClass.ChildTarget;
        if (m_nextFuseClass != null)
        {// 次の導火線へ行く時
            m_fuseClass = m_nextFuseClass;
            
            m_fuseBounds = m_fuseClass.GetComponent<MeshFilter>().mesh.bounds;

            m_fuseCollider = m_fuseClass.GetComponents<BoxCollider>();

            m_checkEnterCol = true;

            m_nextFuseClass = null;
        }

        if (m_checkEnterCol && fuseTarget.localScale.x < 0.7f)
        {
            m_effectMgrClass.CheckEnterCollider(m_fuseClass, m_othersEnterCol);
            m_checkEnterCol = false;
        }

        // 燃え尽き削除
        if (fuseTarget.localScale.x <= 0.0f)
            m_effectMgrClass.RemoveSpark(gameObject);
            

        // 移動量計算
        m_moveSpeed = m_fuseBounds.size.z * (Time.deltaTime  / AdjustParameter.Fuse_Constant.BURN_MAX_TIME);
        m_move = m_moveSpeed * m_moveVector;

        // 移動
        if (m_fuseClass.State == Fuse.FuseState.Burn && fuseTarget.localScale.x != AdjustParameter.Fuse_Constant.BURN_MAX_TIME)
            transform.position += m_move;

        // 中心に来た時
        if (m_lastPos != Vector3.zero)
        {
            if ((Vector3.Dot(m_lastPos - m_fuseClass.transform.position, this.transform.position - m_fuseClass.transform.position) * 
                Vector3.Dot(m_move, m_moveVector)) < 0)
            {
                transform.position = m_fuseClass.transform.position;
                SparkBranch();
            }
        }

        // 過去座標格納
        m_lastPos = transform.position;
    }

    // 入れた値が＋なら１、０なら０、－なら－１を返す
    private float SignZero(float value)
    {
        if (value > 0)
            return 1.0f;
        else if (value == 0)
            return 0.0f;
        else if (value < 0)
            return -1.0f;
        else
            return 0.0f;
    }
    
    private void SparkBranch()
    {
        if (m_enterCollider.center == Vector3.zero)
        {// 進入方向が長いコライダの場合
            for (int _colliderNum = 0; _colliderNum < m_fuseCollider.Length; _colliderNum++)
            {
                // 進入方向のコライダは弾く
                if (m_enterCollider.size == m_fuseCollider[_colliderNum].size)
                    continue;

                // 判定するコライダが誰かのenterCollidrなら飛ばす
                bool _skip = false;
                for (int _listCount = 0; _listCount < m_othersEnterCol.Count; _listCount++)
                {

                    if (m_fuseCollider[_colliderNum] == m_othersEnterCol[_listCount])
                    {
                        _skip = true;
                        break;
                    }
                }
                if (_skip)
                    continue;

                if (m_fuseCollider[_colliderNum].center == new Vector3(0.0f, 0.0f, 0.0f))
                {// 長いコライダ
                    float _mostLong = Mathf.Max(m_fuseCollider[_colliderNum].bounds.size.x,
                                                m_fuseCollider[_colliderNum].bounds.size.y,
                                                m_fuseCollider[_colliderNum].bounds.size.z);
                    
                    if (_mostLong == m_fuseCollider[_colliderNum].bounds.size.x)
                    {
                        m_effectMgrClass.CreateSpark(m_fuseClass, transform.position, Vector3.right);
                        m_effectMgrClass.CreateSpark(m_fuseClass, transform.position, Vector3.left);
                    }
                    else if (_mostLong == m_fuseCollider[_colliderNum].bounds.size.y)
                    {
                        m_effectMgrClass.CreateSpark(m_fuseClass, transform.position, Vector3.up);
                        m_effectMgrClass.CreateSpark(m_fuseClass, transform.position, Vector3.down);
                    }
                    else if (_mostLong == m_fuseCollider[_colliderNum].bounds.size.z)
                    {
                        m_effectMgrClass.CreateSpark(m_fuseClass, transform.position, Vector3.forward);
                        m_effectMgrClass.CreateSpark(m_fuseClass, transform.position, Vector3.back);
                    }
                }
                else
                {// 短いコライダ
                    Vector3 _moveVector = m_fuseClass.transform.position - m_fuseCollider[_colliderNum].bounds.center;
                    float _judgeVector = Mathf.Max(Mathf.Abs(_moveVector.x), Mathf.Abs(_moveVector.y), Mathf.Abs(_moveVector.z));

                    if (_judgeVector == Mathf.Abs(_moveVector.x))
                    {
                        m_effectMgrClass.CreateSpark(m_fuseClass, transform.position, new Vector3(SignZero(-_moveVector.x), 0.0f, 0.0f));
                    }
                    else if (_judgeVector == Mathf.Abs(_moveVector.y))
                    {
                        m_effectMgrClass.CreateSpark(m_fuseClass, transform.position, new Vector3(0.0f, SignZero(-_moveVector.y), 0.0f));
                    }
                    else if (_judgeVector == Mathf.Abs(_moveVector.z))
                    {
                        m_effectMgrClass.CreateSpark(m_fuseClass, transform.position, new Vector3(0.0f, 0.0f, SignZero(-_moveVector.z)));
                    }
                }
            }
        }
        else
        {// 進入方向が短いコライダの場合

            for (int _colliderNum = 0; _colliderNum < m_fuseCollider.Length; _colliderNum++)
            {
                // 進入方向のコライダは弾く
                if (m_enterCollider.size == m_fuseCollider[_colliderNum].size)
                    continue;

                // 判定するコライダが誰かのenterCollidrなら飛ばす
                bool _skip = false;
                for (int _listCount = 0; _listCount < m_othersEnterCol.Count; _listCount++)
                {

                    if (m_fuseCollider[_colliderNum] == m_othersEnterCol[_listCount])
                    {
                        _skip = true;
                        break;
                    }
                }
                if (_skip)
                    continue;

                if (m_fuseCollider[_colliderNum].center == new Vector3(0.0f, 0.0f, 0.0f))
                {// 長いコライダ
                    float _mostLong = Mathf.Max(m_fuseCollider[_colliderNum].bounds.size.x,
                                                m_fuseCollider[_colliderNum].bounds.size.y,
                                                m_fuseCollider[_colliderNum].bounds.size.z);

                    if (_mostLong == m_fuseCollider[_colliderNum].bounds.size.x)
                    {
                        m_effectMgrClass.CreateSpark(m_fuseClass, transform.position, new Vector3(1.0f, 0.0f, 0.0f));
                        m_effectMgrClass.CreateSpark(m_fuseClass, transform.position, new Vector3(-1.0f, 0.0f, 0.0f));
                    }
                    else if (_mostLong == m_fuseCollider[_colliderNum].bounds.size.y)
                    {
                        m_effectMgrClass.CreateSpark(m_fuseClass, transform.position, new Vector3(0.0f, 1.0f, 0.0f));
                        m_effectMgrClass.CreateSpark(m_fuseClass, transform.position, new Vector3(0.0f, -1.0f, 0.0f));
                    }
                    else if (_mostLong == m_fuseCollider[_colliderNum].bounds.size.z)
                    {
                        m_effectMgrClass.CreateSpark(m_fuseClass, transform.position, new Vector3(0.0f, 0.0f, 1.0f));
                        m_effectMgrClass.CreateSpark(m_fuseClass, transform.position, new Vector3(0.0f, 0.0f, -1.0f));
                    }
                }
                else
                {// 短いコライダ
                    Vector3 _moveVector = m_fuseClass.transform.position - m_fuseCollider[_colliderNum].bounds.center;
                    float _judgeVector = Mathf.Max(Mathf.Abs(_moveVector.x), Mathf.Abs(_moveVector.y), Mathf.Abs(_moveVector.z));

                    if (_judgeVector == Mathf.Abs(_moveVector.x))
                    {
                        m_effectMgrClass.CreateSpark(m_fuseClass, transform.position, new Vector3(SignZero(-_moveVector.x), 0.0f, 0.0f));
                    }
                    else if (_judgeVector == Mathf.Abs(_moveVector.y))
                    {
                        m_effectMgrClass.CreateSpark(m_fuseClass, transform.position, new Vector3(0.0f, SignZero(-_moveVector.y), 0.0f));
                    }
                    else if (_judgeVector == Mathf.Abs(_moveVector.z))
                    {
                        m_effectMgrClass.CreateSpark(m_fuseClass, transform.position, new Vector3(0.0f, 0.0f, SignZero(-_moveVector.z)));
                    }
                }
            }

            // 分岐削除
            m_effectMgrClass.RemoveSpark(gameObject);
        }
    }
}
