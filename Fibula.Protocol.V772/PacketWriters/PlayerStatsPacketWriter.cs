// -----------------------------------------------------------------
// <copyright file="PlayerStatsPacketWriter.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Protocol.V772.PacketWriters
{
    using System;
    using Fibula.Communications;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Packets.Outgoing;
    using Serilog;

    /// <summary>
    /// Class that represents a player stats packet writer for the game server.
    /// </summary>
    public class PlayerStatsPacketWriter : BasePacketWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerStatsPacketWriter"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        public PlayerStatsPacketWriter(ILogger logger)
            : base(logger)
        {
        }

        /// <summary>
        /// Writes a packet to the given <see cref="INetworkMessage"/>.
        /// </summary>
        /// <param name="packet">The packet to write.</param>
        /// <param name="message">The message to write into.</param>
        public override void WriteToMessage(IOutboundPacket packet, ref INetworkMessage message)
        {
            if (!(packet is PlayerStatsPacket playerStatsPacket))
            {
                this.Logger.Warning($"Invalid packet {packet.GetType().Name} routed to {this.GetType().Name}");

                return;
            }

            ushort hitpoints = Math.Min(ushort.MaxValue, playerStatsPacket.Player.Hitpoints);
            ushort maxHitpoints = Math.Min(ushort.MaxValue, playerStatsPacket.Player.MaxHitpoints);
            ushort manapoints = Math.Min(ushort.MaxValue, playerStatsPacket.Player.Manapoints);
            ushort maxManapoints = Math.Min(ushort.MaxValue, playerStatsPacket.Player.MaxManapoints);

            ushort capacity = Convert.ToUInt16(Math.Min(ushort.MaxValue, playerStatsPacket.Player.CarryStrength * 100));

            // Experience: 7.7x Client debugs after 0x7FFFFFFF (2,147,483,647) exp
            uint experience = 0; // Math.Min(0x7FFFFFFF, Convert.ToUInt32(playerStatsPacket.Player.Skills[SkillType.Experience].Count));
            ushort expLevel = 1; // (ushort)Math.Min(1, Math.Min(ushort.MaxValue, playerStatsPacket.Player.Skills[SkillType.Experience].Level));
            byte expPercentage = 0; // playerStatsPacket.Player.CalculateSkillPercent(SkillType.Experience);

            byte magicLevel = 0; // (byte)Math.Min(byte.MaxValue, playerStatsPacket.Player.Skills[SkillType.Magic].Level);
            byte magicLevelPercentage = 0; // playerStatsPacket.Player.CalculateSkillPercent(SkillType.Magic);

            message.AddByte(playerStatsPacket.PacketType);

            message.AddUInt16(hitpoints);
            message.AddUInt16(maxHitpoints);
            message.AddUInt16(capacity);

            message.AddUInt32(experience);

            message.AddUInt16(expLevel);
            message.AddByte(expPercentage);
            message.AddUInt16(manapoints);
            message.AddUInt16(maxManapoints);
            message.AddByte(magicLevel);
            message.AddByte(magicLevelPercentage);

            message.AddByte(playerStatsPacket.Player.SoulPoints);
        }
    }
}
