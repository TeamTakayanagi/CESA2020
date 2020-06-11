//---調整用変数---
// レベルデザインなどで後で数値変更を行うべき変数をここに記載
using System.Numerics;

namespace AdjustParameter
{
    public readonly struct Camera_Constant
    {
        public const float CAMERA_NEAR = 5.0f;                          // カメラのNear
        public const float CAMERA_FAR = 11.0f;                          // カメラのFar
        public const float VALUE_SCROLL = 0.5f;                         // NearとFarの間を変化させるときの値
        public const float ROT_Y_VALUE = 7.0f;                          // Y軸固定で見渡すときの回転速度
        public const float SWIPE_MOVE = 1.0f;                           // スワイプの移動速度
        public const float SWIPE_DERAY = 1.0f;                          // スワイプ移動を何秒で行うか
        public const float SWIPE_OUT = -0.3f;                            // スワイプ移動ではじき出されるときの補正値
        public const float AROUND_MOVE = 5.0f;                          // 全方向見渡すときの速さ
        public const float AROUND_ANGLE = 45.0f;                        // 全方向見渡すときの初期角度
        public const float PERMISSION_MOVE = 20.0f;                     // 全方向移動の際、どれほどまでの差を無視するか
        public const float ZOOM_SPEED = 1.0f;                           // ズーム速度
        public const float FADE_DURATION = 2.0f;                        // フェードのズームインでどれほどの時間をかけて進むか
        public const float EFFECT_POS_Z = 2.0f;                         // エフェクトをカメラからどのくらいの距離で出すか
       // public const float ZOOM_HEIGHT = 

    }

    public readonly struct Fuse_Constant
    {
        public const float BURN_MAX_TIME = 7.5f;                        // 燃えてる時間(秒)
        public const float OUT_MAX_TIME = 5.0f;                         // 燃え尽きた後消える時間(秒)
        public const float WET_MAX_TIME = 5.0f;                         // 濡れた状態がいつまで続くか
        public const float MOVE_VALUE = 2.0f;                           // 導火線の移動速度（ギミック）
        public const float ROT_VALUE = 5.0f;                            // 導火線の回転速度（ギミック）
    }

    public readonly struct Arrow_Constant
    {
        public const float SWAYS_SPEED = 20;
        public const float SWAYS_POS = 0.05f;
        public const float MAX_REDIAN = 12;
    }

    public readonly struct UI_Object_Constant
    {
        public const int UI_AMOUNT_MAX = 10;                            // UIの導火線の最大数
        public const int CREATE_COOUNT = 5;                             // UI何秒ごとに生成するか
    }

    public readonly struct Production_Constant
    {
        public const float START_FUSE_MOVE = 30.0f;                     // スタート演出の導火線の移動速度
        public const float WAIT_TIME = 0.2f;                            // 目標地点に到着してから花火が出るまでの待ち時間（秒）
        public const float DURATION = 1.0f;                             // リザルトのUIのスライド時間（秒）
        public const float END_FIRE_POS_Y = 30.0f;                      // 花火の終着地点の座標   
        public const float LAUNCH_TIME = 5.0f;                          // 花火の打ち上げ時間   
        public const float RESULT_TIME = 2.0f;                          // 花火の打ち上げ時間   
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
    public readonly struct UI_Object_Constant
    {
        public const float DEFAULT_POS_Y = 11.0f;
        public const float DEFAULT_POS_Z = 5.0f;
        public const float INTERVAL_X = 2.0f;
        public const float INTERVAL_Y = 2.0f;
        public const float MOVE_VALUE_Y = 0.2f;
    }

    public readonly struct Camera_Constant
    {
        public const float RECT_WIDTH = 0.8f;                          // メインカメラの映す幅
        public const float FIRST_ROT_X = 45f;                          // メインカメラの映す幅
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
        public const float GAME = 5;
    }
}

namespace NameDefine
{
    public readonly struct Scene_Name
    {
        public const string STAGE_SELECT = "StageSelect";
        public const string GAME_MAIN = "GameScene";
    }

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
        public const string UIGameButton = "UI/GameButton";
        public const string TerrainBlock = "TerrainBlock";
        public const string Gimmick = "Gimmick";
        public const string Stage = "Stage";
        public const string ClickObj = "ClickObject";
        public const string StageParent = "StageParent";
        public const string FuseParent = "fuseParent";

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

    public readonly struct Layer
    {
        public const int Default = 0;
        public const int Trans = 1;
        public const int Ignore = 2;
        public const int PostEffect = 8;
    }
}
