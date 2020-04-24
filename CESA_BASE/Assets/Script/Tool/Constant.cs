
//---調整用変数---
// レベルデザインなどで後で数値変更を行うべき変数をここに記載
namespace AdjustParameter
{
    public readonly struct Camera_Constant
    {
        public const float CAMERA_NEAR = 3.0f;
        public const float CAMERA_FAR = 15.0f;
        public const float VALUE_SCROLL = 0.5f;
        public const float ROTY_VALUE = 0.5f;
        public const float AROUND_MOVE = 5.0f;
        public const float PERMISSION_MOVE = 20.0f;
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
    public readonly struct System_Constant
    {
        public const int ERROR_INT = -999;                                 // int型エラー判別変数
        public const int CSV_WORD_LENGHT = 5;                               // CSVに書き込む文字の長さ
    }

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
        public const string SubCamera = "SubCamera";
        public const string Fuse = "Fuse"; 
        public const string UICanvas = "UICanvas";
        public const string UIGameClear = "UI/GameClear";
        public const string UIGameOver = "UI/GameOver";
        public const string TerrainBlock = "TerrainBlock";

        public const string FuseI = "Fuse/I";
        public const string FuseL = "Fuse/L";
        public const string FuseT = "Fuse/T";
        public const string FuseX = "Fuse/X";
        public const string FuseLL = "Fuse/LL";
        public const string FuseTT = "Fuse/TT";
        public const string FuseXX = "Fuse/XX";
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
    public readonly struct Layer
    {
        public const int Default = 0;
        public const int Trans = 1;
        public const int Ignore = 2;
        public const int PostEffect = 8;
    }
}
