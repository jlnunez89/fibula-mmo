// -----------------------------------------------------------------
// <copyright file="IUnitOfWork.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Data.Contracts.Abstractions
{
    using System;
    using Fibula.Data.Entities.Contracts.Abstractions;

    /// <summary>
    /// Interface for units of work that target the OpenTibia project.
    /// </summary>
    /// <typeparam name="TAccounts"></typeparam>
    /// <typeparam name="TCharacters"></typeparam>
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
