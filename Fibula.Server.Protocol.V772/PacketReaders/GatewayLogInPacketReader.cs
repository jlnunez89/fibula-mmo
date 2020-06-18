// -----------------------------------------------------------------
// <copyright file="GatewayLogInPacketReader.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Protocol.V772.PacketReaders
{
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Common.Utilities;
    using Fibula.Communications;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Packets.Incoming;
    using Serilog;

    /// <summary>
    /// Class that represents a log in packet reader for the gateway server.
    /// </summary>
    public sealed class GatewayLogInPacketReader : BasePacketReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayLogInPacketReader"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="applicationContext">A reference to the application context.</param>
        public GatewayLogInPacketReader(ILogger logger, IApplicationContext applicationContext)
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

            // OS and Version were included plainly in this version
            var operatingSystem = message.GetUInt16();
            var version = message.GetUInt16();

            // unknown bytes.
            message.SkipBytes(12);

            var targetSpan = message.Buffer[message.Cursor..message.Length];
            var decryptedBytes = this.ApplicationContext.RsaDecryptor.Decrypt(targetSpan.ToArray());

            decryptedBytes.CopyTo(targetSpan);

            return new GatewayLogInPacket(
                version,
                operatingSystem,
                xteaKey: new uint[] { message.GetUInt32(), message.GetUInt32(), message.GetUInt32(), message.GetUInt32() },
                accountNumber: message.GetUInt32(),
                password: message.GetString());
        }
    }
}