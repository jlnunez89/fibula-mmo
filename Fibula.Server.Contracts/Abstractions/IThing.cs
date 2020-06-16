// -----------------------------------------------------------------
// <copyright file="IThing.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Contracts.Abstractions
{
    /// <summary>
    /// Interface for all things in the game.
    /// </summary>
    public interface IThing : ILocatable, IContainedThing
    {
        ///// <summary>
        ///// Event to invoke when any of the properties of this thing have changed.
        ///// </summary>
        // TODO: move to mechanics.
        //event OnThingStateChanged ThingChanged;

        /// <summary>
        /// Gets the id of this thing.
        /// </summary>
        ushort ThingId { get; }

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
