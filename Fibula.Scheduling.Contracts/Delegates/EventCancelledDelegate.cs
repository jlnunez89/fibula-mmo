// -----------------------------------------------------------------
// <copyright file="EventCancelledDelegate.cs" company="2Dudes">
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
    using Fibula.Scheduling.Contracts.Abstractions;

    /// <summary>
    /// Delegate to call when an event is cancelled.
    /// </summary>
    /// <param name="evt">The event that was cancelled.</param>
    /// <returns>True if the event is successfully cancelled, false otherwise.</returns>
    public delegate bool EventCancelledDelegate(IEvent evt);
}
