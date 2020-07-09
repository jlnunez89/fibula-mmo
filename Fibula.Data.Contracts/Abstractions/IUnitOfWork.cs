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
    /// <typeparam name="TAccounts">The type of account entities.</typeparam>
    /// <typeparam name="TCharacters">The type of character entities.</typeparam>
    public interface IUnitOfWork
        <TAccounts,
        TCharacters> : IDisposable
        where TAccounts : IAccountEntity
        where TCharacters : ICharacterEntity
    {
        /// <summary>
        /// Gets the repository of accounts.
        /// </summary>
        IAccountRepository<TAccounts> Accounts { get; }

        /// <summary>
        /// Gets the repository of characters.
        /// </summary>
        ICharacterRepository<TCharacters> Characters { get; }

        /// <summary>
        /// Saves all changes made during this unit of work to the persistent store.
        /// </summary>
        /// <returns>The number of changes saved.</returns>
        int Complete();
    }
}
