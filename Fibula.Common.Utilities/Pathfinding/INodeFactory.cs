// -----------------------------------------------------------------
// <copyright file="INodeFactory.cs" company="2Dudes">
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
