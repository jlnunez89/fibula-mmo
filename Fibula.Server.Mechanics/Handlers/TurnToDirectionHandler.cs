// -----------------------------------------------------------------
// <copyright file="TurnToDirectionHandler.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using Serilog;

    /// <summary>
    /// Abstract class that represents the player turning to a direction handler.
    /// </summary>
    public abstract class TurnToDirectionHandler : GameHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TurnToDirectionHandler"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="gameContext">A reference to the game context to use.</param>
        /// <param name="direction">The direction in which the walk is happening.</param>
        public TurnToDirectionHandler(ILogger logger, IGameContext gameContext, Direction direction)
            : base(logger, gameContext)
        {
            this.Direction = direction;
        }

        /// <summary>
        /// Gets the direction to turn to.
        /// </summary>
        public Direction Direction { get; }

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        /// <returns>A collection of <see cref="IResponsePacket"/>s that compose that synchronous response, if any.</returns>
        public override IEnumerable<IResponsePacket> HandleRequest(INetworkMessage message, IConnection connection)
        {
            // No other content in message.
            if (this.Context.CreatureFinder.FindCreatureById(connection.PlayerId) is IPlayer player)
            {
                //player.ClearAllLocationBasedOperations();

                this.Context.Scheduler.CancelAllFor(player.Id, typeof(IMovementOperation));

                this.ScheduleNewOperation(
                    this.Context.OperationFactory.Create(
                        OperationType.Turn,
                        new TurnToDirectionOperationCreationArguments(player, this.Direction)));
            }

            return null;
        }
    }
}