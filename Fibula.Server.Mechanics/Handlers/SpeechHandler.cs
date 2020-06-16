// -----------------------------------------------------------------
// <copyright file="SpeechHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Protocol.V772.RequestHandlers
{
    using System.Collections.Generic;
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Communications.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
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
        public override byte ForRequestType => (byte)GameRequestType.Speech;

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        /// <returns>A collection of <see cref="IResponsePacket"/>s that compose that synchronous response, if any.</returns>
        public override IEnumerable<IResponsePacket> HandleRequest(INetworkMessage message, IConnection connection)
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
                    new SpeechOperationCreationArguments(
                        player.Id,
                        speechInfo.Type,
                        speechInfo.ChannelId,
                        speechInfo.Receiver,
                        speechInfo.Content)));

            return null;
        }
    }
}