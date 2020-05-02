namespace Lolighter.Items
{
    static class Enum
    {
        public static class Line
        {
            public const int Left = 0;
            public const int MiddleLeft = 1;
            public const int MiddleRight = 2;
            public const int Right = 3;
        }

        public static class Layer
        {
            public const int Bottom = 0;
            public const int Middle = 1;
            public const int Top = 2;
        }

        public static class CutDirection
        {
            public const int Up = 0;
            public const int Down = 1;
            public const int Left = 2;
            public const int Right = 3;
            public const int UpLeft = 4;
            public const int UpRight = 5;
            public const int DownLeft = 6;
            public const int DownRight = 7;
            public const int Any = 8;
        }

        public static class NoteType
        {
            public const int Red = 0;
            public const int Blue = 1;
            public const int Mine = 3;
        }

        public static class ObstacleType
        {
            public const int Wall = 0;
            public const int Ceiling = 1;
        }

        public static class EventType
        {
            public const int LightBackTopLasers = 0;
            public const int LightTrackRingNeons = 1;
            public const int LightLeftLasers = 2;
            public const int LightRightLasers = 3;
            public const int LightBottomBackSideLasers = 4;
            public const int RotationAllTrackRings = 8;
            public const int RotationSmallTrackRings = 9;
            public const int RotatingLeftLasers = 12;
            public const int RotatingRightLasers = 13;
        }

        public static class EventLightValue
        {
            public const int Off = 0;
            public const int BlueOn = 1;
            public const int BlueFlashStay = 2;
            public const int BlueFlashFade = 3;
            public const int RedOn = 5;
            public const int RedFlashStay = 6;
            public const int RedFlashFade = 7;
        }
    }
}
