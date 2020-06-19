// -----------------------------------------------------------------
// <copyright file="IItem.cs" company="2Dudes">
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
    using System;
    using System.Collections.Generic;
    using Fibula.Items.Contracts.Enumerations;
    using Fibula.Parsing.Contracts.Abstractions;
    using Fibula.Server.Contracts.Abstractions;
    using Fibula.Server.Contracts.Enumerations;
    using Serilog;

    /// <summary>
    /// Interface for all items in the game.
    /// </summary>
    public interface IItem : IThing, IContainedThing
    {
        // event ItemHolderChangeEvent OnHolderChanged;

        // event ItemAmountChangeEvent OnAmountChanged;

        /// <summary>
        /// Gets the unique id of this item.
        /// </summary>
        Guid UniqueId { get; }

        /// <summary>
        /// Gets a reference to this item's <see cref="IItemType"/>.
        /// </summary>
        IItemType Type { get; }

        /// <summary>
        /// Gets the attributes of this item.
        /// </summary>
        IDictionary<ItemAttribute, IConvertible> Attributes { get; }

        /// <summary>
        /// Gets a value indicating whether this item is ground floor.
        /// </summary>
        bool IsGround { get; }

        byte MovementPenalty { get; }

        bool StaysOnTop { get; }

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

        bool IsLiquidPool { get; }

        bool IsLiquidSource { get; }

        bool IsLiquidContainer { get; }

        LiquidType LiquidType { get; }

        bool HasCollision { get; }

        bool HasSeparation { get; }

        bool BlocksThrow { get; }

        bool BlocksPass { get; }

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

        Slot DressPosition { get; }

        // byte DressPosition { get; }

        // byte Attack { get; }

        // byte Defense { get; }

        // byte Armor { get; }

        // int Range { get; }

        // decimal Weight { get; }

        /// <summary>
        /// Gets the amount of this item.
        /// </summary>
        byte Amount { get; }

        void SetAmount(byte remainingCount);

        void SetAttributes(ILogger logger, IItemFactory itemFactory, IList<IParsedAttribute> attributes);

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

        bool IsPathBlocking(byte avoidTypes = (byte)AvoidDamageType.All);
    }
}
