// -----------------------------------------------------------------
// <copyright file="ICreatureWithExhaustion.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Contracts.Abstractions
{
    using System;
    using System.Collections.Generic;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Enumerations;

    /// <summary>
    /// Interface for any <see cref="ICreature"/> in the game that can suffer from exhaustion and thus, has cooldown periods.
    /// </summary>
    public interface ICreatureWithExhaustion : ICreature
    {
        /// <summary>
        /// Calculates the remaining <see cref="TimeSpan"/> until the entity's exhaustion is recovered from.
        /// </summary>
        /// <param name="type">The type of exhaustion.</param>
        /// <param name="fromTime">The current time to calculate from.</param>
        /// <returns>The <see cref="TimeSpan"/> result.</returns>
        TimeSpan CalculateRemainingCooldownTime(ExhaustionType type, DateTimeOffset fromTime);

        /// <summary>
        /// Adds exhaustion of the given type.
        /// </summary>
        /// <param name="type">The type of exhaustion to add.</param>
        /// <param name="fromTime">The reference time from which to add.</param>
        /// <param name="timeSpan">The amount of time to add exhaustion for.</param>
        void AddExhaustion(ExhaustionType type, DateTimeOffset fromTime, TimeSpan timeSpan);
    }
}
