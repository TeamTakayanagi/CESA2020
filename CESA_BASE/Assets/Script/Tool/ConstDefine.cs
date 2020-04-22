
namespace ConstDefine
{
    public readonly struct ConstParameter
    {
        public const float BURN_MAX_TIME = 15.0f;                    // 燃えてる時間
        public const float SPREAD_TIME = BURN_MAX_TIME * 0.1f;      // 燃え移り始める時間
        public const float CUBE_SCALE = 1.0f;
        public const float CAMERA_NEAR = 3.0f;
        public const float CAMERA_FAR = 15.0f;
        public const float VALUE_CAMERA = 0.5f;
        public const float CAMERA_MOVE = 1.0f;
        public const float AROUND_MOVE = 10.0f;
        public const float PERMISSION_MOVE = 100.0f;

    }


    ///////////////////////////////////////////////////////////////////////////////

    public readonly struct TagName
    {
        public const string None = "None";
        public const string Untagged = "Untagged";
        public const string Player = "Player";
        public const string UICamera = "UICamera";

        public const string SceneMgr = "SceneMgr";
        public const string Title = "Title";
        public const string UIStage = "UI/Stage";
        
        // ここからparent
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

    public readonly struct Fuse
    {
        //public const string[] FuseTag = { "Fuse/I", "Fuse/L", "Fuse/T", "Fuse/X", "Fuse/LL", "Fuse/TT", "Fuse/All" };

        public const string FuseI = "Fuse/I";
        public const string FuseL = "Fuse/L";
        public const string FuseT = "Fuse/T";
        public const string FuseX = "Fuse/X";
        public const string FuseLL = "Fuse/LL";
        public const string FuseTT = "Fuse/TT";
        public const string FuseXX = "Fuse/XX";
    }
}
