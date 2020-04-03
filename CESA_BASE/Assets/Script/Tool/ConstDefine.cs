
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
}
