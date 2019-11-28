// -----------------------------------------------------------------
// <copyright file="CanThrowBetweenEventCondition.cs" company="2Dudes">
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
    /// Class that represents a condition that evaluates whether a throw from A to B can be performed.
    /// </summary>
    internal class CanThrowBetweenEventCondition : IEventCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CanThrowBetweenEventCondition"/> class.
        /// </summary>
        /// <param name="creatureFinder">A reference to the creature finder in use.</param>
        /// <param name="game">A reference to the game instance.</param>
        /// <param name="requestorId">The id of the creature requesting the throw.</param>
        /// <param name="fromLocation">The start location.</param>
        /// <param name="toLocation">The end location.</param>
        /// <param name="checkLineOfSight">Whether or not to check the line of sight.</param>
        public CanThrowBetweenEventCondition(ICreatureFinder creatureFinder, IGame game, uint requestorId, Location fromLocation, Location toLocation, bool checkLineOfSight = true)
        {
            creatureFinder.ThrowIfNull(nameof(creatureFinder));

            this.CreatureFinder = creatureFinder;
            this.Game = game;
            this.RequestorId = requestorId;
            this.FromLocation = fromLocation;
            this.ToLocation = toLocation;
            this.CheckLineOfSight = checkLineOfSight;
        }

        /// <summary>
        /// Gets the reference to the creature finder.
        /// </summary>
        public ICreatureFinder CreatureFinder { get; }

        /// <summary>
        /// Gets the reference to the game instance.
        /// </summary>
        public IGame Game { get; }

        /// <summary>
        /// Gets the start location of the throw.
        /// </summary>
        public Location FromLocation { get; }

        /// <summary>
        /// Gets the end location of the throw.
        /// </summary>
        public Location ToLocation { get; }

        /// <summary>
        /// Gets a value indicating whether the line of sight should be checked.
        /// </summary>
        public bool CheckLineOfSight { get; }

        /// <summary>
        /// Gets the id of the creature requesting the throw.
        /// </summary>
        public uint RequestorId { get; }

        /// <inheritdoc/>
        public string ErrorMessage => "You may not throw there.";

        /// <inheritdoc/>
        public bool Evaluate()
        {
            var requestor = this.RequestorId == 0 ? null : this.CreatureFinder.FindCreatureById(this.RequestorId);

            if (requestor == null)
            {
                // Empty requestorId means not a creature generated event... possibly a script.
                return true;
            }

            return true; // this.Game.CanThrowBetween(this.FromLocation, this.ToLocation, this.CheckLineOfSight);
        }
    }
}