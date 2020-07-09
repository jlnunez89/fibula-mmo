// -----------------------------------------------------------------
// <copyright file="IRepository.cs" company="2Dudes">
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
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Fibula.Data.Entities.Contracts.Abstractions;

    /// <summary>
    /// Interface for a generic entity repository.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    public interface IRepository<TEntity>
        where TEntity : IIdentifiableEntity
    {
        /// <summary>
        /// Gets a single entity matching the id supplied.
        /// </summary>
        /// <param name="id">The id to search the entity by.</param>
        /// <returns>The entity that matched the id supplied.</returns>
        TEntity GetById(string id);

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
        IEnumerable<TEntity> FindMany(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Finds an entity in the set within the context that satisfies an expression.
        /// If more than one entity satisfies the expression, one is picked up in an unknown criteria.
        /// </summary>
        /// <param name="predicate">The expression to satisfy.</param>
        /// <returns>The entity found.</returns>
        TEntity FindOne(Expression<Func<TEntity, bool>> predicate);

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
