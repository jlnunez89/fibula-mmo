// -----------------------------------------------------------------
// <copyright file="ITcpListener.cs" company="2Dudes">
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
    using System.Net.Sockets;
    using System.Threading.Tasks;

    /// <summary>
    /// Common interface for TCP listeners.
    /// </summary>
    public interface ITcpListener
    {
        /// <summary>
        /// Starts listening for internal connection requests.
        /// </summary>
        void Start();

        /// <summary>
        /// Closes the listener.
        /// </summary>
        void Stop();

        /// <summary>
        /// Accepts a pending connection request as an asynchronous operation.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<Socket> AcceptSocketAsync();
    }
}
