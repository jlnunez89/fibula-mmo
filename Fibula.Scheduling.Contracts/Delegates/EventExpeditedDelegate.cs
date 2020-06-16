﻿// -----------------------------------------------------------------
// <copyright file="EventExpeditedDelegate.cs" company="2Dudes">
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
    /// Delegate to call when an event is processed earlier than the scheduler intended.
    /// </summary>
    /// <param name="sender">The sender of the event processed event.</param>
    /// <returns>True if the event is successfully expedited, false otherwise.</returns>
    public delegate bool EventExpeditedDelegate(IEvent sender);
}
