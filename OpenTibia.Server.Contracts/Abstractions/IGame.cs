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
    using System.Buffers;
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
        /// Requests a notification be scheduled to be sent.
        /// </summary>
        /// <param name="notification">The notification.</param>
        void RequestNofitication(INotification notification);

        /// <summary>
        /// Gets the description bytes of the map in behalf of a given player at a given location.
        /// </summary>
        /// <param name="player">The player for which the description is being retrieved for.</param>
        /// <param name="location">The center location from which the description is being retrieved.</param>
        /// <returns>A sequence of bytes representing the description.</returns>
        ReadOnlySequence<byte> GetDescriptionOfMapForPlayer(IPlayer player, Location location);

        /// <summary>
        /// Gets the description bytes of the map in behalf of a given player for the specified window.
        /// </summary>
        /// <param name="player">The player for which the description is being retrieved for.</param>
        /// <param name="startX">The starting X coordinate of the window.</param>
        /// <param name="startY">The starting Y coordinate of the window.</param>
        /// <param name="startZ">The starting Z coordinate of the window.</param>
        /// <param name="endZ">The ending Z coordinate of the window.</param>
        /// <param name="windowSizeX">The size of the window in X.</param>
        /// <param name="windowSizeY">The size of the window in Y.</param>
        /// <param name="startingZOffset">Optional. A starting offset for Z.</param>
        /// <returns>A sequence of bytes representing the description.</returns>
        ReadOnlySequence<byte> GetDescriptionOfMapForPlayer(IPlayer player, ushort startX, ushort startY, sbyte startZ, sbyte endZ, byte windowSizeX = IMap.DefaultWindowSizeX, byte windowSizeY = IMap.DefaultWindowSizeY, sbyte startingZOffset = 0);

        /// <summary>
        /// Gets the description bytes of a single tile of the map in behalf of a given player at a given location.
        /// </summary>
        /// <param name="player">The player for which the description is being retrieved for.</param>
        /// <param name="location">The location from which the description of the tile is being retrieved.</param>
        /// <returns>A sequence of bytes representing the tile's description.</returns>
        ReadOnlySequence<byte> GetDescriptionOfTile(IPlayer player, Location location);

        /// <summary>
        /// Attempts to schedule a player's auto walk movements.
        /// </summary>
        /// <param name="player">The player making the request.</param>
        /// <param name="directions">The directions to walk to.</param>
        /// <returns>True if the auto walk request was accepted, false otherwise.</returns>
        bool PlayerRequest_AutoWalk(IPlayer player, Direction[] directions);

        /// <summary>
        /// Attempts to cancel all of a player's pending movements.
        /// </summary>
        /// <param name="player">The player making the request.</param>
        /// <returns>True if the request was accepted, false otherwise.</returns>
        bool PlayerRequest_CancelPendingMovements(IPlayer player);

        /// <summary>
        /// Attempts to log a player in to the game.
        /// </summary>
        /// <param name="character">The character that the player is logging in to.</param>
        /// <param name="connection">The connection that the player uses.</param>
        /// <returns>An instance of the new <see cref="IPlayer"/> in the game, or null if it couldn't be instantiated.</returns>
        IPlayer PlayerRequest_Login(CharacterEntity character, IConnection connection);

        /// <summary>
        /// Attempts to log a player out of the game.
        /// </summary>
        /// <param name="player">The player to attempt to attempt log out.</param>
        /// <returns>True if the log-out request was accepted, false otherwise.</returns>
        bool PlayerRequest_Logout(IPlayer player);

        /// <summary>
        /// Attempts to schedule a creature's walk in the provided direction.
        /// </summary>
        /// <param name="player">The player making the request.</param>
        /// <param name="direction">The direction to walk to.</param>
        /// <returns>True if the walk request was accepted, false otherwise.</returns>
        bool PlayerRequest_WalkToDirection(IPlayer player, Direction direction);

        /// <summary>
        /// Attempts to turn a player to the requested direction.
        /// </summary>
        /// <param name="player">The player making the request.</param>
        /// <param name="direction">The direction to turn to.</param>
        /// <returns>True if the turn request was accepted, false otherwise.</returns>
        bool PlayerRequest_TurnToDirection(IPlayer player, Direction direction);

        /// <summary>
        /// Attempts to move a thing on behalf of a player.
        /// </summary>
        /// <param name="player">The player making the request.</param>
        /// <param name="thingId">The id of the thing attempting to be moved.</param>
        /// <param name="fromLocation">The location from which the thing is being moved.</param>
        /// <param name="fromStackPos">The position in the stack of the location from which the thing is being moved.</param>
        /// <param name="toLocation">The location to which the thing is being moved.</param>
        /// <param name="count">The amount of the thing that is being moved.</param>
        /// <returns>True if the thing movement was accepted, false otherwise.</returns>
        bool PlayerRequest_MoveThing(IPlayer player, ushort thingId, Location fromLocation, byte fromStackPos, Location toLocation, byte count);

        /// <summary>
        /// Attempts to use an item on behalf of a player.
        /// </summary>
        /// <param name="player">The player making the request.</param>
        /// <param name="itemClientId">The id of the item attempting to be used.</param>
        /// <param name="fromLocation">The location from which the item is being used.</param>
        /// <param name="fromStackPos">The position in the stack of the location from which the item is being used.</param>
        /// <param name="index">The index of the item being used.</param>
        /// <returns>True if the use item request was accepted, false otherwise.</returns>
        bool PlayerRequest_UseItem(IPlayer player, ushort itemClientId, Location fromLocation, byte fromStackPos, byte index);

        /// <summary>
        /// Inmediately attempts to perform a thing movement between two tiles.
        /// </summary>
        /// <param name="thing">The thing being moved.</param>
        /// <param name="fromTileLocation">The tile from which the movement is being performed.</param>
        /// <param name="toTileLocation">The tile to which the movement is being performed.</param>
        /// <param name="fromTileStackPos">Optional. The position in the stack of the tile from which the movement is being performed. Defaults to <see cref="byte.MaxValue"/> which signals to attempt to find the thing from the source location.</param>
        /// <param name="amountToMove">Optional. The amount of the thing to move. Defaults to 1.</param>
        /// <param name="isTeleport">Optional. A value indicating whether the move is considered a teleportation. Defaults to false.</param>
        /// <returns>True if the movement was successfully performed, false otherwise.</returns>
        /// <remarks>Changes game state, should only be performed after all pertinent validations happen.</remarks>
        bool PerformThingMovementBetweenTiles(IThing thing, Location fromTileLocation, Location toTileLocation, byte fromTileStackPos = byte.MaxValue, byte amountToMove = 1, bool isTeleport = false);

        /// <summary>
        /// Inmediately attempts to perform an item use in behalf of the requesting creature, if any.
        /// </summary>
        /// <param name="itemId">The id of the item being used.</param>
        /// <param name="fromLocation">The location from which the use is happening.</param>
        /// <param name="fromStackPos">The position in the stack of the item at the location.</param>
        /// <param name="index">The index of the item to use.</param>
        /// <param name="requestor">Optional. The creature requesting the use.</param>
        /// <returns>True if the item was successfully used, false otherwise.</returns>
        /// <remarks>Changes game state, should only be performed after all pertinent validations happen.</remarks>
        bool PerformItemUse(ushort itemId, Location fromLocation, byte fromStackPos, byte index, ICreature requestor = null);

        /// <summary>
        /// Evaluates separation event rules on the given location for the given thing, on behalf of the supplied requestor creature.
        /// </summary>
        /// <param name="fromLocation">The location from which the events take place.</param>
        /// <param name="thingMoving">The thing that is moving.</param>
        /// <param name="requestor">The requestor creature, if any.</param>
        /// <returns>True if there is at least one rule that was executed, false otherwise.</returns>
        bool EvaluateSeparationEventRules(Location fromLocation, IThing thingMoving, ICreature requestor);

        /// <summary>
        /// Evaluates collision event rules on the given location for the given thing, on behalf of the supplied requestor creature.
        /// </summary>
        /// <param name="toLocation">The location to which the events take place.</param>
        /// <param name="thingMoving">The thing that is moving.</param>
        /// <param name="requestor">The requestor creature, if any.</param>
        /// <returns>True if there is at least one rule that was executed, false otherwise.</returns>
        bool EvaluateCollisionEventRules(Location toLocation, IThing thingMoving, ICreature requestor);

        /// <summary>
        /// Attempts to display an animated efect on the given location.
        /// </summary>
        /// <param name="location">The location at which to display the effect.</param>
        /// <param name="animatedEffect">The effect to display.</param>
        /// <returns>True if the request was accepted, false otherwise.</returns>
        bool ScriptRequest_AddAnimatedEffectAt(Location location, AnimatedEffect animatedEffect);

        /// <summary>
        /// Attempts to change a given item to the supplied id.
        /// </summary>
        /// <param name="thing">The thing to change.</param>
        /// <param name="toItemId">The id of the item type to change to.</param>
        /// <param name="animatedEffect">An optional effect to send as part of the change.</param>
        /// <returns>True if the request was accepted, false otherwise.</returns>
        bool ScriptRequest_ChangeItem(ref IThing thing, ushort toItemId, AnimatedEffect animatedEffect);

        /// <summary>
        /// Attempts to change a given item to the supplied id at a given location.
        /// </summary>
        /// <param name="location">The location at which the change will happen.</param>
        /// <param name="fromItemId">The id of the item from which the change is happening.</param>
        /// <param name="toItemId">The id of the item to which the change is happening.</param>
        /// <param name="animatedEffect">An optional effect to send as part of the change.</param>
        /// <returns>True if the request was accepted, false otherwise.</returns>
        bool ScriptRequest_ChangeItemAt(Location location, ushort fromItemId, ushort toItemId, AnimatedEffect animatedEffect);

        /// <summary>
        /// Attempts to create an item at a given location.
        /// </summary>
        /// <param name="location">The location at which to create the item.</param>
        /// <param name="itemType">The type of the item to create.</param>
        /// <param name="effect">An effect to use when the creation takes place.</param>
        /// <returns>True if the request was accepted, false otherwise.</returns>
        bool ScriptRequest_CreateItemAt(Location location, ushort itemType, byte effect);

        /// <summary>
        /// Attempts to delete an item.
        /// </summary>
        /// <param name="item">The item to delete.</param>
        /// <returns>True if the request was accepted, false otherwise.</returns>
        bool ScriptRequest_DeleteItem(IItem item);

        /// <summary>
        /// Attempts to delete an item at a given location.
        /// </summary>
        /// <param name="location">The location at which to delete the item.</param>
        /// <param name="itemType">The type of the item to delete.</param>
        /// <returns>True if the request was accepted, false otherwise.</returns>
        bool ScriptRequest_DeleteItemAt(Location location, ushort itemType);

        /// <summary>
        /// Attempts to move a creature to a given location.
        /// </summary>
        /// <param name="creature">The creature to move.</param>
        /// <param name="targetLocation">The location to move the creature to.</param>
        /// <returns>True if the request was accepted, false otherwise.</returns>
        bool ScriptRequest_MoveCreature(ICreature creature, Location targetLocation);

        /// <summary>
        /// Attempts to move all items and creatures in a location to a given location.
        /// </summary>
        /// <param name="fromLocation">The location from which to move everything.</param>
        /// <param name="targetLocation">The location to move everything to.</param>
        /// <returns>True if the request was accepted, false otherwise.</returns>
        bool ScriptRequest_MoveEverythingTo(Location fromLocation, Location targetLocation);

        /// <summary>
        /// Attempts to move an item to a given location.
        /// </summary>
        /// <param name="item">The item to move.</param>
        /// <param name="targetLocation">The location to move the item to.</param>
        /// <returns>True if the request was accepted, false otherwise.</returns>
        bool ScriptRequest_MoveItemTo(IItem item, Location targetLocation);

        /// <summary>
        /// Attempts to move an item of the given type from the given location to another location.
        /// </summary>
        /// <param name="itemType">The type of the item to move.</param>
        /// <param name="fromLocation">The location from which to move the item.</param>
        /// <param name="toLocation">The location to which to move the item.</param>
        /// <returns>True if the request was accepted, false otherwise.</returns>
        bool ScriptRequest_MoveItemByIdTo(ushort itemType, Location fromLocation, Location toLocation);

        /// <summary>
        /// Attempts to log out a player.
        /// </summary>
        /// <param name="player">The player to log out.</param>
        /// <returns>True if the request was accepted, false otherwise.</returns>
        bool ScriptRequest_Logout(IPlayer player);

        /// <summary>
        /// Attempts to place a new monster at the given location.
        /// </summary>
        /// <param name="location">The location at which to place the monster.</param>
        /// <param name="monsterType">The type of the monster to place.</param>
        /// <returns>True if the request was accepted, false otherwise.</returns>
        bool ScriptRequest_PlaceMonsterAt(Location location, ushort monsterType);
    }
}