// -----------------------------------------------------------------
// <copyright file="RestoreCombatCreditOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Events
{
    using OpenTibia.Common.Utilities;
    using OpenTibia.Scheduling;
    using OpenTibia.Server;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using Serilog;

    /// <summary>
    /// Class that represents an event to restore combat credits.
    /// </summary>
    internal class RestoreCombatCreditOperation : BaseEvent
    {
        private const int AmountToRestore = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="RestoreCombatCreditOperation"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="combatant">The combatant that is being restored.</param>
        /// <param name="creditType">The type of combat credit to restore.</param>
        public RestoreCombatCreditOperation(ILogger logger, ICombatant combatant, CombatCreditType creditType)
            : base(logger, combatant?.Id ?? 0)
        {
            combatant.ThrowIfNull(nameof(combatant));

            this.Combatant = combatant;

            switch (creditType)
            {
                case CombatCreditType.Attack:
                    this.ActionsOnPass.Add(new GenericEventAction(() => this.Combatant.RestoreCredits(CombatCreditType.Attack, AmountToRestore)));
                    break;

                case CombatCreditType.Defense:
                    this.ActionsOnPass.Add(new GenericEventAction(() => this.Combatant.RestoreCredits(CombatCreditType.Defense, AmountToRestore)));
                    break;
            }
        }

        /// <summary>
        /// Gets the combatant that is attacking on this operation.
        /// </summary>
        public ICombatant Combatant { get; }
    }
}