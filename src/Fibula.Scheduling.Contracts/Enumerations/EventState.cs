// -----------------------------------------------------------------
// <copyright file="EventState.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Scheduling.Contracts.Enumerations
{
    /// <summary>
    /// Enumerates all the possible event states.
    /// </summary>
    public enum EventState
    {
        /// <summary>
        /// The event has not been scheduled yet.
        /// </summary>
        Created,

        /// <summary>
        /// The event has been scheduled and is waiting to be executed.
        /// </summary>
        Scheduled,

        /// <summary>
        /// The event was cancelled.
        /// </summary>
        Cancelled,

        /// <summary>
        /// The event was processed to completion.
        /// </summary>
        Completed,
    }
}
