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

namespace Fibula.Data
{
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Data.Contracts.Abstractions;
    using Fibula.Data.Repositories;
    using Microsoft.EntityFrameworkCore;

    using IUnitOfWork = Fibula.Data.Contracts.Abstractions.IUnitOfWork<
        Fibula.Data.Repositories.AccountRepository,
        Fibula.Data.Repositories.CharacterRepository,
        Fibula.Data.Repositories.MonsterTypeReadOnlyRepository>;

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
        private readonly DbContext databaseContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        /// <param name="context">The context to work on.</param>
        /// <param name="monsterTypeLoader">A reference to the monster type loader in use.</param>
        public UnitOfWork(IFibulaDbContext context, IMonsterTypeLoader monsterTypeLoader)
        {
            context.ThrowIfNull(nameof(context));

            this.databaseContext = context.AsDbContext();
            this.databaseContext.Database.EnsureCreatedAsync().Wait();

            this.Accounts = new AccountRepository(this.databaseContext);
            this.Characters = new CharacterRepository(this.databaseContext);
            this.Monsters = new MonsterTypeReadOnlyRepository(monsterTypeLoader);
        }

        /// <summary>
        /// Gets a reference to the accounts repository.
        /// </summary>
        public AccountRepository Accounts { get; }

        /// <summary>
        /// Gets a reference to the characters repository.
        /// </summary>
        public CharacterRepository Characters { get; }

        /// <summary>
        /// Gets a reference to the monsters repository.
        /// </summary>
        public MonsterTypeReadOnlyRepository Monsters { get; }

        /// <summary>
        /// Completes this unit of work.
        /// </summary>
        /// <returns>The number of changes saved upon completion of this unit of work.</returns>
        public int Complete()
        {
            return this.databaseContext.SaveChanges();
        }

        /// <summary>
        /// Disposes this unit of work and it's resources.
        /// </summary>
        public void Dispose()
        {
            this.databaseContext.Dispose();
        }
    }
}
