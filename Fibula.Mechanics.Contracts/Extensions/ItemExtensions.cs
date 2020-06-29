// -----------------------------------------------------------------
// <copyright file="ItemExtensions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
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
