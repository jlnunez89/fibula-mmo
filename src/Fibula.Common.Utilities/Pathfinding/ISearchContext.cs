// -----------------------------------------------------------------
// <copyright file="ISearchContext.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common.Utilities.Pathfinding
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
