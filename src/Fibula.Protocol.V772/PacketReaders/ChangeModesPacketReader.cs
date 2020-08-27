// -----------------------------------------------------------------
// <copyright file="ChangeModesPacketReader.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Protocol.V772.PacketReaders
{
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Utilities;
    using Fibula.Communications;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Packets.Incoming;
    using Serilog;

    /// <summary>
    /// Class that represents a change modes packet reader for the game server.
    /// </summary>
    public sealed class ChangeModesPacketReader : BasePacketReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeModesPacketReader"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        public ChangeModesPacketReader(ILogger logger)
            : base(logger)
        {
        }

        /// <summary>
        /// Reads a packet from the given <see cref="INetworkMessage"/>.
        /// </summary>
        /// <param name="message">The message to read from.</param>
        /// <returns>The packet read from the message.</returns>
        public override IIncomingPacket ReadFromMessage(INetworkMessage message)
        {
            message.ThrowIfNull(nameof(message));

            // 1 - offensive, 2 - balanced, 3 - defensive
            FightMode fightMode = message.GetByte() switch
            {
                0x01 => FightMode.FullAttack,
                0x02 => FightMode.Balanced,
                0x03 => FightMode.FullDefense,
                _ => FightMode.Balanced,
            };

            // 0 - stand while fightning, 1 - chase opponent
            ChaseMode chaseMode = message.GetByte() switch
            {
                0x00 => ChaseMode.Stand,
                0x01 => ChaseMode.Chase,
                0x02 => ChaseMode.KeepDistance,
                _ => ChaseMode.Chase,
            };

            // 0 - safe mode, 1 - free mode
            var isSafetyEnabled = message.GetByte() > 0;

            return new ModesPacket(fightMode, chaseMode, isSafetyEnabled);
        }
    }
}
