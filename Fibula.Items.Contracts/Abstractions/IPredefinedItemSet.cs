// -----------------------------------------------------------------
// <copyright file="IPredefinedItemSet.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Items.Contracts.Abstractions
{
    using Fibula.Items.Contracts.Enumerations;

    /// <summary>
    /// Interface that defines pre-defined items that are used by the server logic.
    /// </summary>
    public interface IPredefinedItemSet
    {
        /// <summary>
        /// Finds the splatter <see cref="IItemType"/> for a given blood type.
        /// </summary>
        /// <param name="bloodType">The type of blood to look the item type for.</param>
        /// <returns>The <see cref="IItemType"/> that's predefined for that blood type, or null if none is.</returns>
        IItemType FindSplatterForBloodType(BloodType bloodType);

        /// <summary>
        /// Finds the splatter <see cref="IItemType"/> for a given blood type.
        /// </summary>
        /// <param name="bloodType">The type of blood to look the item type for.</param>
        /// <returns>The <see cref="IItemType"/> that's predefined for that blood type, or null if none is.</returns>
        IItemType FindPoolForBloodType(BloodType bloodType);
    }
}
