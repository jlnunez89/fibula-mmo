// -----------------------------------------------------------------
// <copyright file="FlagExtensions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
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
    }
}
