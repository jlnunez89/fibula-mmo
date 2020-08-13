// -----------------------------------------------------------------
// <copyright file="CreatureMovedNotification.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Notifications
{
    using System;
    using System.Buffers;
    using System.Collections.Generic;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Contracts.Structs;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Contracts.Enumerations;
    using Fibula.Communications.Packets.Outgoing;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Map.Contracts.Abstractions;
    using Fibula.Map.Contracts.Constants;
    using Fibula.Mechanics.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a notification for when a creature has moved.
    /// </summary>
    public class CreatureMovedNotification : Notification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureMovedNotification"/> class.
        /// </summary>
        /// <param name="findTargetPlayers">A function to determine the target players of this notification.</param>
        /// <param name="creatureId">The id of the creature moving.</param>
        /// <param name="fromLocation">The location from which the creature is moving.</param>
        /// <param name="fromStackPos">The stack position from where the creature moving.</param>
        /// <param name="toLocation">The location to which the creature is moving.</param>
        /// <param name="toStackPos">The stack position to which the creature is moving.</param>
        /// <param name="wasTeleport">A value indicating whether this movement was a teleportation.</param>
        public CreatureMovedNotification(
            Func<IEnumerable<IPlayer>> findTargetPlayers,
            uint creatureId,
            Location fromLocation,
            byte fromStackPos,
            Location toLocation,
            byte toStackPos,
            bool wasTeleport)
            : base(findTargetPlayers)
        {
            var locationDiff = fromLocation - toLocation;

            this.CreatureId = creatureId;
            this.OldLocation = fromLocation;
            this.OldStackPosition = fromStackPos;
            this.NewLocation = toLocation;
            this.NewStackPosition = toStackPos;
            this.WasTeleport = wasTeleport || locationDiff.MaxValueIn3D > 1;
        }

        /// <summary>
        /// Gets a value indicating whether this movement was a teleportation.
        /// </summary>
        public bool WasTeleport { get; }

        /// <summary>
        /// Gets the location from which the creature is moving.
        /// </summary>
        public Location OldLocation { get; }

        /// <summary>
        /// Gets the stack position from where the creature moving.
        /// </summary>
        public byte OldStackPosition { get; }

        /// <summary>
        /// Gets the stack position to which the creature is moving.
        /// </summary>
        public byte NewStackPosition { get; }

        /// <summary>
        /// Gets the location to which the creature is moving.
        /// </summary>
        public Location NewLocation { get; }

        /// <summary>
        /// Gets the id of the creature moving.
        /// </summary>
        public uint CreatureId { get; }

        /// <summary>
        /// Finalizes the notification in preparation to it being sent.
        /// </summary>
        /// <param name="context">The context of this notification.</param>
        /// <param name="player">The player which this notification is being prepared for.</param>
        /// <returns>A collection of <see cref="IOutboundPacket"/>s, the ones to be sent.</returns>
        protected override IEnumerable<IOutboundPacket> Prepare(INotificationContext context, IPlayer player)
        {
            var packets = new List<IOutboundPacket>();
            var creature = context.CreatureFinder.FindCreatureById(this.CreatureId);

            var allCreatureIdsToLearn = new List<uint>();
            var allCreatureIdsToForget = new List<uint>();

            if (this.CreatureId == player.Id)
            {
                if (this.WasTeleport)
                {
                    if (this.OldStackPosition <= MapConstants.MaximumNumberOfThingsToDescribePerTile)
                    {
                        // Since this was described to the client before, we send a packet that lets them know the thing must be removed from that Tile's stack.
                        packets.Add(new RemoveAtLocationPacket(this.OldLocation, this.OldStackPosition));
                    }

                    // Then send the entire description at the new location.
                    var (descriptionMetadata, descriptionBytes) = context.MapDescriptor.DescribeAt(player, this.NewLocation);

                    packets.Add(new MapDescriptionPacket(this.NewLocation, descriptionBytes));

                    return packets;
                }

                if (this.OldLocation.Z == 7 && this.NewLocation.Z > 7)
                {
                    if (this.OldStackPosition <= MapConstants.MaximumNumberOfThingsToDescribePerTile)
                    {
                        packets.Add(new RemoveAtLocationPacket(this.OldLocation, this.OldStackPosition));
                    }
                }
                else
                {
                    packets.Add(new CreatureMovedPacket(this.OldLocation, this.OldStackPosition, this.NewLocation));
                }

                // floor change down
                if (this.NewLocation.Z > this.OldLocation.Z)
                {
                    var windowStartLocation = new Location()
                    {
                        X = this.OldLocation.X - ((MapConstants.DefaultWindowSizeX / 2) - 1), // -8
                        Y = this.OldLocation.Y - ((MapConstants.DefaultWindowSizeY / 2) - 1), // -6
                        Z = this.NewLocation.Z,
                    };

                    (IDictionary<string, object> Metadata, ReadOnlySequence<byte> Data) description;

                    // going from surface to underground
                    if (this.NewLocation.Z == 8)
                    {
                        // Client already has the two floors above (6 and 7), so it needs 8 (new current), and 2 below.
                        description = context.MapDescriptor.DescribeWindow(
                            player,
                            (ushort)windowStartLocation.X,
                            (ushort)windowStartLocation.Y,
                            this.NewLocation.Z,
                            (sbyte)(this.NewLocation.Z + 2),
                            MapConstants.DefaultWindowSizeX,
                            MapConstants.DefaultWindowSizeY,
                            -1);
                    }

                    // going further down underground; watch for world's deepest floor (hardcoded for now).
                    else if (this.NewLocation.Z > 8 && this.NewLocation.Z < 14)
                    {
                        // Client already has all floors needed except the new deepest floor, so it needs the 2th floor below the current.
                        description = context.MapDescriptor.DescribeWindow(
                            player,
                            (ushort)windowStartLocation.X,
                            (ushort)windowStartLocation.Y,
                            (sbyte)(this.NewLocation.Z + 2),
                            (sbyte)(this.NewLocation.Z + 2),
                            MapConstants.DefaultWindowSizeX,
                            MapConstants.DefaultWindowSizeY,
                            -3);
                    }

                    // going down but still above surface, so client has all floors.
                    else
                    {
                        description = (new Dictionary<string, object>(), ReadOnlySequence<byte>.Empty);
                    }

                    packets.Add(new MapPartialDescriptionPacket(OutgoingPacketType.FloorChangeDown, description.Data));

                    // moving down a floor makes us out of sync, include east and south
                    var (eastDescriptionMetadata, eastDescriptionBytes) = this.EastSliceDescription(
                            context,
                            player,
                            this.OldLocation.X - this.NewLocation.X,
                            this.OldLocation.Y - this.NewLocation.Y + this.OldLocation.Z - this.NewLocation.Z);

                    if (eastDescriptionMetadata.TryGetValue(IMapDescriptor.CreatureIdsToLearnMetadataKeyName, out object eastCreatureIdsToLearnBoxed) &&
                        eastDescriptionMetadata.TryGetValue(IMapDescriptor.CreatureIdsToForgetMetadataKeyName, out object eastCreatureIdsToForgetBoxed) &&
                        eastCreatureIdsToLearnBoxed is IEnumerable<uint> eastCreatureIdsToLearn && eastCreatureIdsToForgetBoxed is IEnumerable<uint> eastCreatureIdsToForget)
                    {
                        allCreatureIdsToLearn.AddRange(eastCreatureIdsToLearn);
                        allCreatureIdsToForget.AddRange(eastCreatureIdsToForget);
                    }

                    packets.Add(new MapPartialDescriptionPacket(OutgoingPacketType.MapSliceEast, eastDescriptionBytes));

                    var (southDescriptionMetadata, southDescriptionBytes) = this.SouthSliceDescription(context, player, this.OldLocation.Y - this.NewLocation.Y);

                    if (southDescriptionMetadata.TryGetValue(IMapDescriptor.CreatureIdsToLearnMetadataKeyName, out object southCreatureIdsToLearnBoxed) &&
                        southDescriptionMetadata.TryGetValue(IMapDescriptor.CreatureIdsToForgetMetadataKeyName, out object southCreatureIdsToForgetBoxed) &&
                        southCreatureIdsToLearnBoxed is IEnumerable<uint> southCreatureIdsToLearn && southCreatureIdsToForgetBoxed is IEnumerable<uint> southCreatureIdsToForget)
                    {
                        allCreatureIdsToLearn.AddRange(southCreatureIdsToLearn);
                        allCreatureIdsToForget.AddRange(southCreatureIdsToForget);
                    }

                    packets.Add(new MapPartialDescriptionPacket(OutgoingPacketType.MapSliceSouth, southDescriptionBytes));
                }

                // floor change up
                else if (this.NewLocation.Z < this.OldLocation.Z)
                {
                    var windowStartLocation = new Location()
                    {
                        X = this.OldLocation.X - ((MapConstants.DefaultWindowSizeX / 2) - 1), // -8
                        Y = this.OldLocation.Y - ((MapConstants.DefaultWindowSizeY / 2) - 1), // -6
                        Z = this.NewLocation.Z,
                    };

                    (IDictionary<string, object> Metadata, ReadOnlySequence<byte> Data) description;

                    // going to surface
                    if (this.NewLocation.Z == 7)
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
                    else if (this.NewLocation.Z > 7)
                    {
                        // Client already has all floors needed except the new highest floor, so it needs the 2th floor above the current.
                        description = context.MapDescriptor.DescribeWindow(
                            player,
                            (ushort)windowStartLocation.X,
                            (ushort)windowStartLocation.Y,
                            (sbyte)(this.NewLocation.Z - 2),
                            (sbyte)(this.NewLocation.Z - 2),
                            MapConstants.DefaultWindowSizeX,
                            MapConstants.DefaultWindowSizeY,
                            3);
                    }

                    // already above surface, so client has all floors.
                    else
                    {
                        description = (new Dictionary<string, object>(), ReadOnlySequence<byte>.Empty);
                    }

                    packets.Add(new MapPartialDescriptionPacket(OutgoingPacketType.FloorChangeUp, description.Data));

                    // moving up a floor up makes us out of sync, include west and north
                    var (westDescriptionMetadata, westDescriptionBytes) = this.WestSliceDescription(
                            context,
                            player,
                            this.OldLocation.X - this.NewLocation.X,
                            this.OldLocation.Y - this.NewLocation.Y + this.OldLocation.Z - this.NewLocation.Z);

                    if (westDescriptionMetadata.TryGetValue(IMapDescriptor.CreatureIdsToLearnMetadataKeyName, out object westCreatureIdsToLearnBoxed) &&
                        westDescriptionMetadata.TryGetValue(IMapDescriptor.CreatureIdsToForgetMetadataKeyName, out object westCreatureIdsToForgetBoxed) &&
                        westCreatureIdsToLearnBoxed is IEnumerable<uint> westCreatureIdsToLearn && westCreatureIdsToForgetBoxed is IEnumerable<uint> westCreatureIdsToForget)
                    {
                        allCreatureIdsToLearn.AddRange(westCreatureIdsToLearn);
                        allCreatureIdsToForget.AddRange(westCreatureIdsToForget);
                    }

                    packets.Add(new MapPartialDescriptionPacket(OutgoingPacketType.MapSliceWest, westDescriptionBytes));

                    var (northDescriptionMetadata, northDescriptionBytes) = this.NorthSliceDescription(context, player, this.OldLocation.Y - this.NewLocation.Y);

                    if (northDescriptionMetadata.TryGetValue(IMapDescriptor.CreatureIdsToLearnMetadataKeyName, out object northCreatureIdsToLearnBoxed) &&
                        northDescriptionMetadata.TryGetValue(IMapDescriptor.CreatureIdsToForgetMetadataKeyName, out object northCreatureIdsToForgetBoxed) &&
                        northCreatureIdsToLearnBoxed is IEnumerable<uint> northCreatureIdsToLearn && northCreatureIdsToForgetBoxed is IEnumerable<uint> northCreatureIdsToForget)
                    {
                        allCreatureIdsToLearn.AddRange(northCreatureIdsToLearn);
                        allCreatureIdsToForget.AddRange(northCreatureIdsToForget);
                    }

                    packets.Add(new MapPartialDescriptionPacket(OutgoingPacketType.MapSliceNorth, northDescriptionBytes));
                }

                if (this.OldLocation.Y > this.NewLocation.Y)
                {
                    // Creature is moving north, so we need to send the additional north bytes.
                    var (northDescriptionMetadata, northDescriptionBytes) = this.NorthSliceDescription(context, player);

                    if (northDescriptionMetadata.TryGetValue(IMapDescriptor.CreatureIdsToLearnMetadataKeyName, out object northCreatureIdsToLearnBoxed) &&
                        northDescriptionMetadata.TryGetValue(IMapDescriptor.CreatureIdsToForgetMetadataKeyName, out object northCreatureIdsToForgetBoxed) &&
                        northCreatureIdsToLearnBoxed is IEnumerable<uint> northCreatureIdsToLearn && northCreatureIdsToForgetBoxed is IEnumerable<uint> northCreatureIdsToForget)
                    {
                        allCreatureIdsToLearn.AddRange(northCreatureIdsToLearn);
                        allCreatureIdsToForget.AddRange(northCreatureIdsToForget);
                    }

                    packets.Add(new MapPartialDescriptionPacket(OutgoingPacketType.MapSliceNorth, northDescriptionBytes));
                }
                else if (this.OldLocation.Y < this.NewLocation.Y)
                {
                    // Creature is moving south, so we need to send the additional south bytes.
                    var (southDescriptionMetadata, southDescriptionBytes) = this.SouthSliceDescription(context, player);

                    if (southDescriptionMetadata.TryGetValue(IMapDescriptor.CreatureIdsToLearnMetadataKeyName, out object southCreatureIdsToLearnBoxed) &&
                        southDescriptionMetadata.TryGetValue(IMapDescriptor.CreatureIdsToForgetMetadataKeyName, out object southCreatureIdsToForgetBoxed) &&
                        southCreatureIdsToLearnBoxed is IEnumerable<uint> southCreatureIdsToLearn && southCreatureIdsToForgetBoxed is IEnumerable<uint> southCreatureIdsToForget)
                    {
                        allCreatureIdsToLearn.AddRange(southCreatureIdsToLearn);
                        allCreatureIdsToForget.AddRange(southCreatureIdsToForget);
                    }

                    packets.Add(new MapPartialDescriptionPacket(OutgoingPacketType.MapSliceSouth, southDescriptionBytes));
                }

                if (this.OldLocation.X < this.NewLocation.X)
                {
                    // Creature is moving east, so we need to send the additional east bytes.
                    var (eastDescriptionMetadata, eastDescriptionBytes) = this.EastSliceDescription(context, player);

                    if (eastDescriptionMetadata.TryGetValue(IMapDescriptor.CreatureIdsToLearnMetadataKeyName, out object eastCreatureIdsToLearnBoxed) &&
                        eastDescriptionMetadata.TryGetValue(IMapDescriptor.CreatureIdsToForgetMetadataKeyName, out object eastCreatureIdsToForgetBoxed) &&
                        eastCreatureIdsToLearnBoxed is IEnumerable<uint> eastCreatureIdsToLearn && eastCreatureIdsToForgetBoxed is IEnumerable<uint> eastCreatureIdsToForget)
                    {
                        allCreatureIdsToLearn.AddRange(eastCreatureIdsToLearn);
                        allCreatureIdsToForget.AddRange(eastCreatureIdsToForget);
                    }

                    packets.Add(new MapPartialDescriptionPacket(OutgoingPacketType.MapSliceEast, eastDescriptionBytes));
                }
                else if (this.OldLocation.X > this.NewLocation.X)
                {
                    // Creature is moving west, so we need to send the additional west bytes.
                    var (westDescriptionMetadata, westDescriptionBytes) = this.WestSliceDescription(context, player);

                    if (westDescriptionMetadata.TryGetValue(IMapDescriptor.CreatureIdsToLearnMetadataKeyName, out object westCreatureIdsToLearnBoxed) &&
                        westDescriptionMetadata.TryGetValue(IMapDescriptor.CreatureIdsToForgetMetadataKeyName, out object westCreatureIdsToForgetBoxed) &&
                        westCreatureIdsToLearnBoxed is IEnumerable<uint> westCreatureIdsToLearn && westCreatureIdsToForgetBoxed is IEnumerable<uint> westCreatureIdsToForget)
                    {
                        allCreatureIdsToLearn.AddRange(westCreatureIdsToLearn);
                        allCreatureIdsToForget.AddRange(westCreatureIdsToForget);
                    }

                    packets.Add(new MapPartialDescriptionPacket(OutgoingPacketType.MapSliceWest, westDescriptionBytes));
                }
            }
            else if (player.CanSee(this.OldLocation) && player.CanSee(this.NewLocation))
            {
                if (player.CanSee(creature))
                {
                    if (this.WasTeleport || (this.OldLocation.Z == 7 && this.NewLocation.Z > 7) || this.OldStackPosition > 9)
                    {
                        if (this.OldStackPosition <= MapConstants.MaximumNumberOfThingsToDescribePerTile)
                        {
                            packets.Add(new RemoveAtLocationPacket(this.OldLocation, this.OldStackPosition));
                        }

                        var creatureIsKnown = player.Client.KnowsCreatureWithId(this.CreatureId);
                        var creatureIdToForget = player.Client.ChooseCreatureToRemoveFromKnownSet();

                        if (!creatureIsKnown)
                        {
                            allCreatureIdsToLearn.Add(this.CreatureId);
                        }

                        if (creatureIdToForget > uint.MinValue)
                        {
                            allCreatureIdsToForget.Add(creatureIdToForget);
                        }

                        packets.Add(new AddCreaturePacket(creature, creatureIsKnown, creatureIdToForget));
                    }
                    else
                    {
                        packets.Add(new CreatureMovedPacket(this.OldLocation, this.OldStackPosition, this.NewLocation));
                    }
                }
            }
            else if (player.CanSee(this.OldLocation) && !player.CanSee(creature))
            {
                if (this.OldStackPosition <= MapConstants.MaximumNumberOfThingsToDescribePerTile)
                {
                    packets.Add(new RemoveAtLocationPacket(this.OldLocation, this.OldStackPosition));
                }
            }
            else if (player.CanSee(this.NewLocation) && player.CanSee(creature))
            {
                if (this.NewStackPosition <= MapConstants.MaximumNumberOfThingsToDescribePerTile)
                {
                    var creatureIsKnown = player.Client.KnowsCreatureWithId(this.CreatureId);
                    var creatureIdToForget = player.Client.ChooseCreatureToRemoveFromKnownSet();

                    if (!creatureIsKnown)
                    {
                        allCreatureIdsToLearn.Add(this.CreatureId);
                    }

                    if (creatureIdToForget > uint.MinValue)
                    {
                        allCreatureIdsToForget.Add(creatureIdToForget);
                    }

                    packets.Add(new AddCreaturePacket(creature, creatureIsKnown, creatureIdToForget));
                }
            }

            if (this.WasTeleport)
            {
                packets.Add(new MagicEffectPacket(this.NewLocation, AnimatedEffect.BubbleBlue));
            }

            this.Sent += (client) =>
            {
                foreach (var creatureId in allCreatureIdsToLearn)
                {
                    client.AddKnownCreature(creatureId);
                }

                foreach (var creatureId in allCreatureIdsToForget)
                {
                    client.RemoveKnownCreature(creatureId);
                }
            };

            return packets;
        }

        private (IDictionary<string, object> descriptionMetadata, ReadOnlySequence<byte> descriptionData) NorthSliceDescription(INotificationContext notificationContext, IPlayer player, int floorChangeOffset = 0)
        {
            // A = old location, B = new location.
            //
            //       |------ MapConstants.DefaultWindowSizeX = 18 ------|
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
                X = this.OldLocation.X - ((MapConstants.DefaultWindowSizeX / 2) - 1),

                // -6
                Y = this.NewLocation.Y - ((MapConstants.DefaultWindowSizeY / 2) - 1 - floorChangeOffset),

                Z = this.NewLocation.Z,
            };

            return notificationContext.MapDescriptor.DescribeWindow(
                    player,
                    (ushort)windowStartLocation.X,
                    (ushort)windowStartLocation.Y,
                    (sbyte)(this.NewLocation.IsUnderground ? Math.Max(0, this.NewLocation.Z - 2) : 7),
                    (sbyte)(this.NewLocation.IsUnderground ? Math.Min(15, this.NewLocation.Z + 2) : 0),
                    MapConstants.DefaultWindowSizeX,
                    1);
        }

        private (IDictionary<string, object> descriptionMetadata, ReadOnlySequence<byte> descriptionData) SouthSliceDescription(INotificationContext notificationContext, IPlayer player, int floorChangeOffset = 0)
        {
            // A = old location, B = new location
            //
            //       |------ MapConstants.DefaultWindowSizeX = 18 ------|
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
                X = this.OldLocation.X - ((MapConstants.DefaultWindowSizeX / 2) - 1),

                // +7
                Y = this.NewLocation.Y + (MapConstants.DefaultWindowSizeY / 2) + floorChangeOffset,

                Z = this.NewLocation.Z,
            };

            return notificationContext.MapDescriptor.DescribeWindow(
                    player,
                    (ushort)windowStartLocation.X,
                    (ushort)windowStartLocation.Y,
                    (sbyte)(this.NewLocation.IsUnderground ? Math.Max(0, this.NewLocation.Z - 2) : 7),
                    (sbyte)(this.NewLocation.IsUnderground ? Math.Min(15, this.NewLocation.Z + 2) : 0),
                    MapConstants.DefaultWindowSizeX,
                    1);
        }

        private (IDictionary<string, object> descriptionMetadata, ReadOnlySequence<byte> descriptionData) EastSliceDescription(INotificationContext notificationContext, IPlayer player, int floorChangeOffsetX = 0, int floorChangeOffsetY = 0)
        {
            // A = old location, B = new location
            //
            //       |------ MapConstants.DefaultWindowSizeX = 18 ------|
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
                X = this.NewLocation.X + (MapConstants.DefaultWindowSizeX / 2) + floorChangeOffsetX,

                // -6
                Y = this.NewLocation.Y - ((MapConstants.DefaultWindowSizeY / 2) - 1) + floorChangeOffsetY,

                Z = this.NewLocation.Z,
            };

            return notificationContext.MapDescriptor.DescribeWindow(
                    player,
                    (ushort)windowStartLocation.X,
                    (ushort)windowStartLocation.Y,
                    (sbyte)(this.NewLocation.IsUnderground ? Math.Max(0, this.NewLocation.Z - 2) : 7),
                    (sbyte)(this.NewLocation.IsUnderground ? Math.Min(15, this.NewLocation.Z + 2) : 0),
                    1,
                    MapConstants.DefaultWindowSizeY);
        }

        private (IDictionary<string, object> descriptionMetadata, ReadOnlySequence<byte> descriptionData) WestSliceDescription(INotificationContext notificationContext, IPlayer player, int floorChangeOffsetX = 0, int floorChangeOffsetY = 0)
        {
            // A = old location, B = new location
            //
            //          |------ MapConstants.DefaultWindowSizeX = 18 ------|
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
                X = this.NewLocation.X - ((MapConstants.DefaultWindowSizeX / 2) - 1) + floorChangeOffsetX,

                // -6
                Y = this.NewLocation.Y - ((MapConstants.DefaultWindowSizeY / 2) - 1) + floorChangeOffsetY,

                Z = this.NewLocation.Z,
            };

            return notificationContext.MapDescriptor.DescribeWindow(
                    player,
                    (ushort)windowStartLocation.X,
                    (ushort)windowStartLocation.Y,
                    (sbyte)(this.NewLocation.IsUnderground ? Math.Max(0, this.NewLocation.Z - 2) : 7),
                    (sbyte)(this.NewLocation.IsUnderground ? Math.Min(15, this.NewLocation.Z + 2) : 0),
                    1,
                    MapConstants.DefaultWindowSizeY);
        }
    }
}
