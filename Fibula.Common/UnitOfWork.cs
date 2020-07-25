// -----------------------------------------------------------------
// <copyright file="UnitOfWork.cs" company="2Dudes">
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
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Data.Repositories;
    using Fibula.Items.Contracts.Abstractions;
    using Microsoft.EntityFrameworkCore;

    using IUnitOfWork = Fibula.Data.Contracts.Abstractions.IUnitOfWork<
        Fibula.Data.Repositories.AccountRepository,
        Fibula.Data.Repositories.CharacterRepository,
        Fibula.Data.Repositories.MonsterTypeReadOnlyRepository,
        Fibula.Data.Repositories.ItemTypeReadOnlyRepository>;

    /// <summary>
    /// Class that represents a unit of work for the Fibula project.
    /// All operations that persist data to whatever the underlying storage is, should be done within the same
    /// unit of work and persisted at the end of it by invoking <see cref="Complete"/>.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// A reference to the underlying database context on this unit of work.
        /// </summary>
        private readonly IApplicationContext applicationContext;

        /// <summary>
        /// Stores the lazy respository instance for accounts.
        /// </summary>
        private readonly Lazy<AccountRepository> accounts;

        /// <summary>
        /// Stores the lazy respository instance for accounts.
        /// </summary>
        private readonly Lazy<CharacterRepository> characters;

        /// <summary>
        /// Stores a locking object used to prevent double initialization of the <see cref="databaseContext"/>.
        /// </summary>
        private readonly object databaseContextLock;

        /// <summary>
        /// Stores the database context in use by this unit of work.
        /// </summary>
        private DbContext databaseContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        /// <param name="applicationContext">The application context to work in.</param>
        /// <param name="itemTypeLoader">A reference to the item type loader in use.</param>
        /// <param name="monsterTypeLoader">A reference to the monster type loader in use.</param>
        public UnitOfWork(IApplicationContext applicationContext, IItemTypeLoader itemTypeLoader, IMonsterTypeLoader monsterTypeLoader)
        {
            applicationContext.ThrowIfNull(nameof(applicationContext));

            this.applicationContext = applicationContext;

            this.databaseContextLock = new object();

            this.accounts = new Lazy<AccountRepository>(
                () =>
                {
                    this.GetOrInitializeDbContext();

                    return new AccountRepository(this.databaseContext);
                },
                System.Threading.LazyThreadSafetyMode.None);

            this.characters = new Lazy<CharacterRepository>(
                () =>
                {
                    this.GetOrInitializeDbContext();

                    return new CharacterRepository(this.databaseContext);
                },
                System.Threading.LazyThreadSafetyMode.None);

            this.MonsterTypes = new MonsterTypeReadOnlyRepository(monsterTypeLoader);

            this.ItemTypes = new ItemTypeReadOnlyRepository(itemTypeLoader);
        }

        /// <summary>
        /// Gets a reference to the accounts repository.
        /// </summary>
        public AccountRepository Accounts => this.accounts.Value;

        /// <summary>
        /// Gets a reference to the characters repository.
        /// </summary>
        public CharacterRepository Characters => this.characters.Value;

        /// <summary>
        /// Gets a reference to the monster types repository.
        /// </summary>
        public MonsterTypeReadOnlyRepository MonsterTypes { get; }

        /// <summary>
        /// Gets a reference to the item types repository.
        /// </summary>
        public ItemTypeReadOnlyRepository ItemTypes { get; }

        /// <summary>
        /// Completes this unit of work.
        /// </summary>
        /// <returns>The number of changes saved upon completion of this unit of work.</returns>
        public int Complete()
        {
            if (this.databaseContext != null)
            {
                return this.databaseContext.SaveChanges();
            }

            return 0;
        }

        /// <summary>
        /// Disposes this unit of work and it's resources.
        /// </summary>
        public void Dispose()
        {
            if (this.databaseContext != null)
            {
                lock (this.databaseContextLock)
                {
                    if (this.databaseContext != null)
                    {
                        this.databaseContext.Dispose();
                        this.databaseContext = null;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the current unit of work's db context, or performs initialization of one if none is defined.
        /// </summary>
        private void GetOrInitializeDbContext()
        {
            if (this.databaseContext == null)
            {
                lock (this.databaseContextLock)
                {
                    if (this.databaseContext == null)
                    {
                        this.databaseContext = this.applicationContext.DefaultDatabaseContext.AsDbContext();
                        this.databaseContext.Database.EnsureCreatedAsync().Wait();
                    }
                }
            }
        }
    }
}
