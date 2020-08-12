// -----------------------------------------------------------------
// <copyright file="IItem.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Items.Contracts.Abstractions
{
    using System;
    using System.Collections.Generic;
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Data.Entities.Contracts.Abstractions;
    using Fibula.Items.Contracts.Enumerations;

    /// <summary>
    /// Interface for all items in the game.
    /// </summary>
    public interface IItem : IThing, IContainedThing
    {
        /// <summary>
        /// Gets a reference to this item's <see cref="IItemTypeEntity"/>.
        /// </summary>
        IItemTypeEntity Type { get; }

        /// <summary>
        /// Gets the attributes of this item.
        /// </summary>
        IDictionary<ItemAttribute, IConvertible> Attributes { get; }

        /// <summary>
        /// Gets or sets the amount of this item.
        /// </summary>
        byte Amount { get; set; }

        /// <summary>
        /// Gets a value indicating whether this item is ground floor.
        /// </summary>
        bool IsGround { get; }

        /// <summary>
        /// Gets the movement cost for walking over this item, assuming it <see cref="IsGround"/>.
        /// </summary>
        byte MovementPenalty { get; }

        /// <summary>
        /// Gets a value indicating whether this item stays on top of the stack.
        /// </summary>
        bool StaysOnTop { get; }

        /// <summary>
        /// Gets a value indicating whether this item stays on the bottom of the stack.
        /// </summary>
        bool StaysOnBottom { get; }

        /// <summary>
        /// Gets a value indicating whether this item changes on use.
        /// </summary>
        bool ChangesOnUse { get; }

        /// <summary>
        /// Gets the Id of the item into which this will change upon use.
        /// Callers must check <see cref="ChangesOnUse"/> to verify this item does indeed have a target.
        /// </summary>
        ushort ChangeOnUseTo { get; }

        /// <summary>
        /// Gets a value indicating whether this item can be rotated.
        /// </summary>
        bool CanBeRotated { get; }

        /// <summary>
        /// Gets the Id of the item into which this will rotate to.
        /// Callers must check <see cref="CanBeRotated"/> to verify this item does indeed have a target.
        /// </summary>
        ushort RotateTo { get; }

        /// <summary>
        /// Gets a value indicating whether this item expires.
        /// </summary>
        bool HasExpiration { get; }

        /// <summary>
        /// Gets the time left before this item expires, if it <see cref="HasExpiration"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">When the item does not expire.</exception>
        TimeSpan ExpirationTimeLeft { get; }

        /// <summary>
        /// Gets the Id of the item into which this will expire to, if it <see cref="HasExpiration"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">When the item does not expire.</exception>
        ushort ExpirationTarget { get; }

        /// <summary>
        /// Gets a value indicating whether this item is a liquid pool.
        /// </summary>
        bool IsLiquidPool { get; }

        /// <summary>
        /// Gets a value indicating whether this item is a liquid source.
        /// </summary>
        bool IsLiquidSource { get; }

        /// <summary>
        /// Gets a value indicating whether this item is a liquid container.
        /// </summary>
        bool IsLiquidContainer { get; }

        /// <summary>
        /// Gets the type of liquid in this item, assuming it: <see cref="IsLiquidPool"/>, <see cref="IsLiquidSource"/>, or <see cref="IsLiquidContainer"/>.
        /// </summary>
        LiquidType LiquidType { get; }

        /// <summary>
        /// Gets a value indicating whether the item blocks throwing through it.
        /// </summary>
        bool BlocksThrow { get; }

        /// <summary>
        /// Gets a value indicating whether the item blocks walking on it.
        /// </summary>
        bool BlocksPass { get; }

        /// <summary>
        /// Gets a value indicating whether the item blocks laying anything on it.
        /// </summary>
        bool BlocksLay { get; }

        /// <summary>
        /// Gets a value indicating whether this item can be accumulated.
        /// </summary>
        bool IsCumulative { get; }

        /// <summary>
        /// Gets a value indicating whether this item is a container.
        /// </summary>
        bool IsContainer { get; }

        /// <summary>
        /// Gets a value indicating whether this item can be dressed.
        /// </summary>
        bool CanBeDressed { get; }

        /// <summary>
        /// Gets the position at which the item can be dressed.
        /// </summary>
        Slot DressPosition { get; }

        /// <summary>
        /// Gets a value indicating whether this item triggers a collision event.
        /// </summary>
        bool HasCollision { get; }

        /// <summary>
        /// Gets a value indicating whether this item triggers a separation event.
        /// </summary>
        bool HasSeparation { get; }

        /// <summary>
        /// Gets a value indicating whether this item is clipped to the ground.
        /// </summary>
        bool IsGroundFix { get; }

        /// <summary>
        /// Attempts to join an item to this item's content at the default index.
        /// </summary>
        /// <param name="otherItem">The item to join with.</param>
        /// <returns>True if the operation was successful, false otherwise. Along with any surplus of the item after merge.</returns>
        (bool success, IItem surplusItem) Merge(IItem otherItem);

        /// <summary>
        /// Attempts to split this item into two based on the amount provided.
        /// </summary>
        /// <param name="itemFactory">A reference to the item factory in use.</param>
        /// <param name="amount">The amount of the item to split.</param>
        /// <returns>True if the operation was successful, false otherwise, along with the item produced, if any.</returns>
        (bool success, IItem itemProduced) Split(IItemFactory itemFactory, byte amount);

        /// <summary>
        /// Determines if this item is blocks pathfinding.
        /// </summary>
        /// <param name="avoidTypes">The damage types to avoid when checking for path blocking. By default, all types are considered path blocking.</param>
        /// <returns>True if the tile is considered path blocking, false otherwise.</returns>
        bool IsPathBlocking(byte avoidTypes = (byte)AvoidDamageType.All);
    }
}
