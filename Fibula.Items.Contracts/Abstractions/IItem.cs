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
        /// <summary>
        /// The maximum amount value of cummulative items.
        /// </summary>
        public const byte MaximumAmountOfCummulativeItems = 100;

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

        bool ChangesOnUse { get; }

        ushort ChangeOnUseTo { get; }

        bool CanBeRotated { get; }

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

        bool IsCumulative { get; }

        bool IsContainer { get; }

        bool CanBeDressed { get; }

        Slot DressPosition { get; }

        // byte DressPosition { get; }

        // byte Attack { get; }

        // byte Defense { get; }

        // byte Armor { get; }

        // int Range { get; }

        // decimal Weight { get; }

        ///// <summary>
        ///// Gets the creature carrying this item, if any.
        ///// </summary>
        //ICreature Carrier { get; }

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
