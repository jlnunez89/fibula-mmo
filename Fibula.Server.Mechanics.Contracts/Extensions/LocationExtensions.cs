// -----------------------------------------------------------------
// <copyright file="LocationExtensions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts
{
    using System.Linq;
    using Fibula.Common.Utilities;
    using Fibula.Server.Contracts.Enumerations;
    using Fibula.Server.Contracts.Structs;
    using OpenTibia.Server.Contracts.Abstractions;

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
        /// <param name="carrierCreature">The creature that carries the decoded cylinder. Required for locations of type <see cref="LocationType.InsideContainer"/> and <see cref="LocationType.InventorySlot"/>.</param>
        /// <returns>An instance of the target <see cref="ICylinder"/> of the location.</returns>
        public static ICylinder DecodeCyclinder(this Location fromLocation, ITileAccessor tileAccessor, IContainerManager containerManager, out byte index, ICreature carrierCreature = null)
        {
            tileAccessor.ThrowIfNull(nameof(tileAccessor));
            containerManager.ThrowIfNull(nameof(containerManager));

            index = 0;

            if (fromLocation.Type == LocationType.Map && tileAccessor.GetTileAt(fromLocation, out ITile fromTile))
            {
                index = byte.MaxValue;

                return fromTile;
            }
            else if (fromLocation.Type == LocationType.InventorySlot)
            {
                carrierCreature.ThrowIfNull(nameof(carrierCreature));

                index = (byte)fromLocation.Slot;

                // creature?.Inventory[(byte)fromLocation.Slot] as IContainerItem
                return carrierCreature;
            }
            else if (fromLocation.Type == LocationType.InsideContainer)
            {
                carrierCreature.ThrowIfNull(nameof(carrierCreature));

                index = fromLocation.ContainerIndex;

                return containerManager.FindForCreature(carrierCreature.Id, fromLocation.ContainerId);
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
