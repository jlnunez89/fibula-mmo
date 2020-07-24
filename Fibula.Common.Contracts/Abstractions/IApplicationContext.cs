// -----------------------------------------------------------------
// <copyright file="IApplicationContext.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common.Contracts.Abstractions
{
    using System.Threading;
    using Fibula.Common.Contracts.Models;
    using Fibula.Data.Contracts.Abstractions;
    using Fibula.Security.Contracts;
    using Microsoft.ApplicationInsights;

    using IUnitOfWork = Fibula.Data.Contracts.Abstractions.IUnitOfWork<
        Fibula.Data.Contracts.Abstractions.IRepository<Fibula.Data.Entities.Contracts.Abstractions.IAccountEntity>,
        Fibula.Data.Contracts.Abstractions.IRepository<Fibula.Data.Entities.Contracts.Abstractions.ICharacterEntity>,
        Fibula.Data.Contracts.Abstractions.IReadOnlyRepository<Fibula.Data.Entities.Contracts.Abstractions.IMonsterTypeEntity>,
        Fibula.Data.Contracts.Abstractions.IReadOnlyRepository<Fibula.Data.Entities.Contracts.Abstractions.IItemTypeEntity>>;

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

        /// <summary>
        /// Creates a new <see cref="IUnitOfWork"/> for data access.
        /// </summary>
        /// <returns>The instance created.</returns>
        IUnitOfWork CreateNewUnitOfWork();
    }
}
