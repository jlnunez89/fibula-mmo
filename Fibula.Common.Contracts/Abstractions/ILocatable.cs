// -----------------------------------------------------------------
// <copyright file="ILocatable.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common.Contracts.Abstractions
{
    using Fibula.Common.Contracts.Structs;

    /// <summary>
    /// Interface for all entities with a location.
    /// </summary>
    public interface ILocatable
    {
        /// <summary>
        /// Gets this entity's location.
        /// </summary>
        Location Location { get; }

        /// <summary>
        /// Gets the location where this entity is being carried at, if any.
        /// </summary>
        Location? CarryLocation { get; }
    }
}
