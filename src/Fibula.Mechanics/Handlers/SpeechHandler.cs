// -----------------------------------------------------------------
// <copyright file="SpeechHandler.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Handlers
{
    using System.Collections.Generic;
    using Fibula.Client.Contracts.Abstractions;
    using Fibula.Common.Utilities;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Packets.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Serilog;

    /// <summary>
    /// Class that represents a handler for a request to speak.
    /// </summary>
    public class SpeechHandler : GameHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpeechHandler"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="gameInstance">A reference to the game instance.</param>
        public SpeechHandler(ILogger logger, IGame gameInstance)
            : base(logger, gameInstance)
        {
        }

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="incomingPacket">The packet to handle.</param>
        /// <param name="client">A reference to the client from where this request originated from, for context.</param>
        /// <returns>A collection of <see cref="IOutboundPacket"/>s that compose that synchronous response, if any.</returns>
        public override IEnumerable<IOutboundPacket> HandleRequest(IIncomingPacket incomingPacket, IClient client)
        {
            incomingPacket.ThrowIfNull(nameof(incomingPacket));
            client.ThrowIfNull(nameof(client));

            if (!(incomingPacket is ISpeechInfo speechInfo))
            {
                this.Logger.Error($"Expected packet info of type {nameof(ISpeechInfo)} but got {incomingPacket.GetType().Name}.");

                return null;
            }

            this.Game.CreatureSpeech(client.PlayerId, speechInfo.SpeechType, speechInfo.ChannelType, speechInfo.Content, speechInfo.Receiver);

            return null;
        }
    }
}
