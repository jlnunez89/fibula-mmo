// -----------------------------------------------------------------
// <copyright file="ThingCanBeMovedCondition.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Operations.Conditions
{
    using OpenTibia.Scheduling.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Abstractions;

    /// <summary>
    /// Class that represents an event condition that evaluates whether a thing can be moved.
    /// </summary>
    public class ThingCanBeMovedCondition : IEventCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThingCanBeMovedCondition"/> class.
        /// </summary>
        /// <param name="requestor">The creature requesting the move.</param>
        /// <param name="thing">The thing to check for.</param>
        public ThingCanBeMovedCondition(ICreature requestor, IThing thing)
        {
            this.Requestor = requestor;
            this.Thing = thing;
        }

        /// <summary>
        /// Gets the creature requesting the move.
        /// </summary>
        public ICreature Requestor { get; }

        /// <summary>
        /// Gets the <see cref="IThing"/> to check.
        /// </summary>
        public IThing Thing { get; }

        /// <inheritdoc/>
        public string ErrorMessage => "You may not move that.";

        /// <inheritdoc/>
        public bool Evaluate()
        {
            if (this.Thing == null)
            {
                return false;
            }

            return this.Thing.CanBeMoved || this.Thing == this.Requestor;
        }
    }
}