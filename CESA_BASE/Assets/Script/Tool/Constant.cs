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
        public const float SWIPE_MOVE = 1.0f;
        public const float SWIPE_DERAY = 0.5f;
        public const float AROUND_MOVE = 5.0f;
        public const float AROUND_ANGLE = 15.0f;
        public const float PERMISSION_MOVE = 20.0f;
        public const float ZOOM_SPEED = 1.0f;
        public const float FADE_DURATION = 2.0f;
        public const float EFFECT_POS_Z = 2.0f;
    }

    public readonly struct Fuse_Constant
    {
        public const float BURN_MAX_TIME = 50.0f;                        // 燃えてる時間(秒)
        public const float OUT_MAX_TIME = 5.0f;                         // 燃え尽きた後消える時間(秒)
        public const float DEFAULT_SCALE = 1.0f;
        public const float WET_MAX_TIME = 5.0f;
        public const float MOVE_VALUE = 2.0f;
        public const float ROT_VALUE = 5.0f;
    }

    public readonly struct UI_Object_Constant
    {
        public const int UI_AMOUNT_MAX = 10;
        public const int CREATE_COOUNT = 5;
        public const float DEFAULT_POS_Y = 11.0f;
        public const float DEFAULT_POS_Z = 5.0f;
        public const float INTERVAL_X = 2.0f;
        public const float INTERVAL_Y = 2.0f;
        public const float MOVE_VALUE_Y = 0.2f;
    }

    public readonly struct Production_Constant
    {
        public const float START_TIME = 30.0f;
        public const float WAIT_TIME = 0.2f;                                // 目標地点に到着してから花火が出るまでの待ち時間（秒）
        public const float DURATION = 1.0f;                                 // リザルトのUIのスライド時間（秒）
        public const float END_FIRE_POS_Y = 30.0f;                          // 花火の終着地点の座標   
        public const float LAUNCH_TIME = 5.0f;                              // 花火の打ち上げ時間   
    }

    public readonly struct Fade_Constant
    {
        public const float FADE_DURATION = 3.0f;                            // フェードのスライド時間
    }
}

//---加工済み変数---
// 他のオブジェクトでも共有する定数があれば（共有しないなら各クラス内で）
namespace ProcessedtParameter
{
    public readonly struct Game_Scene
    {
        public const string STAGE_SELECT = "StageSelectLatte";
        public const string GAME_MAIN = "SampleSceneLatte";
    }

    public readonly struct Camera_Constant
    {
        public const float RECT_WIDTH = 0.8f;                          // メインカメラの映す幅
    }
    public readonly struct GameObject_Constant
    {
        public const int FUSE_TYPE = 7;                               // 導火線の種類
    }
    public readonly struct System_Constant
    {
        public const int ERROR_INT = -999;                                 // int型エラー判別変数
    }
    public readonly struct CSV_Constant
    {
        public const string STAGE_DATA_PATH = "StageData";
        public const int TYPE_WORD_COUNT = 1;                                    // オブジェクトの追加情報の文字数
        public const int ADDINFO_WORD_COUNT = 1;                                    // オブジェクトの追加情報の文字数
        public const int OBJECT_WORD_COUNT = 2;                                     // オブジェクトの種類の情報の文字数
        public const int OBJECT_ROT_COUNT = 3;                                      // オブジェクトの角度情報の文字数
        public const int STAGE_DATA_COUNT = TYPE_WORD_COUNT + ADDINFO_WORD_COUNT + OBJECT_WORD_COUNT + OBJECT_ROT_COUNT;   // ステージデータの文字数
    }

    public readonly struct ClickObj
    {
        public struct Tree
        {
            public const float SWAYS_SPEED = 20;
            public const float SWAYS_ANGLE = 3;
            public const float MAX_REDIAN = 9;
            public const float ANIME_DURATION = 60;
        }
        public struct Grass
        {
            public const float SWAYS_SPEED = 20;
            public const float SWAYS_POS = 0.01f;
            public const float MAX_REDIAN = 12;
            public const float ANIME_DURATION = 60;
        }
    }

    public readonly struct LaunchTiming
    {
        public const float INIT = 3;
        public const float NEXT = 7;
    }
}

namespace NameDefine
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
        public const string Gimmick = "Gimmick";
        public const string Stage = "Stage";
        public const string ClickObj = "ClickObject";
        public const string StageParent = "StageParent";

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
