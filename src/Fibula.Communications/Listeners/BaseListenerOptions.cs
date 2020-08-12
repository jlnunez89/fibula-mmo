// -----------------------------------------------------------------
// <copyright file="BaseListenerOptions.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Listeners
{
    /// <summary>
    /// Class that represents options for any of the listeners.
    /// </summary>
    public abstract class BaseListenerOptions
    {
        /// <summary>
        /// Gets or sets the port to listen to.
        /// </summary>
        public abstract ushort? Port { get; set; }
    }
}
