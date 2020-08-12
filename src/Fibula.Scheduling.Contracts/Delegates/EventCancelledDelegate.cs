// -----------------------------------------------------------------
// <copyright file="EventCancelledDelegate.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Scheduling.Contracts.Delegates
{
    using Fibula.Scheduling.Contracts.Abstractions;

    /// <summary>
    /// Delegate to call when an event is cancelled.
    /// </summary>
    /// <param name="evt">The event that was cancelled.</param>
    /// <returns>True if the event is successfully cancelled, false otherwise.</returns>
    public delegate bool EventCancelledDelegate(IEvent evt);
}
