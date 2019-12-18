// -----------------------------------------------------------------
// <copyright file="ItemEventType.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Enumerations
{
    using OpenTibia.Server.Contracts.Abstractions;

    /// <summary>
    /// Enumerates the different types of an <see cref="IEventRule"/>.
    /// </summary>
    public enum EventRuleType : byte
    {
        /// <summary>
        /// Using an item.
        /// </summary>
        Use,

        /// <summary>
        /// Using an item on something else.
        /// </summary>
        MultiUse,

        /// <summary>
        /// Movement event.
        /// </summary>
        Movement,

        /// <summary>
        /// Collision event.
        /// </summary>
        Collision,

        /// <summary>
        /// Separation event.
        /// </summary>
        Separation,
    }
}
