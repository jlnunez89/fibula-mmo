// -----------------------------------------------------------------
// <copyright file="ContainerIsOpenEventCondition.cs" company="2Dudes">
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

    /// <summary>
    /// Class that represents an event condition that evaluates whether a container is opened by a player.
    /// </summary>
    internal class ContainerIsOpenEventCondition : IEventCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerIsOpenEventCondition"/> class.
        /// </summary>
        /// <param name="determineTargetCreatureFunc">A function delegate to determine the target creature.</param>
        /// <param name="containerId">The id of the container to check.</param>
        public ContainerIsOpenEventCondition(Func<ICreature> determineTargetCreatureFunc, byte containerId)
        {
            determineTargetCreatureFunc.ThrowIfNull(nameof(determineTargetCreatureFunc));

            this.GetTargetCreature = determineTargetCreatureFunc;
            this.ContainerId = containerId;
        }

        /// <summary>
        /// Gets a delegate function to determine the <see cref="ICreature"/> to check.
        /// </summary>
        public Func<ICreature> GetTargetCreature { get; }

        /// <summary>
        /// Gets the id of the container being checked.
        /// </summary>
        public byte ContainerId { get; }

        /// <inheritdoc/>
        public string ErrorMessage => "The destination container is out of reach.";

        /// <inheritdoc/>
        public bool Evaluate()
        {
            var targetCreature = this.GetTargetCreature();

            if (targetCreature == null || !(targetCreature is IPlayer player))
            {
                return false;
            }

            return player.GetContainerById(this.ContainerId) != null;
        }
    }
}