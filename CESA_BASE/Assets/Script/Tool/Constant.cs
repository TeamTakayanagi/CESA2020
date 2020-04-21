
//---調整用変数---
// レベルデザインなどで後で数値変更を行うべき変数をここに記載
namespace AdjustParameter
{
    public readonly struct Camera_Constant
    {
        public const float CAMERA_NEAR = 3.0f;
        public const float CAMERA_FAR = 15.0f;
        public const float VALUE_CAMERA = 0.5f;
        public const float CAMERA_MOVE = 1.0f;
        public const float AROUND_MOVE = 10.0f;
        public const float PERMISSION_MOVE = 100.0f;
    }

    public readonly struct Fuse_Constant
    {
        public const float BURN_MAX_TIME = 15.0f;                       // 燃えてる時間
        public const float SPREAD_TIME = BURN_MAX_TIME * 0.1f;          // 燃え移り始める時間
        public const float FUSE_SCALE = 1.0f;
    }

    public readonly struct UI_Fuse_Constant
    {
        public const int UI_FUSE_MAX = 10;
        public const int CREATE_COOUNT = 60 * 5;
        public const float UI_FUSE_POS_Y = 11.0f;
        public const float UI_FUSE_POS_Z = 5.0f;
        public const float UI_FUSE_INTERVAL_X = 2.0f;
        public const float UI_FUSE_INTERVAL_Y = 2.0f;
    }

    public readonly struct Result_Constant
    {
        public const float DURATION = 1.0f;                                 // リザルトのUIのスライド時間（秒）
    }


}

//---加工済み変数---
// 他のオブジェクトでも共有する定数があれば（共有しないなら各クラス内で）
namespace ProcessedtParameter
{
    //public readonly struct Camera_Constant
    //{
    //}

    //public readonly struct Fuse_Constant
    //{
    //}
}

namespace StringDefine
{
    public readonly struct TagName
    {
        public const string None = "None";
        public const string Untagged = "Untagged";
        public const string Player = "Player";
        public const string UICamera = "UICamera";
        public const string Fuse = "Fuse"; 
        public const string UICanvas = "UICanvas";

        // 「/」の後のタグ
        public const string GameClear = "GameClear";
        public const string GameOver = "GameOver";
    }

    namespace Audio
    {
        public readonly struct BGM
        {
            public const string GameMain = "GameMain";
        } 
        public readonly struct SE
        {

        }
    }

    public readonly struct Scene
    {
        public const string Title = "Untagged";
        public const string StageSelect = "Untagged";
        public const string Game = "Player";
        public const string Clear = "GameClear";
    }
}
