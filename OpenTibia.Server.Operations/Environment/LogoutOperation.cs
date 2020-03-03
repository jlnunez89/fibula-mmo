// -----------------------------------------------------------------
// <copyright file="LogoutOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Operations.Environment
{
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Notifications;
    using OpenTibia.Server.Notifications.Arguments;
    using OpenTibia.Server.Operations.Actions;
    using Serilog;

    /// <summary>
    /// Class that represents a logout operation.
    /// </summary>
    public class LogOutOperation : BaseEnvironmentOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogOutOperation"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="context">The context of the operation.</param>
        /// <param name="requestorId">The id of the creature requesting the use.</param>
        /// <param name="player">The player being logged out.</param>
        public LogOutOperation(ILogger logger, IElevatedOperationContext context, uint requestorId, IPlayer player)
            : base(logger, context, requestorId)
        {
            this.Player = player;
        }

        /// <summary>
        /// Gets the player to log out.
        /// </summary>
        public IPlayer Player { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        public override void Execute()
        {
            if (!this.Player.IsAllowedToLogOut)
            {
                this.Context.Scheduler.ScheduleEvent(
                    new TextMessageNotification(
                        this.Logger,
                        () => this.Context.ConnectionManager.FindByPlayerId(this.Player.Id).YieldSingleItem(),
                        new TextMessageNotificationArguments(MessageType.StatusSmall, "You may not logout at this time.")));

                return;
            }

            if (!this.Context.TileAccessor.GetTileAt(this.Player.Location, out ITile tile))
            {
                return;
            }

            // TODO: more validations missing

            // At this point, we're allowed to log this player out, so go for it.
            var removedFromMap = this.RemoveCreature(this.Player);

            if (removedFromMap)
            {
                this.Context.CreatureManager.UnregisterCreature(this.Player);

                var currentConnection = this.Context.ConnectionManager.FindByPlayerId(this.Player.Id);

                if (currentConnection != null)
                {
                    currentConnection.Close();

                    this.Context.ConnectionManager.Unregister(currentConnection);
                }
            }
        }
    }
}
