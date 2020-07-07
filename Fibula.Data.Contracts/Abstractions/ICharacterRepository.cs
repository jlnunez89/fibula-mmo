// -----------------------------------------------------------------
// <copyright file="ICharacterRepository.cs" company="2Dudes">
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
    using Fibula.Data.Entities.Contracts.Abstractions;

    /// <summary>
    /// Interface for a character repository.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    public interface ICharacterRepository<TEntity> : IRepository<TEntity>
        where TEntity : ICharacterEntity
    {
    }
}
