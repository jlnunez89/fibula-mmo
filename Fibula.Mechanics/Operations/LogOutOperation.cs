// -----------------------------------------------------------------
// <copyright file="LogOutOperation.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Operations
{
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Utilities;
    using Fibula.Communications.Packets.Outgoing;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Map.Contracts.Abstractions;
    using Fibula.Map.Contracts.Extensions;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Enumerations;
    using Fibula.Notifications;
    using Fibula.Notifications.Arguments;

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
        /// Gets the type of exhaustion that this operation produces.
        /// </summary>
        public override ExhaustionType ExhaustionType => ExhaustionType.None;

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
                new TextMessageNotification(
                    () => this.Player.YieldSingleItem(),
                    new TextMessageNotificationArguments(MessageType.StatusSmall, "You may not logout at this time."))
                .Send(new NotificationContext(context.Logger, context.MapDescriptor, context.CreatureFinder));

                return;
            }

            if (!context.Map.GetTileAt(this.Player.Location, out ITile tile))
            {
                return;
            }

            // TODO: more validations missing

            // At this point, we're allowed to log this player out, so go for it.
            var playerLocation = this.Player.Location;
            var removedFromMap = this.RemoveCreature(context, this.Player);

            if (removedFromMap || this.Player.IsDead)
            {
                if (this.Player.Client.Connection != null)
                {
                    this.Player.Client.Connection.Close();
                }

                if (!this.Player.IsDead)
                {
                    new GenericNotification(
                        () => context.Map.PlayersThatCanSee(playerLocation),
                        new GenericNotificationArguments(new MagicEffectPacket(playerLocation, AnimatedEffect.Puff)))
                    .Send(new NotificationContext(context.Logger, context.MapDescriptor, context.CreatureFinder));
                }

                context.CreatureManager.UnregisterCreature(this.Player);
            }
        }
    }
}
