// -----------------------------------------------------------------
// <copyright file="LocationExtensions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts
{
    using System.Linq;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Helper class for extension methods of locations.
    /// </summary>
    public static class LocationExtensions
    {
        /// <summary>
        /// Gets a cylinder from a location.
        /// </summary>
        /// <param name="fromLocation">The location from which to decode the cylinder information.</param>
        /// <param name="tileAccessor">A reference to the tile accessor.</param>
        /// <param name="containerManager">A reference to the container manager.</param>
        /// <param name="index">The index within the cyclinder to target.</param>
        /// <param name="subIndex">The sub-index within the cylinder to target.</param>
        /// <param name="creature">Optional. The creature that owns the target cylinder to target.</param>
        /// <returns>An instance of the target <see cref="ICylinder"/> of the location.</returns>
        public static ICylinder GetCyclinder(this Location fromLocation, ITileAccessor tileAccessor, IContainerManager containerManager, ref byte index, ref byte subIndex, ICreature creature = null)
        {
            tileAccessor.ThrowIfNull(nameof(tileAccessor));
            containerManager.ThrowIfNull(nameof(containerManager));

            if (fromLocation.Type == LocationType.Map && tileAccessor.GetTileAt(fromLocation, out ITile fromTile))
            {
                return fromTile;
            }
            else if (fromLocation.Type == LocationType.InsideContainer)
            {
                index = fromLocation.ContainerId;
                subIndex = fromLocation.ContainerIndex;

                return containerManager.FindForCreature(creature.Id, fromLocation.ContainerId);
            }
            else if (fromLocation.Type == LocationType.InventorySlot)
            {
                index = 0;
                subIndex = 0;

                return creature?.Inventory[(byte)fromLocation.Slot] as IContainerItem;
            }

            return null;
        }

        /// <summary>
        /// Attempts to find an item at the given location.
        /// </summary>
        /// <param name="atLocation">The location at which to look for the item.</param>
        /// <param name="tileAccessor">A reference to the tile accessor.</param>
        /// <param name="containerManager">A reference to the container manager.</param>
        /// <param name="typeId">The type id of the item to look for.</param>
        /// <param name="creature">Optional. The creature that the location's cyclinder targets, if any.</param>
        /// <returns>An item instance, if found at the location.</returns>
        public static IItem FindItemById(this Location atLocation, ITileAccessor tileAccessor, IContainerManager containerManager, ushort typeId, ICreature creature = null)
        {
            tileAccessor.ThrowIfNull(nameof(tileAccessor));
            containerManager.ThrowIfNull(nameof(containerManager));

            switch (atLocation.Type)
            {
                case LocationType.Map:
                    // Using an item from the ground (map).
                    if (!tileAccessor.GetTileAt(atLocation, out ITile tile))
                    {
                        return null;
                    }

                    return tile.FindItemWithId(typeId);
                case LocationType.InventorySlot:
                    var fromBodyContainer = creature?.Inventory[(byte)atLocation.Slot] as IContainerItem;

                    return fromBodyContainer?.Content.FirstOrDefault();
                case LocationType.InsideContainer:
                    var fromContainer = containerManager.FindForCreature(creature.Id, atLocation.ContainerId);

                    return fromContainer?[atLocation.ContainerIndex];
            }

            return null;
        }
    }
}
