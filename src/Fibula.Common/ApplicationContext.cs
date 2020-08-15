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
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Data.Contracts.Abstractions;
    using Fibula.Items.Contracts.Abstractions;
    using Fibula.Security.Contracts;
    using Microsoft.ApplicationInsights;
    using Microsoft.Extensions.Options;

    using IUnitOfWork = Fibula.Data.Contracts.Abstractions.IUnitOfWork<
        Fibula.Data.Contracts.Abstractions.IRepository<Fibula.Data.Entities.Contracts.Abstractions.IAccountEntity>,
        Fibula.Data.Contracts.Abstractions.IRepository<Fibula.Data.Entities.Contracts.Abstractions.ICharacterEntity>,
        Fibula.Data.Contracts.Abstractions.IReadOnlyRepository<Fibula.Data.Entities.Contracts.Abstractions.IMonsterTypeEntity>,
        Fibula.Data.Contracts.Abstractions.IReadOnlyRepository<Fibula.Data.Entities.Contracts.Abstractions.IItemTypeEntity>>;

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
        /// Stores the reference to the monster types loader in use.
        /// </summary>
        private readonly IMonsterTypeLoader monsterTypeLoader;

        /// <summary>
        /// Stores the reference to the item types loader in use.
        /// </summary>
        private readonly IItemTypeLoader itemTypeLoader;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationContext"/> class.
        /// </summary>
        /// <param name="options">A reference to the application configuration.</param>
        /// <param name="rsaDecryptor">A reference to the RSA decryptor in use.</param>
        /// <param name="itemTypeLoader">A reference to the item type loader in use.</param>
        /// <param name="monsterTypeLoader">A reference to the monster type loader in use.</param>
        /// <param name="telemetryClient">A reference to the telemetry client.</param>
        /// <param name="cancellationTokenSource">A reference to the master cancellation token source.</param>
        /// <param name="dbContextGenerationFunc">A reference to a function to generate the database context.</param>
        public ApplicationContext(
            IOptions<ApplicationContextOptions> options,
            IRsaDecryptor rsaDecryptor,
            IItemTypeLoader itemTypeLoader,
            IMonsterTypeLoader monsterTypeLoader,
            TelemetryClient telemetryClient,
            CancellationTokenSource cancellationTokenSource,
            Func<IFibulaDbContext> dbContextGenerationFunc)
        {
            options.ThrowIfNull(nameof(options));
            rsaDecryptor.ThrowIfNull(nameof(rsaDecryptor));
            itemTypeLoader.ThrowIfNull(nameof(itemTypeLoader));
            monsterTypeLoader.ThrowIfNull(nameof(monsterTypeLoader));
            cancellationTokenSource.ThrowIfNull(nameof(cancellationTokenSource));
            dbContextGenerationFunc.ThrowIfNull(nameof(dbContextGenerationFunc));

            DataAnnotationsValidator.ValidateObjectRecursive(options.Value);

            this.Options = options.Value;
            this.RsaDecryptor = rsaDecryptor;
            this.CancellationTokenSource = cancellationTokenSource;

            this.TelemetryClient = telemetryClient;

            this.itemTypeLoader = itemTypeLoader;
            this.monsterTypeLoader = monsterTypeLoader;
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
        /// Creates a new <see cref="IUnitOfWork"/> for data access.
        /// </summary>
        /// <returns>The instance created.</returns>
        public IUnitOfWork CreateNewUnitOfWork()
        {
            return new UnitOfWork(this, this.itemTypeLoader, this.monsterTypeLoader);
        }
    }
}
