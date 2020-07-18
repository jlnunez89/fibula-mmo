// -----------------------------------------------------------------
// <copyright file="IThing.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common.Contracts.Abstractions
{
    using System;
    using Fibula.Common.Contracts.Delegates;

    /// <summary>
    /// Interface for all things in the game.
    /// </summary>
    public interface IThing : ILocatable, IContainedThing, IEquatable<IThing>
    {
        /// <summary>
        /// Event to invoke when any of the properties of this thing have changed.
        /// </summary>
        event OnThingStateChanged ThingChanged;

        /// <summary>
        /// Gets the id of this thing.
        /// </summary>
        ushort ThingId { get; }

        /// <summary>
        /// Gets the unique id of this item.
        /// </summary>
        Guid UniqueId { get; }

        /// <summary>
        /// Gets a value indicating whether this thing can be moved.
        /// </summary>
        bool CanBeMoved { get; }

        /// <summary>
        /// Provides a string describing the current thing for logging purposes.
        /// </summary>
        /// <returns>The string to log.</returns>
        string DescribeForLogger();
    }
}
