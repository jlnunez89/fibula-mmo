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
    using Fibula.Common.Contracts.Abstractions;
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
        /// <param name="player">The player that is in fight.</param>
        public InFightCondition(IPlayer player)
            : base(ConditionType.InFight)
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
        /// Aggregates this condition into another of the same type.
        /// </summary>
        /// <param name="conditionOfSameType">The condition to aggregate into.</param>
        /// <returns>True if the conditions were aggregated (changed), and false if nothing was done.</returns>
        public override bool Aggregate(ICondition conditionOfSameType)
        {
            conditionOfSameType.ThrowIfNull(nameof(conditionOfSameType));

            if (!(conditionOfSameType is InFightCondition))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Executes the condition's logic.
        /// </summary>
        /// <param name="context">The execution context for this condition.</param>
        protected override void Execute(IConditionContext context)
        {
            // For InFight, we just send the updated flags.
            this.SendNotificationAsync(context, new GenericNotification(() => this.Player.YieldSingleItem(), new PlayerConditionsPacket(this.Player)));
        }
    }
}
