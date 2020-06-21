// -----------------------------------------------------------------
// <copyright file="SimpleDoSDefender.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Security
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Fibula.Common.Utilities;
    using Fibula.Security.Contracts;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Class that represents a simple DoS defender that will block addresses but not unblock them (until the application is restarted, since state is not persisted).
    /// This is helpful only with high connection attempt counts.
    /// </summary>
    public class SimpleDosDefender : IDoSDefender, IHostedService
    {
        /// <summary>
        /// The addresses that are blocked.
        /// </summary>
        private readonly ISet<string> blockedAddresses;

        /// <summary>
        /// A lock object to semaphore access to <see cref="connectionCount"/>.
        /// </summary>
        private readonly object connectionCountLock;

        /// <summary>
        /// The count per connection.
        /// </summary>
        private readonly IDictionary<string, int> connectionCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleDosDefender"/> class.
        /// </summary>
        /// <param name="options">The options for this defender.</param>
        public SimpleDosDefender(IOptions<SimpleDosDefenderOptions> options)
        {
            options.ThrowIfNull(nameof(options));

            DataAnnotationsValidator.ValidateObjectRecursive(options.Value);

            this.Options = options.Value;

            this.blockedAddresses = new HashSet<string>();

            this.connectionCountLock = new object();
            this.connectionCount = new Dictionary<string, int>();
        }

        /// <summary>
        /// Gets the options set for this defender.
        /// </summary>
        public SimpleDosDefenderOptions Options { get; }

        /// <summary>
        /// Starts the defender service.
        /// </summary>
        /// <param name="cancellationToken">A token to observe for cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    lock (this.connectionCountLock)
                    {
                        foreach (var kvp in this.connectionCount.ToList())
                        {
                            if (kvp.Value < this.Options.TimeframeInSeconds)
                            {
                                this.connectionCount.Remove(kvp.Key);
                            }
                            else
                            {
                                this.connectionCount[kvp.Key] = kvp.Value - this.Options.TimeframeInSeconds;
                            }
                        }
                    }

                    await Task.Delay(TimeSpan.FromSeconds(this.Options.TimeframeInSeconds));
                }
            });

            // return this to allow other IHostedService-s to start.
            return Task.CompletedTask;
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
            if (this.blockedAddresses.Count >= this.Options.ListSizeLimit || string.IsNullOrWhiteSpace(addressStr))
            {
                return;
            }

            this.blockedAddresses.Add(addressStr);
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
            lock (this.connectionCountLock)
            {
                if (!this.connectionCount.ContainsKey(addressStr))
                {
                    this.connectionCount.Add(addressStr, 0);
                }

                this.connectionCount[addressStr] += 1;

                if (this.connectionCount[addressStr] == this.Options.BlockAtCount)
                {
                    this.blockedAddresses.Add(addressStr);

                    this.connectionCount.Remove(addressStr);
                }
            }
        }
    }
}
