// -----------------------------------------------------------------
// <copyright file="ApplicationContext.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Common
{
    using System;
    using System.Threading;
    using Microsoft.ApplicationInsights;
    using Microsoft.Extensions.Configuration;
    using OpenTibia.Common.Contracts.Abstractions;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Data.Contracts.Abstractions;

    /// <summary>
    /// Class that represents the common context of the entire application.
    /// </summary>
    public class ApplicationContext : IApplicationContext
    {
        /// <summary>
        /// The function to generate the context.
        /// </summary>
        private readonly Func<IOpenTibiaDbContext> contextGenerationFunction;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationContext"/> class.
        /// </summary>
        /// <param name="configuration">A reference to the application configuration.</param>
        /// <param name="cancellationTokenSource">A reference to the master cancellation token source.</param>
        /// <param name="telemetryClient">A reference to the telemetry client.</param>
        /// <param name="openTibiaDbContextGenerationFunc">A reference to a function to generate the OpenTibia database context.</param>
        public ApplicationContext(IConfiguration configuration, CancellationTokenSource cancellationTokenSource, TelemetryClient telemetryClient, Func<IOpenTibiaDbContext> openTibiaDbContextGenerationFunc)
        {
            configuration.ThrowIfNull(nameof(configuration));
            cancellationTokenSource.ThrowIfNull(nameof(cancellationTokenSource));
            telemetryClient.ThrowIfNull(nameof(telemetryClient));
            openTibiaDbContextGenerationFunc.ThrowIfNull(nameof(openTibiaDbContextGenerationFunc));

            this.Configuration = configuration;
            this.CancellationTokenSource = cancellationTokenSource;
            this.TelemetryClient = telemetryClient;

            this.contextGenerationFunction = openTibiaDbContextGenerationFunc;
        }

        /// <summary>
        /// Gets the configuration in use.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Gets the master cancellation token source.
        /// </summary>
        public CancellationTokenSource CancellationTokenSource { get; }

        /// <summary>
        /// Gets the Telemetry client in use.
        /// </summary>
        public TelemetryClient TelemetryClient { get; }

        /// <summary>
        /// Gets the default database context to use.
        /// </summary>
        public IOpenTibiaDbContext DefaultDatabaseContext => this.contextGenerationFunction();
    }
}
