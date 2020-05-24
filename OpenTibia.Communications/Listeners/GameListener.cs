// -----------------------------------------------------------------
// <copyright file="GameListener.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications
{
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Microsoft.Extensions.Options;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Security.Contracts;
    using Serilog;

    /// <summary>
    /// Class that extends the standard <see cref="BaseListener"/> for a game listener.
    /// </summary>
    /// <typeparam name="THandlerSelector">The type of handler selector in use here.</typeparam>
    public class GameListener<THandlerSelector> : BaseListener
        where THandlerSelector : class, IHandlerSelector
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameListener{THandlerSelector}"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="options">The options for this listener.</param>
        /// <param name="handlerSelector">A reference to the handler selector to use in this protocol.</param>
        /// <param name="dosDefender">A reference to the DoS defender service implementation.</param>
        public GameListener(ILogger logger, IOptions<GameListenerOptions> options, THandlerSelector handlerSelector, IDoSDefender dosDefender)
            : base(logger, options.Value, handlerSelector, dosDefender)
        {
            Validator.ValidateObject(options.Value, new ValidationContext(options.Value), validateAllProperties: true);

            this.Options = options.Value;
        }

        /// <summary>
        /// Gets a reference to the options for this listener.
        /// </summary>
        public GameListenerOptions Options { get; }

        /// <summary>
        /// Processes an incomming message from the connection.
        /// </summary>
        /// <param name="connection">The connection where the message is being read from.</param>
        /// <param name="inboundMessage">The message to process.</param>
        public override void ProcessMessage(IConnection connection, INetworkMessage inboundMessage)
        {
            connection.ThrowIfNull(nameof(connection));
            inboundMessage.ThrowIfNull(nameof(inboundMessage));

            byte packetType;

            if (!connection.IsAuthenticated || connection.XTeaKey.Sum(b => b) == 0)
            {
                // this is a new connection...
                packetType = inboundMessage.GetByte();

                if (packetType != (byte)IncomingGamePacketType.LogIn)
                {
                    this.Logger.Warning($"Unexpected packet {packetType} routed to game protocol, closing connection.");

                    // but this is not the packet we were expecting for a new connection.
                    connection.Close();
                    return;
                }

                if (this.Options.ClientVersion.Numeric > 770)
                {
                    // OS and Version were included in plain in this packet after version 7.70
                    var os = inboundMessage.GetUInt16();
                    var version = inboundMessage.GetUInt16();

                    if (version != this.Options.ClientVersion.Numeric)
                    {
                        this.Logger.Information($"Client attempted to connect with version: {version}, OS: {os}. Expected version: {this.Options.ClientVersion.Numeric}.");

                        connection.Close();
                    }
                }

                // Make a copy of the message in case we fail to decrypt using the first set of keys.
                var messageCopy = inboundMessage.Copy();

                inboundMessage.RsaDecrypt(useCipKeys: this.Options.UsingCipsoftRsaKeys);

                if (inboundMessage.GetByte() != 0)
                {
                    this.Logger.Information($"Failed to decrypt client connection data using {(this.Options.UsingCipsoftRsaKeys ? "CipSoft" : "OTServ")} RSA keys, attempting the other set...");

                    // means the RSA decrypt was unsuccessful, lets try with the other set of RSA keys...
                    inboundMessage = messageCopy;

                    inboundMessage.RsaDecrypt(useCipKeys: !this.Options.UsingCipsoftRsaKeys);

                    if (inboundMessage.GetByte() != 0)
                    {
                        this.Logger.Warning($"Unable to decrypt and communicate with client. Neither CipSoft or OTServ RSA keys matched... giving up.");

                        // These RSA keys are also usuccessful... so give up.
                        connection.Close();
                        return;
                    }
                }
            }
            else
            {
                // Decrypt message using XTea
                inboundMessage.XteaDecrypt(connection.XTeaKey);
                inboundMessage.GetUInt16();
                packetType = inboundMessage.GetByte();
            }

            var handler = this.HandlerSelector.SelectForType(packetType);

            if (handler == null)
            {
                return;
            }

            var responsePackets = handler.HandleRequest(inboundMessage, connection);

            if (responsePackets != null && responsePackets.Any())
            {
                // Send any responses prepared for this.
                var responseMessage = this.PrepareResponse(responsePackets);

                connection.Send(responseMessage);
            }
        }
    }
}
