// -----------------------------------------------------------------
// <copyright file="EventFiredEventArgs.cs" company="2Dudes">
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
    using Fibula.Common.Utilities;
    using Fibula.Scheduling.Contracts.Abstractions;

    /// <summary>
    /// Class that represents the event arguments of an <see cref="EventFiredDelegate"/> event.
    /// </summary>
    public class EventFiredEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventFiredEventArgs"/> class.
        /// </summary>
        /// <param name="evt">The event to include as the event fired.</param>
        public EventFiredEventArgs(IEvent evt)
        {
            evt.ThrowIfNull(nameof(evt));

            this.Event = evt;
        }

        /// <summary>
        /// Gets the event that was fired.
        /// </summary>
        public IEvent Event { get; }
    }
}
