// -----------------------------------------------------------------
// <copyright file="MoveUseEventRulesFactory.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Events.MoveUseFile
{
    using System;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Events.MoveUseFile.EventRules;
    using OpenTibia.Server.Parsing.CipFiles.Models;
    using Serilog;

    /// <summary>
    /// Class that represents an item event factory for the move/use events file.
    /// </summary>
    public class MoveUseEventRulesFactory : IEventRulesFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoveUseEventRulesFactory"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        public MoveUseEventRulesFactory(ILogger logger)
        {
            logger.ThrowIfNull(nameof(logger));

            this.Logger = logger;
        }

        /// <summary>
        /// Gets a reference to the logger in use.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Creates a new item event from a parsed instance of <see cref="IEventRuleCreationArguments"/>, that must be castable to <see cref="ParsedEventRule"/>.
        /// </summary>
        /// <param name="gameApi">A reference to the game API to initialize rules with.</param>
        /// <param name="eventRuleArgs">The parsed event rule creation arguments..</param>
        /// <returns>A new instance of a <see cref="IEventRule"/> implementation, based on the parsed event.</returns>
        public IEventRule Create(IGameApi gameApi, IEventRuleCreationArguments eventRuleArgs)
        {
            gameApi.ThrowIfNull(nameof(gameApi));
            eventRuleArgs.ThrowIfNull(nameof(eventRuleArgs));

            if (!(eventRuleArgs is ParsedEventRule rawEventRuleArgs))
            {
                throw new NotSupportedException($"Unsupported instance of creation arguments '{nameof(eventRuleArgs)}' supplied. The supplied type must be castable to {typeof(ParsedEventRule)}.");
            }

            if (!Enum.TryParse(rawEventRuleArgs.Type, out EventRuleType eventType))
            {
                throw new ArgumentException($"Invalid rule '{rawEventRuleArgs.Type}' supplied.");
            }

            return eventType switch
            {
                EventRuleType.Collision => new CollisionMoveUseEventRule(this.Logger, gameApi, rawEventRuleArgs.ConditionSet, rawEventRuleArgs.ActionSet),
                EventRuleType.Use => new UseMoveUseEventRule(this.Logger, gameApi, rawEventRuleArgs.ConditionSet, rawEventRuleArgs.ActionSet),
                EventRuleType.MultiUse => new MultiUseMoveUseEventRule(this.Logger, gameApi, rawEventRuleArgs.ConditionSet, rawEventRuleArgs.ActionSet),
                EventRuleType.Separation => new SeparationMoveUseEventRule(this.Logger, gameApi, rawEventRuleArgs.ConditionSet, rawEventRuleArgs.ActionSet),
                EventRuleType.Movement => new MovementMoveUseEventRule(this.Logger, gameApi, rawEventRuleArgs.ConditionSet, rawEventRuleArgs.ActionSet),

                _ => throw new InvalidCastException($"Unsupported type of event on {nameof(MoveUseEventRulesFactory)}: {rawEventRuleArgs.Type}"),
            };
        }
    }
}
