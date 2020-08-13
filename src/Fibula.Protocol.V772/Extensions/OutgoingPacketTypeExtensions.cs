// -----------------------------------------------------------------
// <copyright file="OutgoingPacketTypeExtensions.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Protocol.V772.Extensions
{
    using System;
    using Fibula.Communications.Contracts.Enumerations;

    /// <summary>
    /// Static class that contains helper methods to convert <see cref="OutgoingPacketType"/>.
    /// </summary>
    public static class OutgoingPacketTypeExtensions
    {
        /// <summary>
        /// Attempts to convert an <see cref="OutgoingPacketType"/> value into a byte value.
        /// </summary>
        /// <param name="packetType">The packet type to convert.</param>
        /// <returns>The byte value converted to.</returns>
        public static byte ToByte(this OutgoingPacketType packetType)
        {
            return packetType switch
            {
                OutgoingPacketType.GatewayDisconnect => 0x0A,
                OutgoingPacketType.MessageOfTheDay => 0x14,
                OutgoingPacketType.CharacterList => 0x64,
                OutgoingPacketType.PlayerLogin => 0x0A,
                OutgoingPacketType.GamemasterFlags => 0x0B,
                OutgoingPacketType.GameDisconnect => 0x14,
                OutgoingPacketType.WaitingList => 0x16,
                OutgoingPacketType.HeartbeatResponse => 0x1D,
                OutgoingPacketType.Heartbeat => 0x1E,
                OutgoingPacketType.Death => 0x28,
                OutgoingPacketType.AddUnknownCreature => 0x61,
                OutgoingPacketType.AddKnownCreature => 0x62,
                OutgoingPacketType.MapDescription => 0x64,
                OutgoingPacketType.MapSliceNorth => 0x65,
                OutgoingPacketType.MapSliceEast => 0x66,
                OutgoingPacketType.MapSliceSouth => 0x67,
                OutgoingPacketType.MapSliceWest => 0x68,
                OutgoingPacketType.TileUpdate => 0x69,
                OutgoingPacketType.AddThing => 0x6A,
                OutgoingPacketType.UpdateThing => 0x6B,
                OutgoingPacketType.RemoveThing => 0x6C,
                OutgoingPacketType.CreatureMoved => 0x6D,
                OutgoingPacketType.ContainerOpen => 0x6E,
                OutgoingPacketType.ContainerClose => 0x6F,
                OutgoingPacketType.ContainerAddItem => 0x70,
                OutgoingPacketType.ContainerUpdateItem => 0x71,
                OutgoingPacketType.ContainerRemoveItem => 0x72,
                OutgoingPacketType.InventoryItem => 0x78,
                OutgoingPacketType.InventoryEmpty => 0x79,
                OutgoingPacketType.WorldLight => 0x82,
                OutgoingPacketType.MagicEffect => 0x83,
                OutgoingPacketType.AnimatedText => 0x84,
                OutgoingPacketType.ProjectileEffect => 0x85,
                OutgoingPacketType.Square => 0x86,
                OutgoingPacketType.CreatureHealth => 0x8C,
                OutgoingPacketType.CreatureLight => 0x8D,
                OutgoingPacketType.CreatureOutfit => 0x8E,
                OutgoingPacketType.CreatureSpeedChange => 0x8F,
                OutgoingPacketType.CreatureSkull => 0x90,
                OutgoingPacketType.CreatureShield => 0x91,
                OutgoingPacketType.TextWindow => 0x96,
                OutgoingPacketType.HouseWindow => 0x97,
                OutgoingPacketType.PlayerStats => 0xA0,
                OutgoingPacketType.PlayerSkills => 0xA1,
                OutgoingPacketType.PlayerConditions => 0xA2,
                OutgoingPacketType.CancelAttack => 0xA3,
                OutgoingPacketType.PlayerModes => 0xA7,
                OutgoingPacketType.CreatureSpeech => 0xAA,
                OutgoingPacketType.TextMessage => 0xB4,
                OutgoingPacketType.CancelWalk => 0xB5,
                OutgoingPacketType.FloorChangeUp => 0xBE,
                OutgoingPacketType.FloorChangeDown => 0xBF,
                OutgoingPacketType.OutfitWindow => 0xC8,
                OutgoingPacketType.VipDetails => 0xD2,
                OutgoingPacketType.VipOnline => 0xD3,
                OutgoingPacketType.VipOffline => 0xD4,
                _ => throw new NotSupportedException($"Outgoing packet type {packetType} is not supported in this client version.")
            };
        }
    }
}
