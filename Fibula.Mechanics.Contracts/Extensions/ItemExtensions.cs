// -----------------------------------------------------------------
// <copyright file="ItemExtensions.cs" company="2Dudes">
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
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Items.Contracts.Abstractions;

    /// <summary>
    /// Helper class that provides extensions for the <see cref="IItem"/> implementations.
    /// </summary>
    public static class ItemExtensions
    {
        /// <summary>
        /// Attempts to get the creature carrying this item, if any.
        /// </summary>
        /// <param name="item">The item to find the carrier for.</param>
        /// <returns>The carrier of the item, if any.</returns>
        public static ICreature GetCarrier(this IItem item)
        {
            item.ThrowIfNull(nameof(item));

            // Find if there is a parent container that is a creature.
            IThingContainer container = item.ParentContainer;

            while (container != null)
            {
                if (container is ICreature creatureCarrier)
                {
                    return creatureCarrier;
                }

                if (container is IContainedThing containedContainer)
                {
                    container = containedContainer.ParentContainer;
                }
            }

            return null;
        }
    }
}
