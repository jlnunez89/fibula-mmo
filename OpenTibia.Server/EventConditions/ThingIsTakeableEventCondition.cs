// -----------------------------------------------------------------
// <copyright file="ThingIsTakeableEventCondition.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.EventConditions
{
    using OpenTibia.Common.Utilities;
    using OpenTibia.Scheduling.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Class that represents an event condition that evaluates if a thing is takeable.
    /// </summary>
    internal class ThingIsTakeableEventCondition : IEventCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThingIsTakeableEventCondition"/> class.
        /// </summary>
        /// <param name="creatureFinder">A reference to the creature finder in use.</param>
        /// <param name="grabberId">The id of the grabber creature.</param>
        /// <param name="thingMoving">The thing being checked.</param>
        public ThingIsTakeableEventCondition(ICreatureFinder creatureFinder, uint grabberId, IThing thingMoving)
        {
            creatureFinder.ThrowIfNull(nameof(creatureFinder));

            this.CreatureFinder = creatureFinder;
            this.GrabberId = grabberId;
            this.Thing = thingMoving;
        }

        /// <summary>
        /// Gets the reference to the creature finder.
        /// </summary>
        public ICreatureFinder CreatureFinder { get; }

        /// <summary>
        /// Gets the <see cref="IThing"/> to check.
        /// </summary>
        public IThing Thing { get; }

        /// <summary>
        /// Gets the id of the grabber creature.
        /// </summary>
        public uint GrabberId { get; }

        /// <inheritdoc/>
        public string ErrorMessage => "You may not move this object.";

        /// <inheritdoc/>
        public bool Evaluate()
        {
            var grabber = this.GrabberId == 0 ? null : this.CreatureFinder.FindCreatureById(this.GrabberId);

            if (grabber == null || this.Thing == null)
            {
                return false;
            }

            // TODO: GrabberId access level?
            return this.Thing is IItem item && item.Type.Flags.Contains(ItemFlag.Take);
        }
    }
}