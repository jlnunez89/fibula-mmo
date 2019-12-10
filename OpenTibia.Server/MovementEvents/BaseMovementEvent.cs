// -----------------------------------------------------------------
// <copyright file="BaseMovementEvent.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.MovementEvents
{
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Scheduling;
    using OpenTibia.Scheduling.Contracts.Enumerations;
    using OpenTibia.Server;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Notifications;
    using Serilog;

    /// <summary>
    /// Class that represents a common base between movements.
    /// </summary>
    internal abstract class BaseMovementEvent : BaseEvent
    {
        /// <summary>
        /// Caches the requestor creature, if defined.
        /// </summary>
        private ICreature requestor;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMovementEvent"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="game">A reference to the game instance.</param>
        /// <param name="connectionManager">A reference to the connection manager in use.</param>
        /// <param name="creatureFinder">A reference to the creature finder in use.</param>
        /// <param name="requestorId">The id of the creature requesting the movement.</param>
        /// <param name="evaluationTime">The time to evaluate the movement.</param>
        protected BaseMovementEvent(ILogger logger, IGame game, IConnectionManager connectionManager, ICreatureFinder creatureFinder, uint requestorId, EvaluationTime evaluationTime)
            : base(logger, requestorId, evaluationTime)
        {
            game.ThrowIfNull(nameof(game));
            connectionManager.ThrowIfNull(nameof(connectionManager));
            creatureFinder.ThrowIfNull(nameof(creatureFinder));

            this.ConnectionManager = connectionManager;
            this.CreatureFinder = creatureFinder;
            this.Game = game;

            this.ActionsOnFail.Add(new GenericEventAction(this.NotifyOfFailure));
        }

        /// <summary>
        /// Gets the connection manager in use.
        /// </summary>
        public IConnectionManager ConnectionManager { get; }

        /// <summary>
        /// Gets the creature finder instance in use.
        /// </summary>
        public ICreatureFinder CreatureFinder { get; }

        /// <summary>
        /// Gets a reference to the game instance.
        /// </summary>
        public IGame Game { get; }

        /// <summary>
        /// Gets the creature that is requesting the event, if known.
        /// </summary>
        public ICreature Requestor
        {
            get
            {
                if (this.requestor == null)
                {
                    this.requestor = this.RequestorId == 0 ? null : this.CreatureFinder.FindCreatureById(this.RequestorId);
                }

                return this.requestor;
            }
        }

        /// <summary>
        /// Notifies the requestor player, if any,of this failure.
        /// </summary>
        protected void NotifyOfFailure()
        {
            if (this.Requestor is Player player)
            {
                this.Game.RequestNofitication(
                    new GenericNotification(
                        this.Logger,
                        () => { return this.ConnectionManager.FindByPlayerId(player.Id).YieldSingleItem(); },
                        new GenericNotificationArguments(new PlayerWalkCancelPacket(player.Direction), new TextMessagePacket(MessageType.StatusSmall, this.ErrorMessage ?? "Sorry, not possible."))));
            }
        }
    }
}
