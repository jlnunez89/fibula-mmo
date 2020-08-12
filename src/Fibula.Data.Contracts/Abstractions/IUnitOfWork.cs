// -----------------------------------------------------------------
// <copyright file="IUnitOfWork.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Data.Contracts.Abstractions
{
    using System;
    using Fibula.Data.Entities.Contracts.Abstractions;

    /// <summary>
    /// Interface for units of work that target the Fibula project.
    /// </summary>
    /// <typeparam name="TAccountsRepository">The type of accounts repository.</typeparam>
    /// <typeparam name="TCharactersRepository">The type of characters repository.</typeparam>
    /// <typeparam name="TMonsterTypesRepository">The type of monster types repository.</typeparam>
    /// <typeparam name="TItemTypesRepository">The type of item types repository.</typeparam>
    public interface IUnitOfWork
        <out TAccountsRepository,
        out TCharactersRepository,
        out TMonsterTypesRepository,
        out TItemTypesRepository> : IDisposable
        where TAccountsRepository : IRepository<IAccountEntity>
        where TCharactersRepository : IRepository<ICharacterEntity>
        where TMonsterTypesRepository : IReadOnlyRepository<IMonsterTypeEntity>
        where TItemTypesRepository : IReadOnlyRepository<IItemTypeEntity>
    {
        /// <summary>
        /// Gets the repository of accounts.
        /// </summary>
        TAccountsRepository Accounts { get; }

        /// <summary>
        /// Gets the repository of characters.
        /// </summary>
        TCharactersRepository Characters { get; }

        /// <summary>
        /// Gets the repository of monster types.
        /// </summary>
        TMonsterTypesRepository MonsterTypes { get; }

        /// <summary>
        /// Gets the repository of item types.
        /// </summary>
        TItemTypesRepository ItemTypes { get; }

        /// <summary>
        /// Saves all changes made during this unit of work to the persistent store.
        /// </summary>
        /// <returns>The number of changes saved.</returns>
        int Complete();
    }
}
