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
    using System;
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
        /// <param name="creatureRequesting">The creature requesting the throw.</param>
        /// <param name="determineThingMovingFunc">A delegate function to determine the thing that is being thrown.</param>
        /// <param name="determineDestinationLocationFunc">A delegate function to determine the location to where it is being thrown.</param>
        public CreatureThrowBetweenFloorsEventCondition(ICreature creatureRequesting, Func<IThing> determineThingMovingFunc, Func<Location> determineDestinationLocationFunc)
        {
            determineThingMovingFunc.ThrowIfNull(nameof(determineThingMovingFunc));
            determineDestinationLocationFunc.ThrowIfNull(nameof(determineDestinationLocationFunc));

            this.Requestor = creatureRequesting;
            this.GetThingMoving = determineThingMovingFunc;
            this.GetDestinationLocation = determineDestinationLocationFunc;
        }

        /// <summary>
        /// Gets the creature requesting the throw.
        /// </summary>
        public ICreature Requestor { get; }

        /// <summary>
        /// Gets a delegate function to determine the <see cref="IThing"/> being thrown.
        /// </summary>
        public Func<IThing> GetThingMoving { get; }

        /// <summary>
        /// Gets a delegate function to determine the location to where the thing is being thrown.
        /// </summary>
        public Func<Location> GetDestinationLocation { get; }

        /// <inheritdoc/>
        public string ErrorMessage => "You may not throw there.";

        /// <inheritdoc/>
        public bool Evaluate()
        {
            if (this.Requestor == null || !(this.GetThingMoving() is ICreature thingAsCreature))
            {
                // Not a creature requesting this one, possibly a script.
                // Or the thing moving is null, not this policy's job to restrict this...
                return true;
            }

            var locDiff = thingAsCreature.Location - this.GetDestinationLocation();

            return locDiff.Z == 0;
        }
    }
}