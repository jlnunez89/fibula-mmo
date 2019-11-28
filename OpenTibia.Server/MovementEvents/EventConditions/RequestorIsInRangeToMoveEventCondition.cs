// -----------------------------------------------------------------
// <copyright file="RequestorIsInRangeToMoveEventCondition.cs" company="2Dudes">
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
    /// Class that represents an event condition that evaluates whether a requestor is in range to move from a location.
    /// </summary>
    internal class RequestorIsInRangeToMoveEventCondition : IEventCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestorIsInRangeToMoveEventCondition"/> class.
        /// </summary>
        /// <param name="creatureFinder">A reference to the creature finder in use.</param>
        /// <param name="requestorId">The id of the requestor creature.</param>
        /// <param name="movingFrom">The location from where the move is happening.</param>
        public RequestorIsInRangeToMoveEventCondition(ICreatureFinder creatureFinder, uint requestorId, Location movingFrom)
        {
            creatureFinder.ThrowIfNull(nameof(creatureFinder));

            this.CreatureFinder = creatureFinder;
            this.RequestorId = requestorId;
            this.FromLocation = movingFrom;
        }

        /// <summary>
        /// Gets the reference to the creature finder.
        /// </summary>
        public ICreatureFinder CreatureFinder { get; }

        /// <summary>
        /// Gets the id of the requesting creature.
        /// </summary>
        public uint RequestorId { get; }

        /// <summary>
        /// Gets the location to check.
        /// </summary>
        public Location FromLocation { get; }

        /// <inheritdoc/>
        public string ErrorMessage => "You are too far away.";

        /// <inheritdoc/>
        public bool Evaluate()
        {
            var requestor = this.RequestorId == 0 ? null : this.CreatureFinder.FindCreatureById(this.RequestorId);

            if (requestor == null)
            {
                // script called, probably
                return true;
            }

            return (requestor.Location - this.FromLocation).MaxValueIn2D <= 1;
        }
    }
}