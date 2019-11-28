// -----------------------------------------------------------------
// <copyright file="IThing.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Interface for all things in the game.
    /// </summary>
    public interface IThing
    {
        /// <summary>
        /// Event to invoke when any of the properties of this thing have changed.
        /// </summary>
        event OnThingStateChanged OnThingChanged;

        /// <summary>
        /// Gets the id of this thing.
        /// </summary>
        ushort ThingId { get; }

        /// <summary>
        /// Gets or sets this thing's location.
        /// </summary>
        Location Location { get; set; }

        /// <summary>
        /// Gets the description of the thing.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the inspection text of the thing.
        /// </summary>
        string InspectionText { get; }

        /// <summary>
        /// Gets a value indicating whether this thing can be moved.
        /// </summary>
        bool CanBeMoved { get; }
    }
}
