// -----------------------------------------------------------------
// <copyright file="CreatureMovedNotification.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Notifications
{
    using System;
    using System.Buffers;
    using System.Collections.Generic;
    using Fibula.Common.Utilities;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Contracts.Enumerations;
    using Fibula.Communications.Packets.Outgoing;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Server.Contracts.Structs;
    using Fibula.Server.Map.Contracts.Constants;
    using Fibula.Server.Mechanics.Contracts.Enumerations;
    using Fibula.Server.Notifications.Arguments;
    using Fibula.Server.Notifications.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a notification for when a creature has moved.
    /// </summary>
    public class CreatureMovedNotification : Notification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureMovedNotification"/> class.
        /// </summary>
        /// <param name="findTargetPlayers">A function to determine the target players of this notification.</param>
        /// <param name="arguments">The arguments for this notification.</param>
        public CreatureMovedNotification(Func<IEnumerable<IPlayer>> findTargetPlayers, CreatureMovedNotificationArguments arguments)
            : base(findTargetPlayers)
        {
            arguments.ThrowIfNull(nameof(arguments));

            this.Arguments = arguments;
        }

        /// <summary>
        /// Gets this notification's arguments.
        /// </summary>
        public CreatureMovedNotificationArguments Arguments { get; }

        /// <summary>
        /// Finalizes the notification in preparation to it being sent.
        /// </summary>
        /// <param name="context">The context of this notification.</param>
        /// <param name="player">The player which this notification is being prepared for.</param>
        /// <returns>A collection of <see cref="IOutboundPacket"/>s, the ones to be sent.</returns>
        protected override IEnumerable<IOutboundPacket> Prepare(INotificationContext context, IPlayer player)
        {
            var packets = new List<IOutboundPacket>();
            var creature = context.CreatureFinder.FindCreatureById(this.Arguments.CreatureId);

            if (this.Arguments.CreatureId == player.Id)
            {
                if (this.Arguments.WasTeleport)
                {
                    if (this.Arguments.OldStackPosition <= MapConstants.MaximumNumberOfThingsToDescribePerTile)
                    {
                        // Since this was described to the client before, we send a packet that lets them know the thing must be removed from that Tile's stack.
                        packets.Add(new RemoveAtPositionPacket(this.Arguments.OldLocation, this.Arguments.OldStackPosition));
                    }

                    // Then send the entire description at the new location.
                    packets.Add(new MapDescriptionPacket(this.Arguments.NewLocation, context.MapDescriptor.DescribeAt(player, this.Arguments.NewLocation)));

                    return packets;
                }

                if (this.Arguments.OldLocation.Z == 7 && this.Arguments.NewLocation.Z > 7)
                {
                    if (this.Arguments.OldStackPosition <= MapConstants.MaximumNumberOfThingsToDescribePerTile)
                    {
                        packets.Add(new RemoveAtPositionPacket(this.Arguments.OldLocation, this.Arguments.OldStackPosition));
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
                        X = this.Arguments.OldLocation.X - ((MapConstants.DefaultWindowSizeX / 2) - 1), // -8
                        Y = this.Arguments.OldLocation.Y - ((MapConstants.DefaultWindowSizeY / 2) - 1), // -6
                        Z = this.Arguments.NewLocation.Z,
                    };

                    ReadOnlySequence<byte> description;

                    // going from surface to underground
                    if (this.Arguments.NewLocation.Z == 8)
                    {
                        // Client already has the two floors above (6 and 7), so it needs 8 (new current), and 2 below.
                        description = context.MapDescriptor.DescribeWindow(
                            player,
                            (ushort)windowStartLocation.X,
                            (ushort)windowStartLocation.Y,
                            this.Arguments.NewLocation.Z,
                            (sbyte)(this.Arguments.NewLocation.Z + 2),
                            MapConstants.DefaultWindowSizeX,
                            MapConstants.DefaultWindowSizeY,
                            -1);
                    }

                    // going further down underground; watch for world's deepest floor (hardcoded for now).
                    else if (this.Arguments.NewLocation.Z > 8 && this.Arguments.NewLocation.Z < 14)
                    {
                        // Client already has all floors needed except the new deepest floor, so it needs the 2th floor below the current.
                        description = context.MapDescriptor.DescribeWindow(
                            player,
                            (ushort)windowStartLocation.X,
                            (ushort)windowStartLocation.Y,
                            (sbyte)(this.Arguments.NewLocation.Z + 2),
                            (sbyte)(this.Arguments.NewLocation.Z + 2),
                            MapConstants.DefaultWindowSizeX,
                            MapConstants.DefaultWindowSizeY,
                            -3);
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
                            context,
                            player,
                            this.Arguments.OldLocation.X - this.Arguments.NewLocation.X,
                            this.Arguments.OldLocation.Y - this.Arguments.NewLocation.Y + this.Arguments.OldLocation.Z - this.Arguments.NewLocation.Z));

                    packets.Add(this.SouthSliceDescription(context, player, this.Arguments.OldLocation.Y - this.Arguments.NewLocation.Y));
                }

                // floor change up
                else if (this.Arguments.NewLocation.Z < this.Arguments.OldLocation.Z)
                {
                    var windowStartLocation = new Location()
                    {
                        X = this.Arguments.OldLocation.X - ((MapConstants.DefaultWindowSizeX / 2) - 1), // -8
                        Y = this.Arguments.OldLocation.Y - ((MapConstants.DefaultWindowSizeY / 2) - 1), // -6
                        Z = this.Arguments.NewLocation.Z,
                    };

                    ReadOnlySequence<byte> description;

                    // going to surface
                    if (this.Arguments.NewLocation.Z == 7)
                    {
                        // Client already has the first two above-the-ground floors (6 and 7), so it needs 0-5 above.
                        description = context.MapDescriptor.DescribeWindow(
                            player,
                            (ushort)windowStartLocation.X,
                            (ushort)windowStartLocation.Y,
                            5,
                            0,
                            MapConstants.DefaultWindowSizeX,
                            MapConstants.DefaultWindowSizeY,
                            3);
                    }

                    // going up but still underground
                    else if (this.Arguments.NewLocation.Z > 7)
                    {
                        // Client already has all floors needed except the new highest floor, so it needs the 2th floor above the current.
                        description = context.MapDescriptor.DescribeWindow(
                            player,
                            (ushort)windowStartLocation.X,
                            (ushort)windowStartLocation.Y,
                            (sbyte)(this.Arguments.NewLocation.Z - 2),
                            (sbyte)(this.Arguments.NewLocation.Z - 2),
                            MapConstants.DefaultWindowSizeX,
                            MapConstants.DefaultWindowSizeY,
                            3);
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
                            context,
                            player,
                            this.Arguments.OldLocation.X - this.Arguments.NewLocation.X,
                            this.Arguments.OldLocation.Y - this.Arguments.NewLocation.Y + this.Arguments.OldLocation.Z - this.Arguments.NewLocation.Z));

                    packets.Add(this.NorthSliceDescription(context, player, this.Arguments.OldLocation.Y - this.Arguments.NewLocation.Y));
                }

                if (this.Arguments.OldLocation.Y > this.Arguments.NewLocation.Y)
                {
                    // Creature is moving north, so we need to send the additional north bytes.
                    packets.Add(this.NorthSliceDescription(context, player));
                }
                else if (this.Arguments.OldLocation.Y < this.Arguments.NewLocation.Y)
                {
                    // Creature is moving south, so we need to send the additional south bytes.
                    packets.Add(this.SouthSliceDescription(context, player));
                }

                if (this.Arguments.OldLocation.X < this.Arguments.NewLocation.X)
                {
                    // Creature is moving east, so we need to send the additional east bytes.
                    packets.Add(this.EastSliceDescription(context, player));
                }
                else if (this.Arguments.OldLocation.X > this.Arguments.NewLocation.X)
                {
                    // Creature is moving west, so we need to send the additional west bytes.
                    packets.Add(this.WestSliceDescription(context, player));
                }
            }
            else if (player.CanSee(this.Arguments.OldLocation) && player.CanSee(this.Arguments.NewLocation))
            {
                if (player.CanSee(creature))
                {
                    if (this.Arguments.WasTeleport || (this.Arguments.OldLocation.Z == 7 && this.Arguments.NewLocation.Z > 7) || this.Arguments.OldStackPosition > 9)
                    {
                        if (this.Arguments.OldStackPosition <= MapConstants.MaximumNumberOfThingsToDescribePerTile)
                        {
                            packets.Add(new RemoveAtPositionPacket(this.Arguments.OldLocation, this.Arguments.OldStackPosition));
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
                if (this.Arguments.OldStackPosition <= MapConstants.MaximumNumberOfThingsToDescribePerTile)
                {
                    packets.Add(new RemoveAtPositionPacket(this.Arguments.OldLocation, this.Arguments.OldStackPosition));
                }
            }
            else if (player.CanSee(this.Arguments.NewLocation) && player.CanSee(creature))
            {
                if (this.Arguments.NewStackPosition <= MapConstants.MaximumNumberOfThingsToDescribePerTile)
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

        private IOutboundPacket NorthSliceDescription(INotificationContext notificationContext, IPlayer player, int floorChangeOffset = 0)
        {
            // A = old location, B = new location.
            //
            //       |---------- MapConstants.DefaultWindowSizeX = 18 ----------|
            //                           as seen by A
            //       x  ~  ~  ~  ~  ~  ~  ~  ~  ~  ~  ~  ~  ~  ~  ~  ~  ~
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .     ---
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .      |
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .      |
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .      |
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .      |
            //       .  .  .  .  .  .  .  .  B  .  .  .  .  .  .  .  .  .      |
            //       .  .  .  .  .  .  .  .  A  .  .  .  .  .  .  .  .  .      | MapConstants.DefaultWindowSizeY = 14
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
                X = this.Arguments.OldLocation.X - ((MapConstants.DefaultWindowSizeX / 2) - 1),

                // -6
                Y = this.Arguments.NewLocation.Y - ((MapConstants.DefaultWindowSizeY / 2) - 1 - floorChangeOffset),

                Z = this.Arguments.NewLocation.Z,
            };

            return new MapPartialDescriptionPacket(
                OutgoingGamePacketType.MapSliceNorth,
                notificationContext.MapDescriptor.DescribeWindow(
                    player,
                    (ushort)windowStartLocation.X,
                    (ushort)windowStartLocation.Y,
                    (sbyte)(this.Arguments.NewLocation.IsUnderground ? Math.Max(0, this.Arguments.NewLocation.Z - 2) : 7),
                    (sbyte)(this.Arguments.NewLocation.IsUnderground ? Math.Min(15, this.Arguments.NewLocation.Z + 2) : 0),
                    MapConstants.DefaultWindowSizeX,
                    1));
        }

        private IOutboundPacket SouthSliceDescription(INotificationContext notificationContext, IPlayer player, int floorChangeOffset = 0)
        {
            // A = old location, B = new location
            //
            //       |---------- MapConstants.DefaultWindowSizeX = 18 ----------|
            //                           as seen by A
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .     ---
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .      |
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .      |
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .      |
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .      |
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .      |
            //       .  .  .  .  .  .  .  .  A  .  .  .  .  .  .  .  .  .      | MapConstants.DefaultWindowSizeY = 14
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
                X = this.Arguments.OldLocation.X - ((MapConstants.DefaultWindowSizeX / 2) - 1),

                // +7
                Y = this.Arguments.NewLocation.Y + (MapConstants.DefaultWindowSizeY / 2) + floorChangeOffset,

                Z = this.Arguments.NewLocation.Z,
            };

            return new MapPartialDescriptionPacket(
                OutgoingGamePacketType.MapSliceSouth,
                notificationContext.MapDescriptor.DescribeWindow(
                    player,
                    (ushort)windowStartLocation.X,
                    (ushort)windowStartLocation.Y,
                    (sbyte)(this.Arguments.NewLocation.IsUnderground ? Math.Max(0, this.Arguments.NewLocation.Z - 2) : 7),
                    (sbyte)(this.Arguments.NewLocation.IsUnderground ? Math.Min(15, this.Arguments.NewLocation.Z + 2) : 0),
                    MapConstants.DefaultWindowSizeX,
                    1));
        }

        private IOutboundPacket EastSliceDescription(INotificationContext notificationContext, IPlayer player, int floorChangeOffsetX = 0, int floorChangeOffsetY = 0)
        {
            // A = old location, B = new location
            //
            //       |---------- MapConstants.DefaultWindowSizeX = 18 ----------|
            //                           as seen by A
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  x  ---
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  ~   |
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  ~   |
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  ~   |
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  ~   |
            //       .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  ~   |
            //       .  .  .  .  .  .  .  .  A  B  .  .  .  .  .  .  .  .  ~   | MapConstants.DefaultWindowSizeY = 14
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
                X = this.Arguments.NewLocation.X + (MapConstants.DefaultWindowSizeX / 2) + floorChangeOffsetX,

                // -6
                Y = this.Arguments.NewLocation.Y - ((MapConstants.DefaultWindowSizeY / 2) - 1) + floorChangeOffsetY,

                Z = this.Arguments.NewLocation.Z,
            };

            return new MapPartialDescriptionPacket(
                OutgoingGamePacketType.MapSliceEast,
                notificationContext.MapDescriptor.DescribeWindow(
                    player,
                    (ushort)windowStartLocation.X,
                    (ushort)windowStartLocation.Y,
                    (sbyte)(this.Arguments.NewLocation.IsUnderground ? Math.Max(0, this.Arguments.NewLocation.Z - 2) : 7),
                    (sbyte)(this.Arguments.NewLocation.IsUnderground ? Math.Min(15, this.Arguments.NewLocation.Z + 2) : 0),
                    1,
                    MapConstants.DefaultWindowSizeY));
        }

        private IOutboundPacket WestSliceDescription(INotificationContext notificationContext, IPlayer player, int floorChangeOffsetX = 0, int floorChangeOffsetY = 0)
        {
            // A = old location, B = new location
            //
            //          |---------- MapConstants.DefaultWindowSizeX = 18 ----------|
            //                           as seen by A
            //       x  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  ---
            //       ~  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .   |
            //       ~  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .   |
            //       ~  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .   |
            //       ~  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .   |
            //       ~  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .  .   |
            //       ~  .  .  .  .  .  .  .  B  A  .  .  .  .  .  .  .  .  .   | MapConstants.DefaultWindowSizeY = 14
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
                X = this.Arguments.NewLocation.X - ((MapConstants.DefaultWindowSizeX / 2) - 1) + floorChangeOffsetX,

                // -6
                Y = this.Arguments.NewLocation.Y - ((MapConstants.DefaultWindowSizeY / 2) - 1) + floorChangeOffsetY,

                Z = this.Arguments.NewLocation.Z,
            };

            return new MapPartialDescriptionPacket(
                OutgoingGamePacketType.MapSliceWest,
                notificationContext.MapDescriptor.DescribeWindow(
                    player,
                    (ushort)windowStartLocation.X,
                    (ushort)windowStartLocation.Y,
                    (sbyte)(this.Arguments.NewLocation.IsUnderground ? Math.Max(0, this.Arguments.NewLocation.Z - 2) : 7),
                    (sbyte)(this.Arguments.NewLocation.IsUnderground ? Math.Min(15, this.Arguments.NewLocation.Z + 2) : 0),
                    1,
                    MapConstants.DefaultWindowSizeY));
        }
    }
}