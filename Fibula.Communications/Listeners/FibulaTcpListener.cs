// -----------------------------------------------------------------
// <copyright file="FibulaTcpListener.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Listeners
{
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using Fibula.Communications.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a TCP listener.
    /// </summary>
    public class FibulaTcpListener : ITcpListener
    {
        /// <summary>
        /// The TCP listener to use internally.
        /// </summary>
        private readonly TcpListener internalListener;

        /// <summary>
        /// Initializes a new instance of the <see cref="FibulaTcpListener"/> class.
        /// </summary>
        /// <param name="ipAddress">The ip address to listen on.</param>
        /// <param name="port">The port to listen on.</param>
        public FibulaTcpListener(IPAddress ipAddress, ushort port)
        {
            this.internalListener = new TcpListener(ipAddress, port);
        }

        /// <summary>
        /// Accepts a pending connection request as an asynchronous operation.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<Socket> AcceptSocketAsync()
        {
            return await this.internalListener.AcceptSocketAsync();
        }

        /// <summary>
        /// Starts listening for internal connection requests.
        /// </summary>
        public void Start()
        {
            this.internalListener.Start();
        }

        /// <summary>
        /// Closes the listener.
        /// </summary>
        public void Stop()
        {
            this.internalListener.Stop();
        }
    }
}
