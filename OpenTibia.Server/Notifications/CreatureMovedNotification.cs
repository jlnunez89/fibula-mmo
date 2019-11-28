// -----------------------------------------------------------------
// <copyright file="CreatureMovedNotification.cs" company="2Dudes">
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
    using System.Collections.Generic;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using Serilog;

    /// <summary>
    /// Class that represents a notification for a creature moving.
    /// </summary>
    internal class CreatureMovedNotification : Notification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureMovedNotification"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="game">A reference to the game instance.</param>
        /// <param name="creatureFinder">A reference to the creature finder instance.</param>
        /// <param name="determineTargetConnectionsFunction">A function to determine the target connections of this notification.</param>
        /// <param name="arguments">The arguments for this notification.</param>
        public CreatureMovedNotification(ILogger logger, IGame game, ICreatureFinder creatureFinder, Func<IEnumerable<IConnection>> determineTargetConnectionsFunction, CreatureMovedNotificationArguments arguments)
            : base(logger)
        {
            determineTargetConnectionsFunction.ThrowIfNull(nameof(determineTargetConnectionsFunction));
            arguments.ThrowIfNull(nameof(arguments));

            this.TargetConnectionsFunction = determineTargetConnectionsFunction;
            this.Arguments = arguments;

            this.Game = game;
            this.CreatureFinder = creatureFinder;
        }

        /// <summary>
        /// Gets this notification's arguments.
        /// </summary>
        public CreatureMovedNotificationArguments Arguments { get; }

        /// <summary>
        /// Gets the game reference.
        /// </summary>
        public IGame Game { get; }

        /// <summary>
        /// Gets the reference to the creature finder.
        /// </summary>
        public ICreatureFinder CreatureFinder { get; }

        /// <summary>
        /// Gets the function for determining target connections for this notification.
        /// </summary>
        protected override Func<IEnumerable<IConnection>> TargetConnectionsFunction { get; }

        /// <summary>
        /// Finalizes the notification in preparation to it being sent.
        /// </summary>
        /// <param name="playerId">The id of the player which this notification is being prepared for.</param>
        /// <returns>A collection of <see cref="IOutgoingPacket"/>s, the ones to be sent.</returns>
        protected override IEnumerable<IOutgoingPacket> Prepare(uint playerId)
        {
            if (!(this.CreatureFinder.FindCreatureById(playerId) is IPlayer player))
            {
                return null;
            }

            var packets = new List<IOutgoingPacket>();
            var creature = this.CreatureFinder.FindCreatureById(this.Arguments.CreatureId);

            if (this.Arguments.CreatureId == playerId)
            {
                if (this.Arguments.WasTeleport)
                {
                    if (this.Arguments.OldStackPosition <= IMap.MaximumNumberOfThingsToDescribePerTile)
                    {
                        // Since this was described to the client before, we send a packet that lets them know the thing must be removed from that Tile's stack.
                        packets.Add(new RemoveAtStackposPacket(this.Arguments.OldLocation, this.Arguments.OldStackPosition));
                    }

                    // Then send the entire description at the new location.
                    packets.Add(new MapDescriptionPacket(this.Arguments.NewLocation, this.Game.GetDescriptionOfMapForPlayer(player, this.Arguments.NewLocation)));

                    return null;
                }

                if (this.Arguments.OldLocation.Z == 7 && this.Arguments.NewLocation.Z > 7)
                {
                    if (this.Arguments.OldStackPosition <= IMap.MaximumNumberOfThingsToDescribePerTile)
                    {
                        packets.Add(new RemoveAtStackposPacket(this.Arguments.OldLocation, this.Arguments.OldStackPosition));
                    }
                }
                else
                {
                    packets.Add(new CreatureMovedPacket(this.Arguments.OldLocation, this.Arguments.OldStackPosition, this.Arguments.NewLocation));
                }

                // floor change down
                if (this.Arguments.NewLocation.Z > this.Arguments.OldLocation.Z)
                {
                    // going from surface to underground
                    if (this.Arguments.NewLocation.Z == 8)
                    {
                        packets.Add(new MapPartialDescriptionPacket(
                            OutgoingGamePacketType.FloorChangeDown,
                            this.Game.GetDescriptionOfMapForPlayer(
                                player,
                                (ushort)(this.Arguments.OldLocation.X - 8),
                                (ushort)(this.Arguments.OldLocation.X - 8 + IMap.DefaultWindowSizeX - 1),
                                (ushort)(this.Arguments.OldLocation.Y - 6),
                                (ushort)(this.Arguments.OldLocation.Y - 6 + IMap.DefaultWindowSizeY - 1),
                                (sbyte)(this.Arguments.NewLocation.Z - 1),
                                (sbyte)(this.Arguments.NewLocation.Z + 2 - 1))));
                    }

                    // going further down
                    else if (this.Arguments.NewLocation.Z > this.Arguments.OldLocation.Z && this.Arguments.NewLocation.Z > 8 && this.Arguments.NewLocation.Z < IMap.DefaultWindowSizeY)
                    {
                        packets.Add(new MapPartialDescriptionPacket(
                            OutgoingGamePacketType.FloorChangeDown,
                            this.Game.GetDescriptionOfMapForPlayer(
                                player,
                                (ushort)(this.Arguments.OldLocation.X - 8),
                                (ushort)(this.Arguments.OldLocation.X - 8 + IMap.DefaultWindowSizeX - 1),
                                (ushort)(this.Arguments.OldLocation.Y - 6),
                                (ushort)(this.Arguments.OldLocation.Y - 6 + IMap.DefaultWindowSizeY - 1),
                                (sbyte)(this.Arguments.NewLocation.Z + 2 - 3),
                                (sbyte)(this.Arguments.NewLocation.Z + 2 - 3))));
                    }
                    else
                    {
                        packets.Add(new MapPartialDescriptionPacket(OutgoingGamePacketType.FloorChangeDown, new byte[0]));
                    }

                    // moving down a floor makes us out of sync, include east and south
                    packets.Add(new MapPartialDescriptionPacket(
                        OutgoingGamePacketType.MapSliceEast,
                        this.Game.GetDescriptionOfMapForPlayer(
                            player,
                            (ushort)(this.Arguments.OldLocation.X + 9),
                            (ushort)(this.Arguments.OldLocation.X + 9),
                            (ushort)(this.Arguments.OldLocation.Y - 7),
                            (ushort)(this.Arguments.OldLocation.Y - 7 + IMap.DefaultWindowSizeY - 1),
                            this.Arguments.NewLocation.Z,
                            this.Arguments.NewLocation.Z)));

                    // south
                    packets.Add(new MapPartialDescriptionPacket(
                        OutgoingGamePacketType.MapSliceSouth,
                        this.Game.GetDescriptionOfMapForPlayer(
                            player,
                            (ushort)(this.Arguments.OldLocation.X - 8),
                            (ushort)(this.Arguments.OldLocation.X - 8 + IMap.DefaultWindowSizeX - 1),
                            (ushort)(this.Arguments.OldLocation.Y + 7),
                            (ushort)(this.Arguments.OldLocation.Y + 7),
                            this.Arguments.NewLocation.Z,
                            this.Arguments.NewLocation.Z)));
                }

                // floor change up
                else if (this.Arguments.NewLocation.Z < this.Arguments.OldLocation.Z)
                {
                    // going to surface
                    if (this.Arguments.NewLocation.Z == 7)
                    {
                        packets.Add(new MapPartialDescriptionPacket(
                            OutgoingGamePacketType.FloorChangeUp,
                            this.Game.GetDescriptionOfMapForPlayer(
                                player,
                                (ushort)(this.Arguments.OldLocation.X - 8),
                                (ushort)(this.Arguments.OldLocation.X - 8 + IMap.DefaultWindowSizeX - 1),
                                (ushort)(this.Arguments.OldLocation.Y - 6),
                                (ushort)(this.Arguments.OldLocation.Y - 6 + IMap.DefaultWindowSizeY - 1),
                                5,
                                0)));
                    }

                    // underground, going one floor up (still underground)
                    else if (this.Arguments.NewLocation.Z > 7)
                    {
                        packets.Add(new MapPartialDescriptionPacket(
                            OutgoingGamePacketType.FloorChangeUp,
                            this.Game.GetDescriptionOfMapForPlayer(
                                player,
                                (ushort)(this.Arguments.OldLocation.X - 8),
                                (ushort)(this.Arguments.OldLocation.X - 8 + IMap.DefaultWindowSizeX - 1),
                                (ushort)(this.Arguments.OldLocation.Y - 6),
                                (ushort)(this.Arguments.OldLocation.Y - 6 + IMap.DefaultWindowSizeY - 1),
                                (sbyte)(this.Arguments.OldLocation.Z - 3),
                                (sbyte)(this.Arguments.OldLocation.Z - 3))));
                    }
                    else
                    {
                        packets.Add(new MapPartialDescriptionPacket(OutgoingGamePacketType.FloorChangeUp, new byte[0]));
                    }

                    // moving up a floor up makes us out of sync, include west and north
                    packets.Add(new MapPartialDescriptionPacket(
                        OutgoingGamePacketType.MapSliceWest,
                        this.Game.GetDescriptionOfMapForPlayer(
                            player,
                            (ushort)(this.Arguments.OldLocation.X - 8),
                            (ushort)(this.Arguments.OldLocation.X - 8),
                            (ushort)(this.Arguments.OldLocation.Y - 5),
                            (ushort)(this.Arguments.OldLocation.Y - 5 + IMap.DefaultWindowSizeY - 1),
                            this.Arguments.NewLocation.Z,
                            this.Arguments.NewLocation.Z)));

                    // north
                    packets.Add(new MapPartialDescriptionPacket(
                        OutgoingGamePacketType.MapSliceNorth,
                        this.Game.GetDescriptionOfMapForPlayer(
                            player,
                            (ushort)(this.Arguments.OldLocation.X - 8),
                            (ushort)(this.Arguments.OldLocation.X - 8 + IMap.DefaultWindowSizeX - 1),
                            (ushort)(this.Arguments.OldLocation.Y - 6),
                            (ushort)(this.Arguments.OldLocation.Y - 6),
                            this.Arguments.NewLocation.Z,
                            this.Arguments.NewLocation.Z)));
                }

                if (this.Arguments.OldLocation.Y > this.Arguments.NewLocation.Y)
                {
                    // north, for old x
                    packets.Add(new MapPartialDescriptionPacket(
                        OutgoingGamePacketType.MapSliceNorth,
                        this.Game.GetDescriptionOfMapForPlayer(
                            player,
                            (ushort)(this.Arguments.OldLocation.X - 8),
                            (ushort)(this.Arguments.OldLocation.X - 8 + IMap.DefaultWindowSizeX - 1),
                            (ushort)(this.Arguments.NewLocation.Y - 6),
                            (ushort)(this.Arguments.NewLocation.Y - 6),
                            (sbyte)(this.Arguments.NewLocation.IsUnderground ? this.Arguments.NewLocation.Z - 2 : 7),
                            (sbyte)(this.Arguments.NewLocation.IsUnderground ? this.Arguments.NewLocation.Z + 2 : 0))));
                }
                else if (this.Arguments.OldLocation.Y < this.Arguments.NewLocation.Y)
                {
                    // south, for old x
                    packets.Add(new MapPartialDescriptionPacket(
                        OutgoingGamePacketType.MapSliceSouth,
                        this.Game.GetDescriptionOfMapForPlayer(
                            player,
                            (ushort)(this.Arguments.OldLocation.X - 8),
                            (ushort)(this.Arguments.OldLocation.X - 8 + IMap.DefaultWindowSizeX - 1),
                            (ushort)(this.Arguments.NewLocation.Y + 7),
                            (ushort)(this.Arguments.NewLocation.Y + 7),
                            (sbyte)(this.Arguments.NewLocation.IsUnderground ? this.Arguments.NewLocation.Z - 2 : 7),
                            (sbyte)(this.Arguments.NewLocation.IsUnderground ? this.Arguments.NewLocation.Z + 2 : 0))));
                }

                if (this.Arguments.OldLocation.X < this.Arguments.NewLocation.X)
                {
                    // east, [with new y]
                    packets.Add(new MapPartialDescriptionPacket(
                        OutgoingGamePacketType.MapSliceEast,
                        this.Game.GetDescriptionOfMapForPlayer(
                            player,
                            (ushort)(this.Arguments.NewLocation.X + 9),
                            (ushort)(this.Arguments.NewLocation.X + 9),
                            (ushort)(this.Arguments.NewLocation.Y - 6),
                            (ushort)(this.Arguments.NewLocation.Y - 6 + IMap.DefaultWindowSizeY - 1),
                            (sbyte)(this.Arguments.NewLocation.IsUnderground ? this.Arguments.NewLocation.Z - 2 : 7),
                            (sbyte)(this.Arguments.NewLocation.IsUnderground ? this.Arguments.NewLocation.Z + 2 : 0))));
                }
                else if (this.Arguments.OldLocation.X > this.Arguments.NewLocation.X)
                {
                    // west, [with new y]
                    packets.Add(new MapPartialDescriptionPacket(
                        OutgoingGamePacketType.MapSliceWest,
                        this.Game.GetDescriptionOfMapForPlayer(
                            player,
                            (ushort)(this.Arguments.NewLocation.X - 8),
                            (ushort)(this.Arguments.NewLocation.X - 8),
                            (ushort)(this.Arguments.NewLocation.Y - 6),
                            (ushort)(this.Arguments.NewLocation.Y - 6 + IMap.DefaultWindowSizeY - 1),
                            (sbyte)(this.Arguments.NewLocation.IsUnderground ? this.Arguments.NewLocation.Z - 2 : 7),
                            (sbyte)(this.Arguments.NewLocation.IsUnderground ? this.Arguments.NewLocation.Z + 2 : 0))));
                }
            }
            else if (player.CanSee(this.Arguments.OldLocation) && player.CanSee(this.Arguments.NewLocation))
            {
                if (player.CanSee(creature))
                {
                    if (this.Arguments.WasTeleport || (this.Arguments.OldLocation.Z == 7 && this.Arguments.NewLocation.Z > 7) || this.Arguments.OldStackPosition > 9)
                    {
                        if (this.Arguments.OldStackPosition <= IMap.MaximumNumberOfThingsToDescribePerTile)
                        {
                            packets.Add(new RemoveAtStackposPacket(this.Arguments.OldLocation, this.Arguments.OldStackPosition));
                        }

                        packets.Add(new AddCreaturePacket(creature, player.KnowsCreatureWithId(this.Arguments.CreatureId), player.ChooseToRemoveFromKnownSet()));
                    }
                    else
                    {
                        packets.Add(new CreatureMovedPacket(this.Arguments.OldLocation, this.Arguments.OldStackPosition, this.Arguments.NewLocation));
                    }
                }
            }
            else if (player.CanSee(this.Arguments.OldLocation) && player.CanSee(creature))
            {
                if (this.Arguments.OldStackPosition <= IMap.MaximumNumberOfThingsToDescribePerTile)
                {
                    packets.Add(new RemoveAtStackposPacket(this.Arguments.OldLocation, this.Arguments.OldStackPosition));
                }
            }
            else if (player.CanSee(this.Arguments.NewLocation) && player.CanSee(creature))
            {
                packets.Add(new AddCreaturePacket(creature, player.KnowsCreatureWithId(this.Arguments.CreatureId), player.ChooseToRemoveFromKnownSet()));
            }

            if (this.Arguments.WasTeleport)
            {
                packets.Add(new MagicEffectPacket(this.Arguments.NewLocation, AnimatedEffect.BubbleBlue));
            }

            return packets;
        }
    }
}