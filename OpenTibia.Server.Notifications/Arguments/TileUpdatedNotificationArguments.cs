// -----------------------------------------------------------------
// <copyright file="TileUpdatedNotificationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Notifications.Arguments
{
    using System;
    using System.Buffers;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Notifications;
    using OpenTibia.Server.Notifications.Contracts.Abstractions;

    /// <summary>
    /// Class that represents arguments for the <see cref="TileUpdatedNotification"/>.
    /// </summary>
    public class TileUpdatedNotificationArguments : INotificationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TileUpdatedNotificationArguments"/> class.
        /// </summary>
        /// <param name="location">The location of the updated tile.</param>
        /// <param name="descriptionFunction">The function used to get the description of the updated tile.</param>
        public TileUpdatedNotificationArguments(Location location, Func<IPlayer, Location, ReadOnlySequence<byte>> descriptionFunction)
        {
            this.Location = location;
            this.TileDescriptionFunction = descriptionFunction;
        }

        /// <summary>
        /// Gets the location of the updated tile.
        /// </summary>
        public Location Location { get; }

        /// <summary>
        /// Gets the function that decribes the tile.
        /// </summary>
        public Func<IPlayer, Location, ReadOnlySequence<byte>> TileDescriptionFunction { get; }
    }
}