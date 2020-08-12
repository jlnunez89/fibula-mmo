// -----------------------------------------------------------------
// <copyright file="IStat.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Creatures.Contracts.Abstractions
{
    using Fibula.Creatures.Contracts.Enumerations;
    using Fibula.Mechanics.Contracts.Delegates;

    /// <summary>
    /// Interface for stats in the game.
    /// </summary>
    public interface IStat
    {
        /// <summary>
        /// Event triggered when this stat changes.
        /// </summary>
        event OnStatChanged Changed;

        /// <summary>
        /// Gets this stat's type.
        /// </summary>
        CreatureStat Type { get; }

        /// <summary>
        /// Gets this stat's current value.
        /// </summary>
        uint Current { get; }

        /// <summary>
        /// Gets this stat's maximum value.
        /// </summary>
        uint Maximum { get; }

        /// <summary>
        /// Gets the current percentual value between current and maximum values.
        /// </summary>
        byte Percent { get; }

        /// <summary>
        /// Increases this stats's value.
        /// </summary>
        /// <param name="value">The amount by which to increase this stat's value.</param>
        /// <returns>True if the value was actually increased, false otherwise.</returns>
        bool Increase(int value);

        /// <summary>
        /// Decreases this stats's value.
        /// </summary>
        /// <param name="value">The amount by which to decrease this stat's value.</param>
        /// <returns>True if the value was actually decreased, false otherwise.</returns>
        bool Decrease(int value);

        /// <summary>
        /// Sets this stats's value.
        /// </summary>
        /// <param name="value">The value to set in the stat. This is bounded by [0, <see cref="Maximum"/>].</param>
        /// <returns>True if the value was actually changed, false otherwise.</returns>
        bool Set(uint value);
    }
}
