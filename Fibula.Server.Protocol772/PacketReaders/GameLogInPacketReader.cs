// -----------------------------------------------------------------
// <copyright file="GameLogInPacketReader.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Protocol772.PacketReaders
{
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Common.Utilities;
    using Fibula.Communications;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Packets.Incoming;
    using Serilog;

    /// <summary>
    /// Class that represents a log in packet reader for the game server.
    /// </summary>
    public sealed class GameLogInPacketReader : BasePacketReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameLogInPacketReader"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="applicationContext">A reference to the application context.</param>
        public GameLogInPacketReader(ILogger logger, IApplicationContext applicationContext)
            : base(logger)
        {
            applicationContext.ThrowIfNull(nameof(applicationContext));

            this.ApplicationContext = applicationContext;
        }

        /// <summary>
        /// Gets a reference to the application context.
        /// </summary>
        public IApplicationContext ApplicationContext { get; }

        /// <summary>
        /// Reads a packet from the given <see cref="INetworkMessage"/>.
        /// </summary>
        /// <param name="message">The message to read from.</param>
        /// <returns>The packet read from the message.</returns>
        public override IIncomingPacket ReadFromMessage(INetworkMessage message)
        {
            message.ThrowIfNull(nameof(message));

            // OS and Version were included in plain in this packet after version 7.70
            var operatingSystem = message.GetUInt16();
            var version = message.GetUInt16();

            var targetSpan = message.Buffer[message.Cursor..message.Length];
            var decryptedBytes = this.ApplicationContext.RsaDecryptor.Decrypt(targetSpan.ToArray());

            decryptedBytes.CopyTo(targetSpan);

            return new GameLogInPacket(
                xteaKey: new uint[]
                {
                    message.GetUInt32(),
                    message.GetUInt32(),
                    message.GetUInt32(),
                    message.GetUInt32(),
                },
                operatingSystem: operatingSystem,       // OS was only included in this packet before version 7.71
                version: version,                       // Version was only included in this packet before version 7.71
                isGamemaster: message.GetByte() > 0,
                accountNumber: message.GetUInt32(),
                characterName: message.GetString(),
                password: message.GetString());
        }
    }
}