
namespace ConstDefine
{
    public readonly struct ConstParameter
    {
        public const float BURN_MAX_TIME = 5.0f;                    // 燃えてる時間
        public const float SPREAD_TIME = BURN_MAX_TIME * 0.1f;      // 燃え移り始める時間
        public const float CUBE_SCALE = 1.0f;
        public const float CAMERA_NEAR = 3.0f;
        public const float CAMERA_FAR = 15.0f;
        public const float VALUE_CAMERA = 0.5f;
    }


    ///////////////////////////////////////////////////////////////////////////////

    public readonly struct TagName
    {
        public const string None = "None";
        public const string Untagged = "Untagged";
        public const string Player = "Player";
        public const string UICamera = "UICamera";
        public const string Fuse = "Fuse"; 
        public const string UICanvas = "UICanvas"; 
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
