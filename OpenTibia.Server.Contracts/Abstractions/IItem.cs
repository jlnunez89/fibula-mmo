// -----------------------------------------------------------------
// <copyright file="IItem.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Abstractions
{
    using System;
    using System.Collections.Generic;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Parsing.Contracts.Abstractions;
    using Serilog;

    /// <summary>
    /// Interface for all items in the game.
    /// </summary>
    public interface IItem : IThing
    {
        /// <summary>
        /// The maximum amount value of cummulative items.
        /// </summary>
        public const byte MaximumAmountOfCummulativeItems = 100;

        // event ItemHolderChangeEvent OnHolderChanged;

        // event ItemAmountChangeEvent OnAmountChanged;

        /// <summary>
        /// Gets a reference to this item's <see cref="IItemType"/>.
        /// </summary>
        IItemType Type { get; }

        /// <summary>
        /// Gets the attributes of this item.
        /// </summary>
        IDictionary<ItemAttribute, IConvertible> Attributes { get; }

        ///// <summary>
        ///// Gets the id of the creature holding this item, if any.
        ///// </summary>
        // uint HolderId { get; }

        IContainerItem ParentContainer { get; }

        /// <summary>
        /// Gets a value indicating whether this item is ground floor.
        /// </summary>
        bool IsGround { get; }

        byte MovementPenalty { get; }

        bool StaysOnTop { get; }

        bool StaysOnBottom { get; }

        bool ChangesOnUse { get; }

        ushort ChangeOnUseTo { get; }

        bool IsLiquidPool { get; }

        bool IsLiquidSource { get; }

        bool IsLiquidContainer { get; }

        byte LiquidType { get; }

        bool HasCollision { get; }

        bool HasSeparation { get; }

        bool BlocksThrow { get; }

        bool BlocksPass { get; }

        bool BlocksLay { get; }

        bool IsCumulative { get; }

        bool IsContainer { get; }

        bool IsDressable { get; }

        // byte DressPosition { get; }

        // byte Attack { get; }

        // byte Defense { get; }

        // byte Armor { get; }

        // int Range { get; }

        // decimal Weight { get; }

        byte Amount { get; }

        void SetAmount(byte remainingCount);

        void SetAttributes(ILogger logger, IItemFactory itemFactory, IList<IParsedAttribute> attributes);

        (bool success, IItem remainderItem) JoinWith(IItemFactory itemFactory, IItem otherItem);

        (bool success, IItem remainderItem) SeparateFrom(IItemFactory itemFactory, byte amount);

        // bool IsPathBlocking(byte avoidType = 0);

        // void AddContent(IEnumerable<object> content);

        // void SetHolder(ICreature creature, Location fromLocation);

        // void SetParent(IContainerItem parentContainer);

        // bool Join(IItem otherItem);

        // bool Separate(byte amount, out IItem splitItem);
    }
}
