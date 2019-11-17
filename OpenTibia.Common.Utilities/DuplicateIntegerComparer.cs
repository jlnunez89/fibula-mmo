// -----------------------------------------------------------------
// <copyright file="DuplicateIntegerComparer.cs" company="2Dudes">
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
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Class that represents a comparer that returns a deterministic value when two integers have the same value.
    /// </summary>
    /// <remarks>
    /// <see cref="SortedList"/> by default does not allow duplicate items because of the comparer it uses.
    /// </remarks>
    internal class DuplicateIntegerComparer : IComparer<int>
    {
        /// <summary>
        /// Compares an int to another.
        /// </summary>
        /// <param name="x">The first integer to compare.</param>
        /// <param name="y">The second integer to compare.c.</param>
        /// <returns>-1 if first is less than or equal to second, 1 otherwise.</returns>
        public int Compare(int x, int y)
        {
            return (x <= y) ? -1 : 1;
        }
    }
}
