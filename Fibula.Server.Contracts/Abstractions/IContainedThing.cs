// -----------------------------------------------------------------
// <copyright file="IContainedThing.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Contracts.Abstractions
{
    /// <summary>
    /// Interface for all entities that keep track of a parent container.
    /// </summary>
    public interface IContainedThing
    {
        /// <summary>
        /// Gets or sets the parent container of this entity.
        /// </summary>
        IThingContainer ParentContainer { get; set; }

        ///// <summary>
        ///// Gets this entity's container hierarchy.
        ///// </summary>
        ///// <param name="includeTiles">Optional. A value indicating whether to include tiles in the hierarchy. Defaults to true.</param>
        ///// <returns>The ordered collection of <see cref="IContainer"/>s in this thing's container hierarchy.</returns>
        //// TODO: move to an extension method instead.
        //IEnumerable<IContainer> GetCylinderHierarchy(bool includeTiles = true);
    }
}