// -----------------------------------------------------------------
// <copyright file="MoveUseItemEventFactory.cs" company="2Dudes">
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
    using OpenTibia.Server.Parsing.Contracts;
    using Serilog;

    /// <summary>
    /// Class that represents an item event factory for the move/use events file.
    /// </summary>
    public class MoveUseItemEventFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoveUseItemEventFactory"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="scriptFactory">A reference to the script factory.</param>
        public MoveUseItemEventFactory(ILogger logger, IScriptApi scriptFactory)
        {
            logger.ThrowIfNull(nameof(logger));
            scriptFactory.ThrowIfNull(nameof(scriptFactory));

            this.Logger = logger;
            this.ScriptFactory = scriptFactory;
        }

        /// <summary>
        /// Gets a reference to the logger in use.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Gets a reference to the script factory.
        /// </summary>
        public IScriptApi ScriptFactory { get; }

        /// <summary>
        /// Creates a new item event from a parsed <see cref="RawEventRule"/>.
        /// </summary>
        /// <param name="rawEventRule">The parsed event.</param>
        /// <returns>A new instance of a <see cref="IEventRule"/> implementation, based on the parsed event.</returns>
        public MoveUseEventRule Create(RawEventRule rawEventRule)
        {
            rawEventRule.ThrowIfNull(nameof(rawEventRule));

            if (!Enum.TryParse(rawEventRule.Type, out EventRuleType eventType))
            {
                throw new ArgumentException($"Invalid rule '{rawEventRule.Type}' supplied.");
            }

            return eventType switch
            {
                EventRuleType.Collision => new CollisionEventRule(this.Logger, this.ScriptFactory, rawEventRule.ConditionSet, rawEventRule.ActionSet),
                EventRuleType.Use => new UseItemEventRule(this.Logger, this.ScriptFactory, rawEventRule.ConditionSet, rawEventRule.ActionSet),
                EventRuleType.MultiUse => new UseItemOnEventRule(this.Logger, this.ScriptFactory, rawEventRule.ConditionSet, rawEventRule.ActionSet),
                EventRuleType.Separation => new SeparationEventRule(this.Logger, this.ScriptFactory, rawEventRule.ConditionSet, rawEventRule.ActionSet),
                EventRuleType.Movement => new ThingMovementEventRule(this.Logger, this.ScriptFactory, rawEventRule.ConditionSet, rawEventRule.ActionSet),

                _ => throw new InvalidCastException($"Unsupported type of event on {nameof(MoveUseItemEventFactory)}: {rawEventRule.Type}"),
            };
        }
    }
}
