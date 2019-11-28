// -----------------------------------------------------------------
// <copyright file="CreatureThrowBetweenFloorsEventCondition.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.MovementEvents.EventConditions
{
    using OpenTibia.Common.Utilities;
    using OpenTibia.Scheduling.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Class that represents a condition that evaluates whether a creature can throw a thing to a location on a different floor.
    /// </summary>
    internal class CreatureThrowBetweenFloorsEventCondition : IEventCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureThrowBetweenFloorsEventCondition"/> class.
        /// </summary>
        /// <param name="creatureFinder">A reference to the creature finder in use.</param>
        /// <param name="creatureRequestingId">The id of the creature requesting the throw.</param>
        /// <param name="thingMoving">The thing that it is throwing.</param>
        /// <param name="toLocation">The location to where it is being thrown.</param>
        public CreatureThrowBetweenFloorsEventCondition(ICreatureFinder creatureFinder, uint creatureRequestingId, IThing thingMoving, Location toLocation)
        {
            creatureFinder.ThrowIfNull(nameof(creatureFinder));

            this.CreatureFinder = creatureFinder;
            this.RequestorId = creatureRequestingId;
            this.Thing = thingMoving;
            this.ToLocation = toLocation;
        }

        /// <summary>
        /// Gets the reference to the creature finder.
        /// </summary>
        public ICreatureFinder CreatureFinder { get; }

        /// <summary>
        /// Gets the location to where the thing is being thrown.
        /// </summary>
        public Location ToLocation { get; }

        /// <summary>
        /// Gets the id of the creature requesting the throw.
        /// </summary>
        public uint RequestorId { get; }

        /// <summary>
        /// Gets the <see cref="IThing"/> being thrown.
        /// </summary>
        public IThing Thing { get; }

        /// <inheritdoc/>
        public string ErrorMessage => "You may not throw there.";

        /// <inheritdoc/>
        public bool Evaluate()
        {
            var requestor = this.RequestorId == 0 ? null : this.CreatureFinder.FindCreatureById(this.RequestorId);

            if (requestor == null || !(this.Thing is ICreature thingAsCreature))
            {
                // Not a creature requesting this one, possibly a script.
                // Or the thing moving is null, not this policy's job to restrict this...
                return true;
            }

            var locDiff = thingAsCreature.Location - this.ToLocation;

            return locDiff.Z == 0;
        }
    }
}