// -----------------------------------------------------------------
// <copyright file="EventFiredDelegate.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
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
