// -----------------------------------------------------------------
// <copyright file="UnpassItemsInRangeEventCondition.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Class that represents an event condition that evaluates whether an item with the unpass flag is being moved within range.
    /// </summary>
    internal class UnpassItemsInRangeEventCondition : IEventCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnpassItemsInRangeEventCondition"/> class.
        /// </summary>
        /// <param name="mover">The creature requesting the move.</param>
        /// <param name="determineThingMovingFunc">A delegate function to determine the thing that is being moved.</param>
        /// <param name="determineTargetLocationFunc">A function to determine the target location to check.</param>
        public UnpassItemsInRangeEventCondition(ICreature mover, Func<IThing> determineThingMovingFunc, Func<Location> determineTargetLocationFunc)
        {
            determineThingMovingFunc.ThrowIfNull(nameof(determineThingMovingFunc));
            determineTargetLocationFunc.ThrowIfNull(nameof(determineTargetLocationFunc));

            this.Mover = mover;
            this.GetThing = determineThingMovingFunc;
            this.GetTargetLocation = determineTargetLocationFunc;
        }

        /// <summary>
        /// Gets the mover.
        /// </summary>
        public ICreature Mover { get; }

        /// <summary>
        /// Gets the delegate function to determine the target location.
        /// </summary>
        public Func<Location> GetTargetLocation { get; }

        /// <summary>
        /// Gets the delegate function to determine the <see cref="IThing"/> being moved.
        /// </summary>
        public Func<IThing> GetThing { get; }

        /// <inheritdoc/>
        public string ErrorMessage => "Sorry, not possible.";

        /// <inheritdoc/>
        public bool Evaluate()
        {
            var thing = this.GetThing();

            if (this.Mover == null || !(thing is IItem item) || !item.Type.Flags.Contains(ItemFlag.Unpass))
            {
                // MoverId being null means this is probably a script's action.
                // Policy does not apply to this thing.
                return true;
            }

            var locDiff = this.Mover.Location - this.GetTargetLocation();

            return locDiff.Z == 0 && locDiff.MaxValueIn2D <= 2;
        }
    }
}