// -----------------------------------------------------------------
// <copyright file="EventFiredDelegate.cs" company="2Dudes">
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
    /// <summary>
    /// Delegate to call when an event is fired.
    /// </summary>
    /// <param name="sender">The sender of the event fired event.</param>
    /// <param name="eventArgs">The event arguments that contain the actual event that was fired.</param>
    public delegate void EventFiredDelegate(object sender, EventFiredEventArgs eventArgs);
}
