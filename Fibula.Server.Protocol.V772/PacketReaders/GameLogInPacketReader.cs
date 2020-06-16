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

namespace Fibula.Server.Protocol.V772.PacketReaders
{
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Common.Utilities;
    using Fibula.Communications;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Packets.Incoming;
    using Fibula.Server.Protocol.V772;
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

            // Make a copy of the message in case we fail to decrypt using the first set of keys.
            var messageCopy = message.Copy();

            message.RsaDecrypt(useCipKeys: this.ApplicationContext.Options.UsingCipsoftRsaKeys);

            // If GetByte() here is not Zero, it means the RSA decrypt was unsuccessful, lets try with the other set of RSA keys...
            if (message.GetByte() != 0)
            {
                this.Logger.Information($"Failed to decrypt client connection data using {(this.ApplicationContext.Options.UsingCipsoftRsaKeys ? "CipSoft" : "OTServ")} RSA keys, attempting the other set...");

                message = messageCopy;

                message.RsaDecrypt(useCipKeys: !this.ApplicationContext.Options.UsingCipsoftRsaKeys);

                if (message.GetByte() != 0)
                {
                    // These RSA keys are also unsuccessful... give up.
                    this.Logger.Warning($"Unable to decrypt and communicate with client. Neither CipSoft or OTServ RSA keys matched... giving up.");

                    return null;
                }
            }

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