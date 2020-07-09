// -----------------------------------------------------------------
// <copyright file="TileUpdatedNotificationArguments.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Notifications.Arguments
{
    using System;
    using System.Buffers;
    using System.Collections.Generic;
    using Fibula.Common.Contracts.Structs;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Notifications.Contracts.Abstractions;

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
        public TileUpdatedNotificationArguments(Location location, Func<IPlayer, Location, (IDictionary<string, object> descriptionMetadata, ReadOnlySequence<byte> descriptionData)> descriptionFunction)
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
        public Func<IPlayer, Location, (IDictionary<string, object> descriptionMetadata, ReadOnlySequence<byte> descriptionData)> TileDescriptionFunction { get; }
    }
}
