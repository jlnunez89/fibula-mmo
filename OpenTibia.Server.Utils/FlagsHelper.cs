namespace OpenTibia.Utilities
{
    public static class FlagsHelper
    {
        public static bool HasFlag(byte keysVal, byte flagVal)
        {
            return (keysVal & flagVal) == flagVal;
        }

        public static bool HasFlag(uint keysVal, uint flagVal)
        {
            return (keysVal & flagVal) == flagVal;
        }

        public static byte SetFlag(byte keysVal, byte flagVal)
        {
            return (byte)(keysVal | flagVal);
        }

        public static uint SetFlag(uint keysVal, uint flagVal)
        {
            return (keysVal | flagVal);
        }
    }
}
