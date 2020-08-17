// -----------------------------------------------------------------
// <copyright file="InFightCondition.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Conditions
{
    using System;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Utilities;
    using Fibula.Communications.Packets.Outgoing;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Notifications;

    /// <summary>
    /// Class that represents a condition for participating in combat.
    /// </summary>
    public class InFightCondition : Condition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InFightCondition"/> class.
        /// </summary>
        /// <param name="endTime">The date and time at which the condition is set to end.</param>
        /// <param name="player">The player that is in fight.</param>
        public InFightCondition(DateTimeOffset endTime, IPlayer player)
            : base(ConditionType.InFight, endTime)
        {
            player.ThrowIfNull(nameof(player));

            this.Player = player;

            this.ExcludeFromTelemetry = true;
        }

        /// <summary>
        /// Gets the player in fight.
        /// </summary>
        public IPlayer Player { get; }

        /// <summary>
        /// Executes the condition's logic.
        /// </summary>
        /// <param name="context">The execution context for this condition.</param>
        protected override void Execute(IConditionContext context)
        {
            // For InFight, we just send the updated flags.
            this.SendNotificationAsync(
                context,
                new GenericNotification(() => this.Player.YieldSingleItem(), new PlayerConditionsPacket(this.Player)),
                TimeSpan.FromSeconds(1));
        }
    }
}
