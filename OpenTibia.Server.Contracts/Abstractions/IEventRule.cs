// -----------------------------------------------------------------
// <copyright file="IEventRule.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Abstractions
{
    using System.Collections.Generic;
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Interface for event rules.
    /// </summary>
    public interface IEventRule
    {
        /// <summary>
        /// Gets the type of this event rule.
        /// </summary>
        EventRuleType Type { get; }

        /// <summary>
        /// Gets the conditions for the event to happen.
        /// </summary>
        IEnumerable<IEventRuleFunction> Conditions { get; }

        /// <summary>
        /// Gets the actions to perform when an event is executed.
        /// </summary>
        IEnumerable<IEventRuleFunction> Actions { get; }

        /// <summary>
        /// Checks whether this event rule can be executed.
        /// This generally means the <see cref="Conditions"/> all evaluate to true.
        /// </summary>
        /// <param name="gameApi">A reference to the game's api in use.</param>
        /// <param name="primaryThing">The primary thing involved in the event rule.</param>
        /// <param name="secondaryThing">The secondary thing involved in the event rule.</param>
        /// <param name="requestingPlayer">The player requesting the event rule execution.</param>
        /// <returns>True if the rule can be executed, false otherwise.</returns>
        bool CanBeExecuted(IGame gameApi, IThing primaryThing, IThing secondaryThing = null, IPlayer requestingPlayer = null);

        /// <summary>
        /// Executes this event rule.
        /// This generally means executing the event rule's <see cref="Actions"/>.
        /// </summary>
        /// <param name="gameApi">A reference to the game's api in use.</param>
        /// <param name="primaryThing">The primary thing involved in the event rule.</param>
        /// <param name="secondaryThing">The secondary thing involved in the event rule.</param>
        /// <param name="requestingPlayer">The player requesting the event rule execution.</param>
        void Execute(IGame gameApi, ref IThing primaryThing, ref IThing secondaryThing, ref IPlayer requestingPlayer);
    }
}