// -----------------------------------------------------------------
// <copyright file="SampleHostedService.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Standalone
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    internal class SampleHostedService : IHostedService, IDisposable
    {
        private readonly ILogger logger;

        public SampleHostedService(ILogger<SampleHostedService> logger)
        {
            this.logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Background Service is starting.");

            return Task.Factory.StartNew(() => this.DoWork(null));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Background Service is stopping.");

            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }

        private void DoWork(object state)
        {
            this.logger.LogInformation("Background Service is working.");
        }
    }
}
