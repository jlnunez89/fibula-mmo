// -----------------------------------------------------------------
// <copyright file="IEvent.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Scheduling.Contracts.Abstractions
{
    /// <summary>
    /// Interface that represents an event.
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// Gets a unique identifier for this event.
        /// </summary>
        string EventId { get; }

        /// <summary>
        /// Gets the id of the requestor of this event, if available.
        /// </summary>
        uint RequestorId { get; }

        /// <summary>
        /// Gets the error message that should be bubbled back to the player if the event cannot be executed.
        /// </summary>
        string ErrorMessage { get; }

        /// <summary>
        /// Executes the event logic.
        /// </summary>
        void Execute();
    }
}