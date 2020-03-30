// -----------------------------------------------------------------
// <copyright file="SpeechHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Handlers.Game
{
    using System.Collections.Generic;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Communications.Handlers;
    using OpenTibia.Communications.Packets;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Operations.Arguments;
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
        /// <param name="gameContext">A reference to the game context to use.</param>
        public SpeechHandler(ILogger logger, IGameContext gameContext)
            : base(logger, gameContext)
        {
        }

        /// <summary>
        /// Gets the type of packet that this handler is for.
        /// </summary>
        public override byte ForPacketType => (byte)IncomingGamePacketType.Speech;

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        /// <returns>A collection of <see cref="IOutgoingPacket"/>s that compose that synchronous response, if any.</returns>
        public override IEnumerable<IOutgoingPacket> HandleRequest(INetworkMessage message, IConnection connection)
        {
            var speechInfo = message.ReadSpeechInfo();

            if (!(this.Context.CreatureFinder.FindCreatureById(connection.PlayerId) is IPlayer player))
            {
                return null;
            }

            //// TODO: proper implementation.
            // var msgStr = speechInfo.Content;

            // if (msgStr.ToLower().StartsWith("test"))
            // {
            //    this.Game.TestingViaCreatureSpeech(player, msgStr);
            // }

            this.ScheduleNewOperation(
                this.Context.OperationFactory.Create(
                    OperationType.Speech,
                    new SpeechOperationCreationArguments(player.Id, speechInfo)));

            return null;
        }
    }
}