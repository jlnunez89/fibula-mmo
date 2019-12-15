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
    using System;
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
        /// <param name="requestor">The requestor creature.</param>
        /// <param name="determineSourceLocationFunc">A function to determine the source location from where the move is happening.</param>
        public RequestorIsInRangeToMoveEventCondition(ICreature requestor, Func<Location> determineSourceLocationFunc)
        {
            determineSourceLocationFunc.ThrowIfNull(nameof(determineSourceLocationFunc));

            this.Requestor = requestor;
            this.GetSourceLocation = determineSourceLocationFunc;
        }

        /// <summary>
        /// Gets the requesting creature.
        /// </summary>
        public ICreature Requestor { get; }

        /// <summary>
        /// Gets the delegate function to determine the location to check.
        /// </summary>
        public Func<Location> GetSourceLocation { get; }

        /// <inheritdoc/>
        public string ErrorMessage => "You are too far away.";

        /// <inheritdoc/>
        public bool Evaluate()
        {
            if (this.Requestor == null)
            {
                // script called, probably
                return true;
            }

            return (this.Requestor.Location - this.GetSourceLocation()).MaxValueIn2D <= 1;
        }
    }
}