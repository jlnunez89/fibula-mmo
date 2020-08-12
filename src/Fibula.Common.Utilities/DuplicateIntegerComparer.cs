// -----------------------------------------------------------------
// <copyright file="DuplicateIntegerComparer.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common.Utilities
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
            return x <= y ? -1 : 1;
        }
    }
}
