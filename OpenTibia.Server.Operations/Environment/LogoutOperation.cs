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

    /// <summary>
    /// Class that represents a logout operation.
    /// </summary>
    public class LogOutOperation : BaseEnvironmentOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogOutOperation"/> class.
        /// </summary>
        /// <param name="requestorId">The id of the creature requesting the use.</param>
        /// <param name="player">The player being logged out.</param>
        public LogOutOperation(uint requestorId, IPlayer player)
            : base(requestorId)
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
        /// <param name="context">A reference to the operation context.</param>
        protected override void Execute(IElevatedOperationContext context)
        {
            if (!this.Player.IsAllowedToLogOut)
            {
                context.Scheduler.ScheduleEvent(
                    new TextMessageNotification(
                        () => context.ConnectionManager.FindByPlayerId(this.Player.Id).YieldSingleItem(),
                        new TextMessageNotificationArguments(MessageType.StatusSmall, "You may not logout at this time.")));

                return;
            }

            if (!context.TileAccessor.GetTileAt(this.Player.Location, out ITile tile))
            {
                return;
            }

            // TODO: more validations missing

            // At this point, we're allowed to log this player out, so go for it.
            var removedFromMap = this.RemoveCreature(context, this.Player);

            if (removedFromMap)
            {
                context.CreatureManager.UnregisterCreature(this.Player);

                var currentConnection = context.ConnectionManager.FindByPlayerId(this.Player.Id);

                if (currentConnection != null)
                {
                    currentConnection.Close();

                    context.ConnectionManager.Unregister(currentConnection);
                }
            }
        }
    }
}
