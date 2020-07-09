// -----------------------------------------------------------------
// <copyright file="ItemAttribute.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
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
