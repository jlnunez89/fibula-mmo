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
    using System.Buffers;
    using System.Collections.Generic;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Notifications.Arguments;
    using Serilog;

    /// <summary>
    /// Class that represents a notification for when a creature has moved.
    /// </summary>
    public class CreatureMovedNotification : Notification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureMovedNotification"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="mapDescriptor">A reference to the map descriptor in use.</param>
        /// <param name="creatureFinder">A reference to the creature finder instance.</param>
        /// <param name="determineTargetConnectionsFunction">A function to determine the target connections of this notification.</param>
        /// <param name="arguments">The arguments for this notification.</param>
        public CreatureMovedNotification(ILogger logger, IMapDescriptor mapDescriptor, ICreatureFinder creatureFinder, Func<IEnumerable<IConnection>> determineTargetConnectionsFunction, CreatureMovedNotificationArguments arguments)
            : base(logger)
        {
            mapDescriptor.ThrowIfNull(nameof(mapDescriptor));
            determineTargetConnectionsFunction.ThrowIfNull(nameof(determineTargetConnectionsFunction));
            arguments.ThrowIfNull(nameof(arguments));

            this.TargetConnectionsFunction = determineTargetConnectionsFunction;
            this.Arguments = arguments;

            this.MapDescriptor = mapDescriptor;
            this.CreatureFinder = creatureFinder;
        }

        /// <summary>
        /// Gets this notification's arguments.
        /// </summary>
        public CreatureMovedNotificationArguments Arguments { get; }

        /// <summary>
        /// Gets the game reference.
        /// </summary>
        public IMapDescriptor MapDescriptor { get; }

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
                    packets.Add(new MapDescriptionPacket(this.Arguments.NewLocation, this.MapDescriptor.DescribeAt(player, this.Arguments.NewLocation)));

                    return packets;
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
                    var windowStartLocation = new Location()
                    {
                        X = this.Arguments.OldLocation.X - ((IMap.DefaultWindowSizeX / 2) - 1), // -8
                        Y = this.Arguments.OldLocation.Y - ((IMap.DefaultWindowSizeY / 2) - 1), // -6
                        Z = this.Arguments.NewLocation.Z,
                    };

                    ReadOnlySequence<byte> description;

                    // going from surface to underground
                    if (this.Arguments.NewLocation.Z == 8)
                    {
                        // Client already has the two floors above (6 and 7), so it needs 8 (new current), and 2 below.
                        description = this.MapDescriptor.DescribeWindow(player, (ushort)windowStartLocation.X, (ushort)windowStartLocation.Y, this.Arguments.NewLocation.Z, (sbyte)(this.Arguments.NewLocation.Z + 2), IMap.DefaultWindowSizeX, IMap.DefaultWindowSizeY, -1);
                    }

                    // going further down underground; watch for world's deepest floor (hardcoded for now).
                    else if (this.Arguments.NewLocation.Z > 8 && this.Arguments.NewLocation.Z < 14)
                    {
                        // Client already has all floors needed except the new deepest floor, so it needs the 2th floor below the current.
                        description = this.MapDescriptor.DescribeWindow(player, (ushort)windowStartLocation.X, (ushort)windowStartLocation.Y, (sbyte)(this.Arguments.NewLocation.Z + 2), (sbyte)(this.Arguments.NewLocation.Z + 2), IMap.DefaultWindowSizeX, IMap.DefaultWindowSizeY, -3);
                    }

                    // going down but still above surface, so client has all floors.
                    else
                    {
                        description = new ReadOnlySequence<byte>(new byte[0]);
                    }

                    packets.Add(new MapPartialDescriptionPacket(OutgoingGamePacketType.FloorChangeDown, description));

                    // moving down a floor makes us out of sync, include east and south
                    packets.Add(
                        this.EastSliceDescription(
                            player,
                            this.Arguments.OldLocation.X - this.Arguments.NewLocation.X,
                            this.Arguments.OldLocation.Y - this.Arguments.NewLocation.Y + this.Arguments.OldLocation.Z - this.Arguments.NewLocation.Z));

                    packets.Add(this.SouthSliceDescription(player, this.Arguments.OldLocation.Y - this.Arguments.NewLocation.Y));
                }

                // floor change up
                else if (this.Arguments.NewLocation.Z < this.Arguments.OldLocation.Z)
                {
                    var windowStartLocation = new Location()
                    {
                        X = this.Arguments.OldLocation.X - ((IMap.DefaultWindowSizeX / 2) - 1), // -8
                        Y = this.Arguments.OldLocation.Y - ((IMap.DefaultWindowSizeY / 2) - 1), // -6
                        Z = this.Arguments.NewLocation.Z,
                    };

                    ReadOnlySequence<byte> description;

                    // going to surface
                    if (this.Arguments.NewLocation.Z == 7)
                    {
                        // Client already has the first two above-the-ground floors (6 and 7), so it needs 0-5 above.
                        description = this.MapDescriptor.DescribeWindow(player, (ushort)windowStartLocation.X, (ushort)windowStartLocation.Y, 5, 0, IMap.DefaultWindowSizeX, IMap.DefaultWindowSizeY, 3);
                    }

                    // going up but still underground
                    else if (this.Arguments.NewLocation.Z > 7)
                    {
                        // Client already has all floors needed except the new highest floor, so it needs the 2th floor above the current.
                        description = this.MapDescriptor.DescribeWindow(player, (ushort)windowStartLocation.X, (ushort)windowStartLocation.Y, (sbyte)(this.Arguments.NewLocation.Z - 2), (sbyte)(this.Arguments.NewLocation.Z - 2), IMap.DefaultWindowSizeX, IMap.DefaultWindowSizeY, 3);
                    }

                    // already above surface, so client has all floors.
                    else
                    {
                        description = new ReadOnlySequence<byte>(ReadOnlyMemory<byte>.Empty);
                    }

                    packets.Add(new MapPartialDescriptionPacket(OutgoingGamePacketType.FloorChangeUp, description));

                    // moving up a floor up makes us out of sync, include west and north
                    packets.Add(
                        this.WestSliceDescription(
                            player,
                            this.Arguments.OldLocation.X - this.Arguments.NewLocation.X,
                            this.Arguments.OldLocation.Y - this.Arguments.NewLocation.Y + this.Arguments.OldLocation.Z - this.Arguments.NewLocation.Z));

                    packets.Add(this.NorthSliceDescription(player, this.Arguments.OldLocation.Y - this.Arguments.NewLocation.Y));
                }

                if (this.Arguments.OldLocation.Y > this.Arguments.NewLocation.Y)
                {
                    // Creature is moving north, so we need to send the additional north bytes.
                    packets.Add(this.NorthSliceDescription(player));
                }
                else if (this.Arguments.OldLocation.Y < this.Arguments.NewLocation.Y)
                {
                    // Creature is moving south, so we need to send the additional south bytes.
                    packets.Add(this.SouthSliceDescription(player));
                }

                if (this.Arguments.OldLocation.X < this.Arguments.NewLocation.X)
                {
                    // Creature is moving east, so we need to send the additional east bytes.
                    packets.Add(this.EastSliceDescription(player));
                }
                else if (this.Arguments.OldLocation.X > this.Arguments.NewLocation.X)
                {
                    // Creature is moving west, so we need to send the additional west bytes.
                    packets.Add(this.WestSliceDescription(player));
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

                        packets.Add(new AddCreaturePacket(creature, player.KnowsCreatureWithId(this.Arguments.CreatureId), player.ChooseCreatureToRemoveFromKnownSet()));
                    }
                    else
                    {
                        packets.Add(new CreatureMovedPacket(this.Arguments.OldLocation, this.Arguments.OldStackPosition, this.Arguments.NewLocation));
                    }
                }
            }
            else if (player.CanSee(this.Arguments.OldLocation) && !player.CanSee(creature))
            {
                if (this.Arguments.OldStackPosition <= IMap.MaximumNumberOfThingsToDescribePerTile)
                {
                    packets.Add(new RemoveAtStackposPacket(this.Arguments.OldLocation, this.Arguments.OldStackPosition));
                }
            }
            else if (player.CanSee(this.Arguments.NewLocation) && player.CanSee(creature))
            {
                if (this.Arguments.NewStackPosition <= IMap.MaximumNumberOfThingsToDescribePerTile)
                {
                    packets.Add(new AddCreaturePacket(creature, player.KnowsCreatureWithId(this.Arguments.CreatureId), player.ChooseCreatureToRemoveFromKnownSet()));
                }
            }

            if (this.Arguments.WasTeleport)
            {
                packets.Add(new MagicEffectPacket(this.Arguments.NewLocation, AnimatedEffect.BubbleBlue));
            }

            return packets;
        }

        private IOutgoingPacket NorthSliceDescription(IPlayer player, int floorChangeOffset = 0)
        {
            // A = old location, B = new location.
            //
            //       |---------- IMap.DefaultWindowSizeX = 18 ----------|
            //                           as seen by A
            //       x  ~  ~  ~  ~  ~  ~  ~  ~  ~  ~  ~  ~  ~  ~  ~  ~  ~
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .     ---
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .      |
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .      |
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .      |
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .      |
            //       .  .  .  .  .  .  .  .  B  .  .  .  .  .  .  .  .  .      |
            //       .  .  .  .  .  .  .  .  A  .  .  .  .  .  .  .  .  .      | IMap.DefaultWindowSizeY = 14
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .      | as seen by A
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .      |
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .      |
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .      |
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .      |
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .      |
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .     ---
            //
            // x = target start of window (~) to refresh.
            var windowStartLocation = new Location()
            {
                // -8
                X = this.Arguments.OldLocation.X - ((IMap.DefaultWindowSizeX / 2) - 1),

                // -6
                Y = this.Arguments.NewLocation.Y - ((IMap.DefaultWindowSizeY / 2) - 1 - floorChangeOffset),

                Z = this.Arguments.NewLocation.Z,
            };

            return new MapPartialDescriptionPacket(
                OutgoingGamePacketType.MapSliceNorth,
                this.MapDescriptor.DescribeWindow(
                    player,
                    (ushort)windowStartLocation.X,
                    (ushort)windowStartLocation.Y,
                    (sbyte)(this.Arguments.NewLocation.IsUnderground ? Math.Max(0, this.Arguments.NewLocation.Z - 2) : 7),
                    (sbyte)(this.Arguments.NewLocation.IsUnderground ? Math.Min(15, this.Arguments.NewLocation.Z + 2) : 0),
                    IMap.DefaultWindowSizeX,
                    1));
        }

        private IOutgoingPacket SouthSliceDescription(IPlayer player, int floorChangeOffset = 0)
        {
            // A = old location, B = new location
            //
            //       |---------- IMap.DefaultWindowSizeX = 18 ----------|
            //                           as seen by A
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .     ---
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .      |
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .      |
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .      |
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .      |
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .      |
            //       .  .  .  .  .  .  .  .  A  .  .  .  .  .  .  .  .  .      | IMap.DefaultWindowSizeY = 14
            //       .  .  .  .  .  .  .  .  B  .  .  .  .  .  .  .  .  .      | as seen by A
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .      |
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .      |
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .      |
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .      |
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .      |
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .     ---
            //       x  ~  ~  ~  ~  ~  ~  ~  ~  ~  ~  ~  ~  ~  ~  ~  ~  ~
            //
            // x = target start of window (~) to refresh.
            var windowStartLocation = new Location()
            {
                // -8
                X = this.Arguments.OldLocation.X - ((IMap.DefaultWindowSizeX / 2) - 1),

                // +7
                Y = this.Arguments.NewLocation.Y + (IMap.DefaultWindowSizeY / 2) + floorChangeOffset,

                Z = this.Arguments.NewLocation.Z,
            };

            return new MapPartialDescriptionPacket(
                OutgoingGamePacketType.MapSliceSouth,
                this.MapDescriptor.DescribeWindow(
                    player,
                    (ushort)windowStartLocation.X,
                    (ushort)windowStartLocation.Y,
                    (sbyte)(this.Arguments.NewLocation.IsUnderground ? Math.Max(0, this.Arguments.NewLocation.Z - 2) : 7),
                    (sbyte)(this.Arguments.NewLocation.IsUnderground ? Math.Min(15, this.Arguments.NewLocation.Z + 2) : 0),
                    IMap.DefaultWindowSizeX,
                    1));
        }

        private IOutgoingPacket EastSliceDescription(IPlayer player, int floorChangeOffsetX = 0, int floorChangeOffsetY = 0)
        {
            // A = old location, B = new location
            //
            //       |---------- IMap.DefaultWindowSizeX = 18 ----------|
            //                           as seen by A
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  x  ---
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  ~   |
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  ~   |
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  ~   |
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  ~   |
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  ~   |
            //       .  .  .  .  .  .  .  .  A  B  .  .  .  .  .  .  .  .  ~   | IMap.DefaultWindowSizeY = 14
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  ~   | as seen by A
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  ~   |
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  ~   |
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  ~   |
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  ~   |
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  ~   |
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  ~  ---
            //
            // x = target start of window (~) to refresh.
            var windowStartLocation = new Location()
            {
                // +9
                X = this.Arguments.NewLocation.X + (IMap.DefaultWindowSizeX / 2) + floorChangeOffsetX,

                // -6
                Y = this.Arguments.NewLocation.Y - ((IMap.DefaultWindowSizeY / 2) - 1) + floorChangeOffsetY,

                Z = this.Arguments.NewLocation.Z,
            };

            return new MapPartialDescriptionPacket(
                OutgoingGamePacketType.MapSliceEast,
                this.MapDescriptor.DescribeWindow(
                    player,
                    (ushort)windowStartLocation.X,
                    (ushort)windowStartLocation.Y,
                    (sbyte)(this.Arguments.NewLocation.IsUnderground ? Math.Max(0, this.Arguments.NewLocation.Z - 2) : 7),
                    (sbyte)(this.Arguments.NewLocation.IsUnderground ? Math.Min(15, this.Arguments.NewLocation.Z + 2) : 0),
                    1,
                    IMap.DefaultWindowSizeY));
        }

        private IOutgoingPacket WestSliceDescription(IPlayer player, int floorChangeOffsetX = 0, int floorChangeOffsetY = 0)
        {
            // A = old location, B = new location
            //
            //          |---------- IMap.DefaultWindowSizeX = 18 ----------|
            //                           as seen by A
            //       x  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  ---
            //       ~  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .   |
            //       ~  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .   |
            //       ~  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .   |
            //       ~  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .   |
            //       ~  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .   |
            //       ~  .  .  .  .  .  .  .  B  A  .  .  .  .  .  .  .  .  .   | IMap.DefaultWindowSizeY = 14
            //       ~  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .   | as seen by A
            //       ~  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .   |
            //       ~  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .   |
            //       ~  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .   |
            //       ~  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .   |
            //       ~  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .   |
            //       ~  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  ---
            //
            // x = target start of window (~) to refresh.
            var windowStartLocation = new Location()
            {
                // -8
                X = this.Arguments.NewLocation.X - ((IMap.DefaultWindowSizeX / 2) - 1) + floorChangeOffsetX,

                // -6
                Y = this.Arguments.NewLocation.Y - ((IMap.DefaultWindowSizeY / 2) - 1) + floorChangeOffsetY,

                Z = this.Arguments.NewLocation.Z,
            };

            return new MapPartialDescriptionPacket(
                OutgoingGamePacketType.MapSliceWest,
                this.MapDescriptor.DescribeWindow(
                    player,
                    (ushort)windowStartLocation.X,
                    (ushort)windowStartLocation.Y,
                    (sbyte)(this.Arguments.NewLocation.IsUnderground ? Math.Max(0, this.Arguments.NewLocation.Z - 2) : 7),
                    (sbyte)(this.Arguments.NewLocation.IsUnderground ? Math.Min(15, this.Arguments.NewLocation.Z + 2) : 0),
                    1,
                    IMap.DefaultWindowSizeY));
        }
    }
}