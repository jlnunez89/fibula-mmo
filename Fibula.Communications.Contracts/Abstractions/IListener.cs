// -----------------------------------------------------------------
// <copyright file="IListener.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Contracts.Abstractions
{
    using Fibula.Communications.Contracts.Delegates;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Common interface of all listeners.
    /// </summary>
    public interface IListener : IHostedService
    {
        /// <summary>
        /// Event fired when a new connection is enstablished.
        /// </summary>
        event NewConnectionDelegate NewConnection;
    }
}