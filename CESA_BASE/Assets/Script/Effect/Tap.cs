using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class Tap : MonoBehaviour
{
    [SerializeField]
    private Vector2 m_uvDiv = new Vector2(0, 0);    // テクスチャのUV分割数
    private Rect m_uvRect = new Rect();              // Rectクラス
    private RawImage m_rawImage = null; // RawImageコンポーネント
    private Vector2 m_uvPos = Vector2.zero;         // 座標更新用変数

    private void Awake()
    {
        // コンポーネント取得
        m_rawImage = GetComponent<RawImage>();
        // クラス取得
        m_uvRect = m_rawImage.uvRect;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_uvPos.x++;
        // UV座標のX値が上限に達したら
        if (m_uvPos.x >= m_uvDiv.x)
        {
            m_uvPos.x = 0.0f;
            m_uvPos.y++;
            // UV座標のY値が上限に達したら
            if (m_uvPos.y >= m_uvDiv.y)
            {
                Destroy(gameObject);
            }
        }

        // 座標更新
        m_uvRect.width = 1 / m_uvDiv.x;
        m_uvRect.height = 1 / m_uvDiv.y;
        m_uvRect.x = m_uvPos.x / m_uvDiv.x;
        m_uvRect.y = 1 - m_uvPos.y / m_uvDiv.y; 

        // 値を代入
        m_rawImage.uvRect = m_uvRect;
    }
}
