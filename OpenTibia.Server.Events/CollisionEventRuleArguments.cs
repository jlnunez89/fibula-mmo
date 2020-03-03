// -----------------------------------------------------------------
// <copyright file="CollisionEventRuleArguments.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Class that represents arguments for a collision event rule.
    /// </summary>
    public class CollisionEventRuleArguments : IEventRuleArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollisionEventRuleArguments"/> class.
        /// </summary>
        /// <param name="atLocation">The location of the collision.</param>
        /// <param name="thingMoving">The thing being moved.</param>
        /// <param name="requestingCreature">Optional. The creature requesting the event.</param>
        public CollisionEventRuleArguments(Location atLocation, IThing thingMoving, ICreature requestingCreature = null)
        {
            thingMoving.ThrowIfNull(nameof(thingMoving));

            this.AtLocation = atLocation;
            this.ThingMoving = thingMoving;
            this.Requestor = requestingCreature;
        }

        /// <summary>
        /// Gets the location at which the collision happens.
        /// </summary>
        public Location AtLocation { get; }

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