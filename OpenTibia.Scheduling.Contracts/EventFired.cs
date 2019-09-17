﻿// -----------------------------------------------------------------
// <copyright file="EventFired.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Scheduling.Contracts
{
    /// <summary>
    /// Delegate for when an event is fired.
    /// </summary>
    /// <param name="sender">The sender of the event fired event.</param>
    /// <param name="eventArgs">The event arguments that contain the actual event that was fired.</param>
    public delegate void EventFired(object sender, EventFiredEventArgs eventArgs);
}
