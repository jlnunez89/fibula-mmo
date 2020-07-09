// -----------------------------------------------------------------
// <copyright file="EventExpeditedDelegate.cs" company="2Dudes">
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
    /// Delegate to call when an event is processed earlier than the scheduler intended.
    /// </summary>
    /// <param name="sender">The sender of the event processed event.</param>
    /// <returns>True if the event is successfully expedited, false otherwise.</returns>
    public delegate bool EventExpeditedDelegate(IEvent sender);
}
