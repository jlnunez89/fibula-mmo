// -----------------------------------------------------------------
// <copyright file="LocationExtensions.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Contracts.Extensions
{
    using System.Linq;
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Contracts.Structs;
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Items.Contracts.Abstractions;
    using Fibula.Map.Contracts.Abstractions;

    /// <summary>
    /// Helper class for extension methods of locations.
    /// </summary>
    public static class LocationExtensions
    {
        /// <summary>
        /// Gets a thing container from a location.
        /// </summary>
        /// <param name="fromLocation">The location from which to decode the container information.</param>
        /// <param name="map">A reference to the map.</param>
        /// <param name="containerManager">A reference to the container manager.</param>
        /// <param name="index">The index within the cyclinder to target.</param>
        /// <param name="carrierCreature">The creature that carries the decoded container. Required for locations of type <see cref="LocationType.InsideContainer"/> and <see cref="LocationType.InventorySlot"/>.</param>
        /// <returns>An instance of the target <see cref="IThingContainer"/> of the location.</returns>
        public static IThingContainer DecodeContainer(this Location fromLocation, IMap map, IContainerManager containerManager, out byte index, ICreature carrierCreature = null)
        {
            map.ThrowIfNull(nameof(map));
            containerManager.ThrowIfNull(nameof(containerManager));

            index = 0;

            if (fromLocation.Type == LocationType.Map && map.GetTileAt(fromLocation, out ITile fromTile))
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
        /// <param name="map">A reference to the map.</param>
        /// <param name="containerManager">A reference to the container manager.</param>
        /// <param name="typeId">The type id of the item to look for.</param>
        /// <param name="creature">Optional. The creature that the location's cyclinder targets, if any.</param>
        /// <returns>An item instance, if found at the location.</returns>
        public static IItem FindItemByTypeId(this Location atLocation, IMap map, IContainerManager containerManager, ushort typeId, ICreature creature = null)
        {
            map.ThrowIfNull(nameof(map));
            containerManager.ThrowIfNull(nameof(containerManager));

            switch (atLocation.Type)
            {
                case LocationType.Map:
                    // Using an item from the ground (map).
                    if (!map.GetTileAt(atLocation, out ITile tile))
                    {
                        return null;
                    }

                    return tile.FindItemWithTypeId(typeId);
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
