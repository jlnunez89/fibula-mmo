// -----------------------------------------------------------------
// <copyright file="EventDelayedDelegate.cs" company="2Dudes">
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
    using System;
    using Fibula.Scheduling.Contracts.Abstractions;

    /// <summary>
    /// Delegate to call when an event is processed later than the scheduler intended.
    /// </summary>
    /// <param name="sender">The sender of the event processed event.</param>
    /// <param name="delayTime">The time by which to delay the event.</param>
    /// <returns>True if the event is successfully delayed, false otherwise.</returns>
    public delegate bool EventDelayedDelegate(IEvent sender, TimeSpan delayTime);
}
