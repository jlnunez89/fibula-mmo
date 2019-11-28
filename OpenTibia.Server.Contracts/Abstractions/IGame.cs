// -----------------------------------------------------------------
// <copyright file="IGame.cs" company="2Dudes">
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
    using System;
    using Microsoft.Extensions.Hosting;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Data.Entities;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Interface for the game instance.
    /// </summary>
    public interface IGame : IHostedService
    {
        /// <summary>
        /// Gets the game's current time.
        /// </summary>
        DateTimeOffset CurrentTime { get; }

        /// <summary>
        /// Gets the game world's light color.
        /// </summary>
        byte WorldLightColor { get; }

        /// <summary>
        /// Gets the game world's light level.
        /// </summary>
        byte WorldLightLevel { get; }

        /// <summary>
        /// Gets the game world's state.
        /// </summary>
        WorldState Status { get; }

        /// <summary>
        /// Attempts to log a player in to the game.
        /// </summary>
        /// <param name="character">The character that the player is logging in to.</param>
        /// <param name="connection">The connection that the player uses.</param>
        /// <returns>An instance of the new <see cref="IPlayer"/> in the game.</returns>
        IPlayer PlayerLogin(CharacterEntity character, IConnection connection);

        /// <summary>
        /// Attempts to log a player out of the game.
        /// </summary>
        /// <param name="player">The player to attempt to attempt log out.</param>
        /// <returns>True if the log-out was successful, false otherwise.</returns>
        bool PlayerLogout(IPlayer player);

        /// <summary>
        /// Attempts to schedule a creature's walk in the provided direction.
        /// </summary>
        /// <param name="player">The player making the request.</param>
        /// <param name="direction">The direction to walk to.</param>
        /// <returns>True if the walk was possible and scheduled, false otherwise.</returns>
        bool PlayerWalkToDirection(IPlayer player, Direction direction);

        bool MoveThingBetweenTiles(IThing thing, Location fromTileLocation, byte fromTileStackPos, Location toTileLocation, byte amountToMove = 1, bool isTeleport = false);

        /// <summary>
        /// Requests a notification be scheduled to be sent.
        /// </summary>
        /// <param name="notification">The notification.</param>
        void RequestNofitication(INotification notification);

        ReadOnlyMemory<byte> GetDescriptionOfMapForPlayer(IPlayer player, Location location);

        ReadOnlyMemory<byte> GetDescriptionOfMapForPlayer(IPlayer player, ushort fromX, ushort toX, ushort fromY, ushort toY, sbyte currentZ, sbyte toZ);

        ReadOnlyMemory<byte> GetDescriptionOfTile(IPlayer player, Location location);
    }
}