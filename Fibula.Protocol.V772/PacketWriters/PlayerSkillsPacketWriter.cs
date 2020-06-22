// -----------------------------------------------------------------
// <copyright file="PlayerSkillsPacketWriter.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Protocol.V772.PacketWriters
{
    using Fibula.Communications;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Packets.Outgoing;
    using Serilog;

    /// <summary>
    /// Class that represents a player skills packet writer for the game server.
    /// </summary>
    public class PlayerSkillsPacketWriter : BasePacketWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerSkillsPacketWriter"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        public PlayerSkillsPacketWriter(ILogger logger)
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
            if (!(packet is PlayerSkillsPacket playerSkillsPacket))
            {
                this.Logger.Warning($"Invalid packet {packet.GetType().Name} routed to {this.GetType().Name}");

                return;
            }

            message.AddByte(playerSkillsPacket.PacketType);

            // NoWeapon
            message.AddByte(10);
            message.AddByte(0);

            // Club
            message.AddByte(10);
            message.AddByte(0);

            // Sword
            message.AddByte(10);
            message.AddByte(0);

            // Axe
            message.AddByte(10);
            message.AddByte(0);

            // Ranged
            message.AddByte(10);
            message.AddByte(0);

            // Shield
            message.AddByte(10);
            message.AddByte(0);

            // Fishing
            message.AddByte(10);
            message.AddByte(0);

            //message.AddByte((byte)Math.Min(byte.MaxValue, playerSkillsPacket.Player.Skills[SkillType.NoWeapon].Level));
            //message.AddByte(playerSkillsPacket.Player.CalculateSkillPercent(SkillType.NoWeapon));

            //message.AddByte((byte)Math.Min(byte.MaxValue, playerSkillsPacket.Player.Skills[SkillType.Club].Level));
            //message.AddByte(playerSkillsPacket.Player.CalculateSkillPercent(SkillType.Club));

            //message.AddByte((byte)Math.Min(byte.MaxValue, playerSkillsPacket.Player.Skills[SkillType.Sword].Level));
            //message.AddByte(playerSkillsPacket.Player.CalculateSkillPercent(SkillType.Sword));

            //message.AddByte((byte)Math.Min(byte.MaxValue, playerSkillsPacket.Player.Skills[SkillType.Axe].Level));
            //message.AddByte(playerSkillsPacket.Player.CalculateSkillPercent(SkillType.Axe));

            //message.AddByte((byte)Math.Min(byte.MaxValue, playerSkillsPacket.Player.Skills[SkillType.Ranged].Level));
            //message.AddByte(playerSkillsPacket.Player.CalculateSkillPercent(SkillType.Ranged));

            //message.AddByte((byte)Math.Min(byte.MaxValue, playerSkillsPacket.Player.Skills[SkillType.Shield].Level));
            //message.AddByte(playerSkillsPacket.Player.CalculateSkillPercent(SkillType.Shield));

            //message.AddByte((byte)Math.Min(byte.MaxValue, playerSkillsPacket.Player.Skills[SkillType.Fishing].Level));
            //message.AddByte(playerSkillsPacket.Player.CalculateSkillPercent(SkillType.Fishing));
        }
    }
}
