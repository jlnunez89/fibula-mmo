// -----------------------------------------------------------------
// <copyright file="ApplicationContext.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
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
        /// <param name="dataAnnotationsValidator">A reference to the data annotations validator in use.</param>
        /// <param name="rsaDecryptor">A reference to the RSA decryptor in use.</param>
        /// <param name="cancellationTokenSource">A reference to the master cancellation token source.</param>
        /// <param name="telemetryClient">A reference to the telemetry client.</param>
        /// <param name="openTibiaDbContextGenerationFunc">A reference to a function to generate the OpenTibia database context.</param>
        public ApplicationContext(
            IOptions<ApplicationContextOptions> options,
            IDataAnnotationsValidator dataAnnotationsValidator,
            IRsaDecryptor rsaDecryptor,
            CancellationTokenSource cancellationTokenSource,
            TelemetryClient telemetryClient,
            Func<IFibulaDbContext> openTibiaDbContextGenerationFunc)
        {
            options.ThrowIfNull(nameof(options));
            dataAnnotationsValidator.ThrowIfNull(nameof(dataAnnotationsValidator));
            rsaDecryptor.ThrowIfNull(nameof(rsaDecryptor));
            cancellationTokenSource.ThrowIfNull(nameof(cancellationTokenSource));
            telemetryClient.ThrowIfNull(nameof(telemetryClient));
            openTibiaDbContextGenerationFunc.ThrowIfNull(nameof(openTibiaDbContextGenerationFunc));

            dataAnnotationsValidator.ValidateObjectRecursive(options.Value);

            this.Options = options.Value;
            this.Validator = dataAnnotationsValidator;
            this.RsaDecryptor = rsaDecryptor;
            this.CancellationTokenSource = cancellationTokenSource;
            this.TelemetryClient = telemetryClient;

            this.contextGenerationFunction = openTibiaDbContextGenerationFunc;
        }

        /// <summary>
        /// Gets the configuration for the application.
        /// </summary>
        public ApplicationContextOptions Options { get; }

        /// <summary>
        /// Gets the validator to use.
        /// </summary>
        public IDataAnnotationsValidator Validator { get; }

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
    }
}
