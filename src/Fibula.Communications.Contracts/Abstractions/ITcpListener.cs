// -----------------------------------------------------------------
// <copyright file="ITcpListener.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
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
