// -----------------------------------------------------------------
// <copyright file="OnThingStateChanged.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common.Contracts.Delegates
{
    using Fibula.Common.Contracts;
    using Fibula.Common.Contracts.Abstractions;

    /// <summary>
    /// Delegate for <see cref="IThing"/> state changing.
    /// </summary>
    /// <param name="thingThatChanged">A reference to the <see cref="IThing"/> that changed.</param>
    /// <param name="eventArgs">The event arguments.</param>
    public delegate void OnThingStateChanged(IThing thingThatChanged, ThingStateChangedEventArgs eventArgs);
}
