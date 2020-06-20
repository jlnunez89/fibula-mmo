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
    }
}