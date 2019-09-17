// -----------------------------------------------------------------
// <copyright file="SimpleDoSDefender.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Security
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using OpenTibia.Security.Contracts;

    /// <summary>
    /// Class that represents a simple DoS defender.
    /// </summary>
    public class SimpleDoSDefender : IDoSDefender, IHostedService
    {
        /// <summary>
        /// To prevent a memory attack... just blacklist a maximum of 1M addresses.
        /// </summary>
        private const int ListSizeLimit = 1000000;

        /// <summary>
        /// The number of seconds per timeframe.
        /// </summary>
        private const int TimeframeInSeconds = 5;

        /// <summary>
        /// Count to reach within a timeframe.
        /// </summary>
        private const int BlockAtCount = 20;

        /// <summary>
        /// The addresses that are blocked.
        /// </summary>
        private readonly HashSet<string> blockedAddresses;

        /// <summary>
        /// The count per connection.
        /// </summary>
        private readonly ConcurrentDictionary<string, int> connectionCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleDoSDefender"/> class.
        /// </summary>
        public SimpleDoSDefender()
        {
            this.blockedAddresses = new HashSet<string>();
            this.connectionCount = new ConcurrentDictionary<string, int>();
        }

        /// <summary>
        /// Starts the defender service.
        /// </summary>
        /// <param name="cancellationToken">A token to observe for cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var cleaningList = this.connectionCount.ToList();

                foreach (var kvp in cleaningList)
                {
                    if (kvp.Value < TimeframeInSeconds)
                    {
                        this.connectionCount.TryRemove(kvp.Key, out int count);
                    }
                    else
                    {
                        this.connectionCount.TryUpdate(kvp.Key, kvp.Value - TimeframeInSeconds, kvp.Value);
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(TimeframeInSeconds));
            }
        }

        /// <summary>
        /// Stops the defender service.
        /// </summary>
        /// <param name="cancellationToken">A token to observe for cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Do nothing here.
            return Task.CompletedTask;
        }

        /// <summary>
        /// Blocks a given address.
        /// </summary>
        /// <param name="addressStr">The address to block.</param>
        public void BlockAddress(string addressStr)
        {
            if (this.blockedAddresses.Count >= ListSizeLimit || string.IsNullOrWhiteSpace(addressStr))
            {
                return;
            }

            this.AddInternal(addressStr);
        }

        /// <summary>
        /// Checks if a given address is blocked.
        /// </summary>
        /// <param name="addressStr">The address to check for.</param>
        /// <returns>True if the address is blocked, false otherwise.</returns>
        public bool IsBlocked(string addressStr)
        {
            return this.blockedAddresses.Contains(addressStr);
        }

        /// <summary>
        /// Logs a connection attempt.
        /// </summary>
        /// <param name="addressStr">The address from which the connection attempt took place.</param>
        public void LogConnectionAttempt(string addressStr)
        {
            this.connectionCount.AddOrUpdate(addressStr, 0, (key, prev) => { return prev + 1; });

            try
            {
                if (this.connectionCount[addressStr] == BlockAtCount)
                {
                    this.AddInternal(addressStr);

                    this.connectionCount.TryRemove(addressStr, out int count);
                }
            }
            catch
            {
                // happens if the key was removed exactly at the time we were querying. Just ignore.
            }
        }

        /// <summary>
        /// Adds an address to the blocked addresses list.
        /// </summary>
        /// <param name="addressStr">The address to add.</param>
        private void AddInternal(string addressStr)
        {
            try
            {
                this.blockedAddresses.Add(addressStr);
            }
            catch
            {
                // this will be thrown if there is already an element in there, so just ignore.
            }
        }
    }
}
