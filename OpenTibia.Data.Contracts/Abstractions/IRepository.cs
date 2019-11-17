// -----------------------------------------------------------------
// <copyright file="IRepository.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Data.Contracts.Abstractions
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using OpenTibia.Data.Entities;

    /// <summary>
    /// Interface for a generic entity repository.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    public interface IRepository<TEntity>
        where TEntity : BaseEntity
    {
        /// <summary>
        /// Gets a single entity matching the id supplied.
        /// </summary>
        /// <param name="id">The id to search the entity by.</param>
        /// <returns>The entity that matched the id supplied.</returns>
        TEntity Get(string id);

        /// <summary>
        /// Gets a collection of all entities from a type.
        /// </summary>
        /// <returns>The collection of entities.</returns>
        IEnumerable<TEntity> GetAll();

        /// <summary>
        /// Finds all entities that match a predicate.
        /// </summary>
        /// <param name="predicate">The predicate to use for matching.</param>
        /// <returns>The entities that matched the predicate.</returns>
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Adds an entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        void Add(TEntity entity);

        /// <summary>
        /// Adds multiple entities to the respository.
        /// </summary>
        /// <param name="entities">The entities to add.</param>
        void AddRange(IEnumerable<TEntity> entities);

        /// <summary>
        /// Removes an entity from the repository.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        void Remove(TEntity entity);

        /// <summary>
        /// Removes multiple entities from the repository.
        /// </summary>
        /// <param name="entities">The entities to remove.</param>
        void RemoveRange(IEnumerable<TEntity> entities);
    }
}
