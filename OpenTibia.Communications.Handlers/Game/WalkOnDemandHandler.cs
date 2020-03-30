// -----------------------------------------------------------------
// <copyright file="WalkOnDemandHandler.cs" company="2Dudes">
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
    /// Abstract class that represents the base for all player walk handlers.
    /// </summary>
    public abstract class WalkOnDemandHandler : GameHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WalkOnDemandHandler"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="gameContext">A reference to the game context to use.</param>
        /// <param name="direction">The direction in which the turn is happening.</param>
        public WalkOnDemandHandler(ILogger logger, IGameContext gameContext, Direction direction)
            : base(logger, gameContext)
        {
            this.Direction = direction;
        }

        /// <summary>
        /// Gets the direction to walk to.
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
            if (!(this.Context.CreatureFinder.FindCreatureById(connection.PlayerId) is IPlayer player))
            {
                return null;
            }

            //player.ClearAllLocationBasedOperations();

            this.Context.Scheduler.CancelAllFor(player.Id, typeof(IMovementOperation));

            var nextLocation = player.Location.LocationAt(this.Direction);

            this.ScheduleNewOperation(
                this.Context.OperationFactory.Create(
                    OperationType.Movement,
                    new MovementOperationCreationArguments(
                        player.Id,
                        ICreature.CreatureThingId,
                        player.Location,
                        fromIndex: 0xFF,
                        player.Id,
                        nextLocation,
                        player.Id)));

            return null;
        }
    }
}