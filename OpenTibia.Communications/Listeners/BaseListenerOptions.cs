// -----------------------------------------------------------------
// <copyright file="BaseListenerOptions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications
{
    /// <summary>
    /// Class that represents options for any of the listeners.
    /// </summary>
    public abstract class BaseListenerOptions
    {
        /// <summary>
        /// Gets or sets the port to listen to.
        /// </summary>
        public abstract ushort Port { get; set; }
    }
}
