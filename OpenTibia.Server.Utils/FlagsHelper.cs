// <copyright file="FlagsHelper.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

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
            return keysVal | flagVal;
        }
    }
}
