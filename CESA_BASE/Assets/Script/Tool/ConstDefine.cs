
namespace ConstDefine
{
    public readonly struct ConstParameter
    {
        public const float BURN_SPEED = 2.0f;
        public const float CUBE_SCALE = 1.0f;
        public const float CAMERA_NEAR = 3.0f;
        public const float CAMERA_FAR = 15.0f;
        public const float VALUE_CAMERA = 0.5f;
    }


    ///////////////////////////////////////////////////////////////////////////////

    public readonly struct TagName
    {
        public const string None = "Untagged";
        public const string Untagged = "Untagged";
        public const string Player = "Player";
        public const string UICamera = "UICamera";

        public const string SceneMgr = "SceneMgr";
        public const string Title = "Title";
        public const string UIStage = "UI/Stage";
        
        // ここからparent
        public const string Fuse = "Fuse"; 
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
