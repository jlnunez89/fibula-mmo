// -----------------------------------------------------------------
// <copyright file="ThingStateChangedEventArgs.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common.Contracts
{
    using System;
    using Fibula.Common.Contracts.Abstractions;

    /// <summary>
    /// Class that represents event arguments for a <see cref="IThing"/> state changed event.
    /// </summary>
    public class ThingStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the property that changed within the <see cref="IThing"/>.
        /// </summary>
        public string PropertyChanged { get; set; }
    }
}
