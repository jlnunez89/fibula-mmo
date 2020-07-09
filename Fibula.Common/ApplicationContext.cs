// -----------------------------------------------------------------
// <copyright file="ApplicationContext.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common
{
    using System;
    using System.Threading;
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Common.Contracts.Models;
    using Fibula.Common.Utilities;
    using Fibula.Data.Contracts.Abstractions;
    using Fibula.Security.Contracts;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector.QuickPulse;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Class that represents the common context of the entire application.
    /// </summary>
    public class ApplicationContext : IApplicationContext
    {
        /// <summary>
        /// The function to generate the context.
        /// </summary>
        private readonly Func<IFibulaDbContext> contextGenerationFunction;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationContext"/> class.
        /// </summary>
        /// <param name="options">A reference to the application configuration.</param>
        /// <param name="telemetryOptions">A reference to the telemetry configuration that will be used to create the telemetry client.</param>
        /// <param name="rsaDecryptor">A reference to the RSA decryptor in use.</param>
        /// <param name="cancellationTokenSource">A reference to the master cancellation token source.</param>
        /// <param name="dbContextGenerationFunc">A reference to a function to generate the database context.</param>
        public ApplicationContext(
            IOptions<ApplicationContextOptions> options,
            IOptions<TelemetryConfiguration> telemetryOptions,
            IRsaDecryptor rsaDecryptor,
            CancellationTokenSource cancellationTokenSource,
            Func<IFibulaDbContext> dbContextGenerationFunc)
        {
            options.ThrowIfNull(nameof(options));
            telemetryOptions.ThrowIfNull(nameof(telemetryOptions));
            rsaDecryptor.ThrowIfNull(nameof(rsaDecryptor));
            cancellationTokenSource.ThrowIfNull(nameof(cancellationTokenSource));
            dbContextGenerationFunc.ThrowIfNull(nameof(dbContextGenerationFunc));

            DataAnnotationsValidator.ValidateObjectRecursive(options.Value);

            this.Options = options.Value;
            this.RsaDecryptor = rsaDecryptor;
            this.CancellationTokenSource = cancellationTokenSource;

            this.TelemetryClient = this.InitializeTelemetry(telemetryOptions.Value);

            this.contextGenerationFunction = dbContextGenerationFunc;
        }

        /// <summary>
        /// Gets the configuration for the application.
        /// </summary>
        public ApplicationContextOptions Options { get; }

        /// <summary>
        /// Gets the RSA decryptor to use.
        /// </summary>
        public IRsaDecryptor RsaDecryptor { get; }

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
        public IFibulaDbContext DefaultDatabaseContext => this.contextGenerationFunction();

        /// <summary>
        /// Initializes the telemetry client with the given configuration.
        /// </summary>
        /// <param name="telemetryConfig">The telemetry configuration.</param>
        /// <returns>The telemetry client.</returns>
        private TelemetryClient InitializeTelemetry(TelemetryConfiguration telemetryConfig)
        {
            QuickPulseTelemetryProcessor processor = null;

            telemetryConfig.TelemetryInitializers.Add(new OperationCorrelationTelemetryInitializer());
            telemetryConfig.TelemetryProcessorChainBuilder
                .Use((next) =>
                {
                    processor = new QuickPulseTelemetryProcessor(next);

                    return processor;
                })
                .Build();

            var quickPulse = new QuickPulseTelemetryModule()
            {
                AuthenticationApiKey = telemetryConfig.InstrumentationKey,
            };

            quickPulse.Initialize(telemetryConfig);
            quickPulse.RegisterTelemetryProcessor(processor);

            return new TelemetryClient(telemetryConfig);
        }
    }
}
