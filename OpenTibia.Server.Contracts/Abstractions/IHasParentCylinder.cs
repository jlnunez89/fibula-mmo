// -----------------------------------------------------------------
// <copyright file="IHasParentCylinder.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Abstractions
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for all entities that keep track of a parent cylinder.
    /// </summary>
    public interface IHasParentCylinder
    {
        /// <summary>
        /// Gets or sets the parent cylinder of this entity.
        /// </summary>
        ICylinder ParentCylinder { get; set; }

        /// <summary>
        /// Gets this entity's cylinder hierarchy.
        /// </summary>
        /// <returns>The ordered collection of <see cref="ICylinder"/>s in this entity's cylinder hierarchy.</returns>
        IEnumerable<ICylinder> GetCylinderHierarchy();
    }
}