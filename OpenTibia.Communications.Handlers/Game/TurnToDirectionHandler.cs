// -----------------------------------------------------------------
// <copyright file="TurnToDirectionHandler.cs" company="2Dudes">
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
    using OpenTibia.Communications.Handlers;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Operations.Arguments;
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
        /// <param name="operationFactory">A reference to the operation factory in use.</param>
        /// <param name="gameContext">A reference to the game context to use.</param>
        /// <param name="direction">The direction in which the walk is happening.</param>
        public TurnToDirectionHandler(ILogger logger, IOperationFactory operationFactory, IGameContext gameContext, Direction direction)
            : base(logger, operationFactory, gameContext)
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
        /// <returns>A collection of <see cref="IOutgoingPacket"/>s that compose that synchronous response, if any.</returns>
        public override IEnumerable<IOutgoingPacket> HandleRequest(INetworkMessage message, IConnection connection)
        {
            // No other content in message.
            if (this.Context.CreatureFinder.FindCreatureById(connection.PlayerId) is IPlayer player)
            {
                player.ClearAllLocationActions();

                this.Context.Scheduler.CancelAllFor(player.Id, typeof(IMovementOperation));

                this.ScheduleNewOperation(OperationType.Turn, new TurnToDirectionOperationCreationArguments(player, this.Direction));
            }

            return null;
        }
    }
}