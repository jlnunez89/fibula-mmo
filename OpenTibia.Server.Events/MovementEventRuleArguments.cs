// -----------------------------------------------------------------
// <copyright file="MovementEventRuleArguments.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts.Abstractions;

    /// <summary>
    /// Class that represents arguments for a movement event rule.
    /// </summary>
    public class MovementEventRuleArguments : IEventRuleArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MovementEventRuleArguments"/> class.
        /// </summary>
        /// <param name="thingMoving">The thing being moved.</param>
        /// <param name="requestingCreature">Optional. The creature requesting the event.</param>
        public MovementEventRuleArguments(IThing thingMoving, ICreature requestingCreature = null)
        {
            thingMoving.ThrowIfNull(nameof(thingMoving));

            this.ThingMoving = thingMoving;
            this.Requestor = requestingCreature;
        }

        /// <summary>
        /// Gets the thing that is moving.
        /// </summary>
        public IThing ThingMoving { get; }

        /// <summary>
        /// Gets the creature who requested the use, if any.
        /// </summary>
        public ICreature Requestor { get; }
    }
}