// -----------------------------------------------------------------
// <copyright file="ThingStateChangedEventArgs.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
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
