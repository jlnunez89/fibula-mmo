// -----------------------------------------------------------------
// <copyright file="ThingStateChangedEventArgs.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server
{
    using System;
    using OpenTibia.Server.Contracts.Abstractions;

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