// -----------------------------------------------------------------
// <copyright file="SortedListExtensions.cs" company="2Dudes">
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
    using System.Collections.Generic;

    /// <summary>
    /// Extension methods to make the System.Collections.Generic.SortedList easier to use.
    /// </summary>
    public static class SortedListExtensions
    {
        /// <summary>
        /// Checks if the SortedList is empty.
        /// </summary>
        /// <typeparam name="TKey">The type of key.</typeparam>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="sortedList">SortedList to check if it is empty.</param>
        /// <returns>True if sortedList is empty, false if it still has elements.</returns>
        public static bool IsEmpty<TKey, TValue>(this SortedList<TKey, TValue> sortedList)
        {
            return sortedList.Count == 0;
        }

        /// <summary>
        /// Adds a INode to the SortedList.
        /// </summary>
        /// <param name="sortedList">SortedList to add the node to.</param>
        /// <param name="node">Node to add to the sortedList.</param>
        public static void Add(this SortedList<int, INode> sortedList, INode node)
        {
            sortedList.Add(node.TotalCost, node);
        }

        /// <summary>
        /// Removes the node from the sorted list with the smallest TotalCost and returns that node.
        /// </summary>
        /// <param name="sortedList">SortedList to remove and return the smallest TotalCost node.</param>
        /// <returns>Node with the smallest TotalCost.</returns>
        public static INode Pop(this SortedList<int, INode> sortedList)
        {
            var top = sortedList.Values[0];

            sortedList.RemoveAt(0);

            return top;
        }
    }
}
