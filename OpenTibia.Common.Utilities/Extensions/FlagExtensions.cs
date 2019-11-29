// -----------------------------------------------------------------
// <copyright file="FlagExtensions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Common.Utilities
{
    /// <summary>
    /// Helper class that provides methods for flag setting and checking.
    /// </summary>
    public static class FlagExtensions
    {
        /// <summary>
        /// Checks if the value has the <paramref name="flagVal"/> set.
        /// </summary>
        /// <param name="keysVal">The value to evaluate.</param>
        /// <param name="flagVal">The flag value to check for.</param>
        /// <returns>True if the value contains the flag, false otherwise.</returns>
        public static bool HasFlag(this byte keysVal, byte flagVal)
        {
            return (keysVal & flagVal) == flagVal;
        }

        /// <summary>
        /// Checks if the value has the <paramref name="flagVal"/> set.
        /// </summary>
        /// <param name="keysVal">The value to evaluate.</param>
        /// <param name="flagVal">The flag value to check for.</param>
        /// <returns>True if the value contains the flag, false otherwise.</returns>
        public static bool HasFlag(this uint keysVal, uint flagVal)
        {
            return (keysVal & flagVal) == flagVal;
        }

        /// <summary>
        /// Sets the flag value into the current variable.
        /// </summary>
        /// <param name="keysVal">The value to set the flag into.</param>
        /// <param name="flagVal">The flag value to add.</param>
        /// <returns>The variable after setting the flag.</returns>
        public static byte SetFlag(this byte keysVal, byte flagVal)
        {
            return (byte)(keysVal | flagVal);
        }

        /// <summary>
        /// Sets the flag value into the current variable.
        /// </summary>
        /// <param name="keysVal">The value to set the flag into.</param>
        /// <param name="flagVal">The flag value to add.</param>
        /// <returns>The variable after setting the flag.</returns>
        public static uint SetFlag(this uint keysVal, uint flagVal)
        {
            return keysVal | flagVal;
        }
    }
}
