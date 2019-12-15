// -----------------------------------------------------------------
// <copyright file="EventGrammar.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Parsing.Grammar
{
    using System.Linq;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Parsing.Contracts;
    using Sprache;

    public class EventGrammar
    {
        public static readonly Parser<MoveUseEvent> Event =
            from rule in CipGrammar.ConditionalActionRule
            select new MoveUseEvent(rule);

        public class MoveUseEvent
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MoveUseEvent"/> class.
            /// </summary>
            /// <param name="rule">The event's conditional rule.</param>
            public MoveUseEvent(ConditionalActionRule rule)
            {
                rule.ThrowIfNull(nameof(rule));

                this.Type = rule.ConditionSet.FirstOrDefault();
                this.Rule = rule;

                // Remove first element since we already parsed it into 'Type'.
                rule.ConditionSet.RemoveAt(0);
            }

            /// <summary>
            /// Gets the type of event.
            /// </summary>
            public string Type { get; }

            /// <summary>
            /// Gets the event's conditional rule.
            /// </summary>
            public ConditionalActionRule Rule { get; }
        }
    }
}
