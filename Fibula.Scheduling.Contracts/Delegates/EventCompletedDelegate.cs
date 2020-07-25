// -----------------------------------------------------------------
// <copyright file="EventCompletedDelegate.cs" company="2Dudes">
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
    /// Delegate to call when an event is processed to completion (after no more repeats).
    /// </summary>
    /// <param name="evt">The event that was completed.</param>
    public delegate void EventCompletedDelegate(IEvent evt);
}
