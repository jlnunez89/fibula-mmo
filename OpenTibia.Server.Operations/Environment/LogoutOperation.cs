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
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Operations.Actions;
    using OpenTibia.Server.Operations.Notifications;
    using OpenTibia.Server.Operations.Notifications.Arguments;
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
            this.ActionsOnPass.Add(() =>
            {
                bool success = this.LogOut(player);

                if (!success)
                {
                    // handles check for isPlayer.
                    // this.NotifyOfFailure();
                    return;
                }
            });
        }

        /// <summary>
        /// Attempts to log a player out of the game.
        /// </summary>
        /// <param name="player">The player to attempt to attempt log out.</param>
        /// <returns>True if the log-out was successful, false otherwise.</returns>
        private bool LogOut(IPlayer player)
        {
            if (!player.IsAllowedToLogOut)
            {
                this.Context.Scheduler.ImmediateEvent(
                    new GenericNotification(
                        this.Logger,
                        () => this.Context.ConnectionManager.FindByPlayerId(player.Id).YieldSingleItem(),
                        new GenericNotificationArguments(new TextMessagePacket(MessageType.StatusSmall, "You may not logout at this time."))));

                return false;
            }

            if (!this.Context.TileAccessor.GetTileAt(player.Location, out ITile tile))
            {
                return false;
            }

            // TODO: more validations missing

            // At this point, we're allowed to log this player out, so go for it.
            var removedFromMap = this.RemoveCreature(player);

            if (removedFromMap)
            {
                this.Context.CreatureManager.UnregisterCreature(player);

                var currentConnection = this.Context.ConnectionManager.FindByPlayerId(player.Id);

                if (currentConnection != null)
                {
                    currentConnection.Close();

                    this.Context.ConnectionManager.Unregister(currentConnection);
                }

                player.TargetChanged -= this.HandleCombatantTargetChanged;
                player.ChaseModeChanged -= this.HandleCombatantChaseModeChanged;
                player.Inventory.SlotChanged -= this.HandlePlayerInventoryChanged;
                player.CombatCreditsConsumed -= this.HandleCombatCreditsConsumed;
            }

            return removedFromMap;
        }
    }
}
