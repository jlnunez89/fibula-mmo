// -----------------------------------------------------------------
// <copyright file="TileNodeCachingFactory.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.PathFinding.AStar
{
    using System;
    using System.Collections.Generic;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Common.Utilities.Pathfinding;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Class that represents a tile node factory that caches the created nodes based on the creation arguments.
    /// </summary>
    internal class TileNodeCachingFactory : INodeFactory
    {
        private readonly ITileAccessor tileAccessor;

        /// <summary>
        /// Stores an object that acts as a semaphore for the <see cref="nodesDictionary"/>.
        /// </summary>
        private readonly object nodesDictionaryLock;

        /// <summary>
        /// Stores the mapping for nodes in a search, and their locations.
        /// </summary>
        private readonly IDictionary<string, IDictionary<Location, TileNode>> nodesDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="TileNodeCachingFactory"/> class.
        /// </summary>
        /// <param name="tileAccessor">A reference to the tile accessor in use.</param>
        public TileNodeCachingFactory(ITileAccessor tileAccessor)
        {
            this.tileAccessor = tileAccessor;

            this.nodesDictionaryLock = new object();
            this.nodesDictionary = new Dictionary<string, IDictionary<Location, TileNode>>();
        }

        /// <summary>
        /// Creates a node belonging to the given <paramref name="searchId"/>, using the given <paramref name="nodeCreationData"/>.
        /// </summary>
        /// <param name="searchId">The search id.</param>
        /// <param name="nodeCreationData">The node creation data.</param>
        /// <returns>An instance of a <see cref="INode"/>.</returns>
        public INode Create(string searchId, INodeCreationArguments nodeCreationData)
        {
            searchId.ThrowIfNullOrWhiteSpace(nameof(searchId));
            nodeCreationData.ThrowIfNull(nameof(nodeCreationData));

            if (!(nodeCreationData is TileNodeCreationArguments tileNodeArguments))
            {
                throw new ArgumentException($"{nameof(nodeCreationData)} must be of type {nameof(TileNodeCreationArguments)}.", nameof(nodeCreationData));
            }

            var locToSearch = tileNodeArguments.Location;

            lock (this.nodesDictionaryLock)
            {
                if (!this.nodesDictionary.ContainsKey(searchId))
                {
                    this.nodesDictionary.Add(searchId, new Dictionary<Location, TileNode>());
                }

                if (!this.nodesDictionary[searchId].ContainsKey(locToSearch) && this.tileAccessor.GetTileAt(tileNodeArguments.Location, out ITile tile))
                {
                    this.nodesDictionary[searchId].Add(locToSearch, new TileNode(searchId, tile, tileNodeArguments.OnBehalfOfCreature));
                }

                return this.nodesDictionary[searchId].TryGetValue(locToSearch, out TileNode node) ? node : null;
            }
        }
    }
}
