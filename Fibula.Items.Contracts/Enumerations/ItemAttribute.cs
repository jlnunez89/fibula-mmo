// -----------------------------------------------------------------
// <copyright file="ItemAttribute.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Items.Contracts.Enumerations
{
    /// <summary>
    /// Enumerates all known item attributes.
    /// </summary>
    public enum ItemAttribute
    {
        /// <summary>
        /// Tracks the amount for a cumulative item.
        /// </summary>
        Amount,

        /// <summary>
        /// The capacity available in a container item.
        /// </summary>
        Capacity,

        /// <summary>
        /// The id of the item to change to on use.
        /// </summary>
        ChangeOnUseTo,

        /// <summary>
        /// The type of liquid that a liquid container, pool or source is of.
        /// </summary>
        LiquidType,

        /// <summary>
        /// The types of damage to avoid.
        /// </summary>
        DamageTypesToAvoid,

        /// <summary>
        /// The id of the item to disguise as.
        /// </summary>
        DisguiseAs,

        /// <summary>
        /// The position at which a dressable item can go.
        /// </summary>
        DressPosition,

        /// <summary>
        /// The movement penalty that a ground item has.
        /// </summary>
        MovementPenalty,

        /// <summary>
        /// The id of the item to rotate to.
        /// </summary>
        RotateTo,

        /// <summary>
        /// The content of a readable item.
        /// </summary>
        Text,

        /// <summary>
        /// The range that a readable item can be read within.
        /// </summary>
        ReadRange,
    }
}
