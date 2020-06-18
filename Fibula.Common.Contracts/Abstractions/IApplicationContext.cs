// -----------------------------------------------------------------
// <copyright file="IApplicationContext.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common.Contracts.Abstractions
{
    using System.Threading;
    using Fibula.Common.Contracts.Models;
    using Fibula.Data.Contracts.Abstractions;
    using Fibula.Security.Contracts;
    using Microsoft.ApplicationInsights;

    /// <summary>
    /// Interface that represents the common context of the entire application.
    /// </summary>
    public interface IApplicationContext
    {
        /// <summary>
        /// Gets the options for the application.
        /// </summary>
        ApplicationContextOptions Options { get; }

        /// <summary>
        /// Gets the master cancellation token source.
        /// </summary>
        CancellationTokenSource CancellationTokenSource { get; }

        /// <summary>
        /// Gets the Telemetry client in use.
        /// </summary>
        TelemetryClient TelemetryClient { get; }

        /// <summary>
        /// Gets the default database context to use.
        /// </summary>
        IFibulaDbContext DefaultDatabaseContext { get; }

        /// <summary>
        /// Gets the RSA decryptor to use.
        /// </summary>
        IRsaDecryptor RsaDecryptor { get; }
    }
}
