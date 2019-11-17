// -----------------------------------------------------------------
// <copyright file="IApplicationContext.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Common.Contracts.Abstractions
{
    using System.Threading;
    using Microsoft.ApplicationInsights;
    using Microsoft.Extensions.Configuration;
    using OpenTibia.Data.Contracts.Abstractions;

    /// <summary>
    /// Interface that represents the common context of the entire application.
    /// </summary>
    public interface IApplicationContext
    {
        /// <summary>
        /// Gets the configuration in use.
        /// </summary>
        IConfiguration Configuration { get; }

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
        IOpenTibiaDbContext DefaultDatabaseContext { get; }
    }
}
