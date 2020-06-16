// -----------------------------------------------------------------
// <copyright file="ICollisionEventRule.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Abstractions
{
    /// <summary>
    /// Interface for collision event rules.
    /// </summary>
    public interface ICollisionEventRule : IEventRule
    {
        /// <summary>
        /// Gets the id of the thing involved in the collision.
        /// </summary>
        public ushort CollidingThingId { get; }
    }
}