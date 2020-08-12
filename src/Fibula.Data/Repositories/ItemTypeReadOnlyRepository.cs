// -----------------------------------------------------------------
// <copyright file="ItemTypeReadOnlyRepository.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Fibula.Common.Utilities;
    using Fibula.Data.Contracts.Abstractions;
    using Fibula.Data.Entities.Contracts.Abstractions;
    using Fibula.Items.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a read-only repository for item types.
    /// </summary>
    public class ItemTypeReadOnlyRepository : IReadOnlyRepository<IItemTypeEntity>
    {
        /// <summary>
        /// A locking object to prevent double initialization of the catalog.
        /// </summary>
        private static readonly object ItemTypeCatalogLock = new object();

        /// <summary>
        /// Stores the map between the item type ids and the actual item types.
        /// </summary>
        private static IDictionary<ushort, IItemTypeEntity> itemTypeCatalog;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemTypeReadOnlyRepository"/> class.
        /// </summary>
        /// <param name="itemTypeLoader">A reference to the item type loader in use.</param>
        public ItemTypeReadOnlyRepository(IItemTypeLoader itemTypeLoader)
        {
            itemTypeLoader.ThrowIfNull(nameof(itemTypeLoader));

            if (itemTypeCatalog == null)
            {
                lock (ItemTypeCatalogLock)
                {
                    if (itemTypeCatalog == null)
                    {
                        itemTypeCatalog = itemTypeLoader.LoadTypes();
                    }
                }
            }
        }

        /// <summary>
        /// Finds all the entities in the set within the context that satisfy an expression.
        /// </summary>
        /// <param name="predicate">The expression to satisfy.</param>
        /// <returns>The collection of entities retrieved.</returns>
        public IEnumerable<IItemTypeEntity> FindMany(Expression<Func<IItemTypeEntity, bool>> predicate)
        {
            return itemTypeCatalog.Values.AsQueryable().Where(predicate);
        }

        /// <summary>
        /// Finds an entity in the set within the context that satisfies an expression.
        /// If more than one entity satisfies the expression, one is picked up in an unknown criteria.
        /// </summary>
        /// <param name="predicate">The expression to satisfy.</param>
        /// <returns>The entity found.</returns>
        public IItemTypeEntity FindOne(Expression<Func<IItemTypeEntity, bool>> predicate)
        {
            return itemTypeCatalog.Values.AsQueryable().FirstOrDefault(predicate);
        }

        /// <summary>
        /// Gets all the entities from the set in the context.
        /// </summary>
        /// <returns>The collection of entities retrieved.</returns>
        public IEnumerable<IItemTypeEntity> GetAll()
        {
            return itemTypeCatalog.Values;
        }

        /// <summary>
        /// Gets an entity that matches an id, from the context.
        /// </summary>
        /// <param name="id">The id to match.</param>
        /// <returns>The entity found, if any.</returns>
        public IItemTypeEntity GetById(string id)
        {
            if (ushort.TryParse(id, out ushort raceId) && itemTypeCatalog.ContainsKey(raceId))
            {
                return itemTypeCatalog[raceId];
            }

            return null;
        }
    }
}
