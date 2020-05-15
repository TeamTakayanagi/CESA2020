using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : SingletonMonoBehaviour<EffectManager>
{
    [SerializeField]
    private GameObject m_spark = null;                                  // 火花プレハブ
    private List<GameObject> m_sparkObject = new List<GameObject>();    // 火花オブジェクトのリスト
    private Fuse m_nextFuseClass = null;                                // 燃え移る先の導火線クラス
    private BoxCollider m_hitCollider = null;                           // 進入方向の導火線コライダ

    public Fuse NextFuse
    {
        get {
            return m_nextFuseClass;
        }
        set {
            m_nextFuseClass = value;
        }
    }
    public BoxCollider HitCollider
    {
        get {
            return m_hitCollider;
        }
        set {
            m_hitCollider = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // スタート導火線検索
        Fuse[] m_allFuseClass = FindObjectsOfType<Fuse>();
        int _fuseClassCnt = 0; // ループ用
        while (true)
        {
            if (m_allFuseClass[_fuseClassCnt] == null)
                break;

            if (m_allFuseClass[_fuseClassCnt].Type == Fuse.FuseType.Start)
            {
                m_nextFuseClass = m_allFuseClass[_fuseClassCnt];
                break;
            }

            _fuseClassCnt++;
        }

        CreateSpark(m_nextFuseClass, Vector3.zero, Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 火花生成
    public void CreateSpark(Fuse _nextFuseClass, Vector3 _startPos, Vector3 _moveVector)
    {
        m_sparkObject.Add(Instantiate(m_spark));    // リストに追加

        int _sparkCount = m_sparkObject.Count;

        m_sparkObject[_sparkCount - 1].transform.parent = transform;
        Spark _sparkScript = m_sparkObject[_sparkCount - 1].GetComponent<Spark>();
        _sparkScript.NextFuseClass = _nextFuseClass;
        _sparkScript.StartPos = _startPos;
        _sparkScript.MoveVector = _moveVector;

        // 終了
        m_nextFuseClass = null;
    }

    // 火花削除
    public void RemoveSpark(GameObject _spark)
    {
        for (int _sparkCount = 0; _sparkCount < m_sparkObject.Count; _sparkCount++)
        {
            if (m_sparkObject[_sparkCount] == _spark)
            {
                m_sparkObject.RemoveAt(_sparkCount);
                Destroy(_spark);
            }
        }
    }

    // 火花の参照する導火線の変更
    public void ChangeFuseClass(Fuse _nowFuse, Fuse _nextFuse, BoxCollider _hitCollider)
    {
        float _fuseValue = Mathf.Infinity;
        int _sparkNum = 0;
        for (int _sparkCount = 0; _sparkCount < m_sparkObject.Count; _sparkCount++)
        {
            // 当たった導火線以外はやり直し
            if (_nowFuse != m_sparkObject[_sparkCount].GetComponent<Spark>().FuseClass)
                continue;

            // 次の導火線へ行く火花を検索
            Vector3 _fuseJudgePos = Vector3.zero;
            _fuseJudgePos = _nextFuse.transform.position - m_sparkObject[_sparkCount].transform.position;
            float _fuseJudgeValue = (_fuseJudgePos.x * _fuseJudgePos.x + _fuseJudgePos.z * _fuseJudgePos.z) *
                                    (_fuseJudgePos.x * _fuseJudgePos.x + _fuseJudgePos.z * _fuseJudgePos.z) +
                                    _fuseJudgePos.y * _fuseJudgePos.y;
            _fuseJudgeValue = Mathf.Sqrt(_fuseJudgeValue);
            if (_fuseValue > _fuseJudgeValue)
            {
                _fuseValue = _fuseJudgeValue;
                _sparkNum = _sparkCount;
            }
        }

        // 次の導火線へ移動させる
        Spark _sparkScript = m_sparkObject[_sparkNum].GetComponent<Spark>();
        _sparkScript.NextFuseClass = _nextFuse;
        _sparkScript.EnterCollider = _hitCollider;
    }

    // 火花が生成される方向のコライダーが誰かのenterColliderに入っているかどうか
    // 重いかも
    public void CheckEnterCollider(Fuse _fuseClass, List<BoxCollider> _otherEnterCol)
    {
        for (int _sparkCount = 0; _sparkCount < m_sparkObject.Count; _sparkCount++)
        {
            Spark _sparkSclipt = m_sparkObject[_sparkCount].GetComponent<Spark>();
            // 参照する導火線クラスが違うものは飛ばす
            if (_fuseClass != _sparkSclipt.FuseClass)
                continue;

            BoxCollider[] _fuseCollider = _fuseClass.gameObject.GetComponents<BoxCollider>();
            for (int _fuseColliderCnt = 0; _fuseColliderCnt < _fuseCollider.Length; _fuseColliderCnt++)
            {
                if (_fuseCollider[_fuseColliderCnt] == _sparkSclipt.EnterCollider)
                    _otherEnterCol.Add(_sparkSclipt.EnterCollider);
            }
        }
    }
}
