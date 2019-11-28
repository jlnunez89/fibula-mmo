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

namespace OpenTibia.Server.Notifications
{
    using System;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;

    internal class TileUpdatedNotificationArguments : INotificationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TileUpdatedNotificationArguments"/> class.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="descriptionFunction"></param>
        public TileUpdatedNotificationArguments(Location location, Func<IPlayer, Location, ReadOnlyMemory<byte>> descriptionFunction)
        {
            this.Location = location;
            this.TileDescriptionFunction = descriptionFunction;
        }

        public Location Location { get; }

        public Func<IPlayer, Location, ReadOnlyMemory<byte>> TileDescriptionFunction { get; }
    }
}