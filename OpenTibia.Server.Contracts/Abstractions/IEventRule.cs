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
        /// Gets the primary thing involved in the event.
        /// </summary>
        IThing PrimaryThing { get; }

        /// <summary>
        /// Gets the secondary thing involved in the event.
        /// </summary>
        IThing SecondaryThing { get; }

        /// <summary>
        /// Gets the player involved in the event.
        /// </summary>
        IPlayer Player { get; }

        /// <summary>
        /// Gets a value indicating whether this event can be executed.
        /// This generally means the <see cref="Conditions"/> have been passed.
        /// </summary>
        bool CanBeExecuted { get; }

        /// <summary>
        /// Gets the conditions for the event to happen.
        /// </summary>
        IEnumerable<IEventRuleFunction> Conditions { get; }

        /// <summary>
        /// Gets the actions to perform when an event is executed.
        /// </summary>
        IEnumerable<IEventRuleFunction> Actions { get; }

        /// <summary>
        /// Sets up this event.
        /// </summary>
        /// <param name="primaryThing">The primary thing involved in the event.</param>
        /// <param name="secondaryThing">The secondary thing involved in the event.</param>
        /// <param name="player">The player involved in the event.</param>
        /// <returns>True if the event is successfully set up, false otherwise.</returns>
        bool Setup(IThing primaryThing, IThing secondaryThing = null, IPlayer player = null);

        /// <summary>
        /// Executes this event.
        /// This generally means executing the event's <see cref="Actions"/>.
        /// </summary>
        void Execute();
    }
}