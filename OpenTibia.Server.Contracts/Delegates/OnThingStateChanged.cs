// -----------------------------------------------------------------
// <copyright file="OnThingStateChanged.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Delegates
{
    using OpenTibia.Server.Contracts.Abstractions;

    /// <summary>
    /// Delegate for <see cref="IThing"/> state changing.
    /// </summary>
    /// <param name="thingThatChanged">A reference to the <see cref="IThing"/> that changed.</param>
    /// <param name="eventArgs">The event arguments.</param>
    public delegate void OnThingStateChanged(IThing thingThatChanged, ThingStateChangedEventArgs eventArgs);
}
