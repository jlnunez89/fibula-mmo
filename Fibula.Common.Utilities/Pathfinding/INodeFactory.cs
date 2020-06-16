// -----------------------------------------------------------------
// <copyright file="INodeFactory.cs" company="2Dudes">
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
    /// Interface for node factories.
    /// </summary>
    public interface INodeFactory
    {
        /// <summary>
        /// Creates a node with the given <paramref name="searchContext"/>, using the given <paramref name="nodeCreationData"/>.
        /// </summary>
        /// <param name="searchContext">A reference to the context of the search this node takes place in.</param>
        /// <param name="nodeCreationData">The node creation data.</param>
        /// <returns>An instance of a <see cref="INode"/>.</returns>
        INode Create(ISearchContext searchContext, INodeCreationArguments nodeCreationData);

        /// <summary>
        /// Method called when a search is completed, whatever the result is.
        /// </summary>
        /// <param name="searchId">The search id.</param>
        void OnSearchCompleted(string searchId);
    }
}
