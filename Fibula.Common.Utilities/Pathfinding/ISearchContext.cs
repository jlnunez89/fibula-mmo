// -----------------------------------------------------------------
// <copyright file="ISearchContext.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Common.Utilities.Pathfinding
{
    /// <summary>
    /// Interface for a search context.
    /// </summary>
    public interface ISearchContext
    {
        /// <summary>
        /// Gets the id of the search.
        /// </summary>
        string SearchId { get; }
    }
}