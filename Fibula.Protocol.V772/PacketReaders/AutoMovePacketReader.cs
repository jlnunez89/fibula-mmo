// -----------------------------------------------------------------
// <copyright file="AutoMovePacketReader.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Protocol.V772.PacketReaders
{
    using System.IO;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Utilities;
    using Fibula.Communications;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Packets.Incoming;
    using Serilog;

    /// <summary>
    /// Class that represents an auto walk packet reader for the game server.
    /// </summary>
    public sealed class AutoMovePacketReader : BasePacketReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMovePacketReader"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        public AutoMovePacketReader(ILogger logger)
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

            var numberOfMovements = message.GetByte();

            var directions = new Direction[numberOfMovements];

            for (var i = 0; i < numberOfMovements; i++)
            {
                var dir = message.GetByte();

                directions[i] = dir switch
                {
                    1 => Direction.East,
                    2 => Direction.NorthEast,
                    3 => Direction.North,
                    4 => Direction.NorthWest,
                    5 => Direction.West,
                    6 => Direction.SouthWest,
                    7 => Direction.South,
                    8 => Direction.SouthEast,
                    _ => throw new InvalidDataException($"Invalid direction value '{dir}' on message."),
                };
            }

            return new AutoMovePacket(directions);
        }
    }
}