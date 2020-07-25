// -----------------------------------------------------------------
// <copyright file="ItemTypeEntityExtensions.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Items.Contracts.Extensions
{
    using Fibula.Common.Utilities;
    using Fibula.Data.Entities.Contracts.Abstractions;
    using Fibula.Items.Contracts.Enumerations;

    /// <summary>
    /// Static class that contains extension methods for <see cref="IMonsterTypeEntity"/>s.
    /// </summary>
    public static class ItemTypeEntityExtensions
    {
        /// <summary>
        /// Checks if the item type has the given item flag set.
        /// </summary>
        /// <param name="itemTypeEntity">The item type entity.</param>
        /// <param name="itemFlag">The item flag to check for.</param>
        /// <returns>True if the item type has the item flag set, and false otherwise.</returns>
        public static bool HasItemFlag(this IItemTypeEntity itemTypeEntity, ItemFlag itemFlag)
        {
            itemTypeEntity.ThrowIfNull(nameof(itemTypeEntity));

            return (itemTypeEntity.Flags & (ulong)itemFlag) == (ulong)itemFlag;
        }
    }
}
