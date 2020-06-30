// -----------------------------------------------------------------
// <copyright file="Item.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Items
{
    using System;
    using System.Collections.Generic;
    using Fibula.Common;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Contracts.Structs;
    using Fibula.Common.Utilities;
    using Fibula.Items.Contracts.Abstractions;
    using Fibula.Items.Contracts.Constants;
    using Fibula.Items.Contracts.Enumerations;

    /// <summary>
    /// Class that represents all items in the game.
    /// </summary>
    public class Item : Thing, IItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        /// <param name="type">The type of this item.</param>
        public Item(IItemType type)
        {
            this.Type = type;

            this.UniqueId = Guid.NewGuid();

            // make a copy of the type we are based on...
            this.Attributes = new Dictionary<ItemAttribute, IConvertible>(this.Type.DefaultAttributes);
        }

        /// <summary>
        /// Gets the unique id of this item.
        /// </summary>
        public Guid UniqueId { get; }

        /// <summary>
        /// Gets a reference to this item's <see cref="IItemType"/>.
        /// </summary>
        public IItemType Type { get; }

        /// <summary>
        /// Gets the attributes of this item.
        /// </summary>
        public IDictionary<ItemAttribute, IConvertible> Attributes { get; }

        /// <summary>
        /// Gets the id of this thing, which is the item's client id.
        /// </summary>
        public override ushort ThingId => this.Type.TypeId;

        /// <summary>
        /// Gets a value indicating whether this item changes on use.
        /// </summary>
        public bool ChangesOnUse => this.Type.Flags.Contains(ItemFlag.ChangesOnUse);

        /// <summary>
        /// Gets the Id of the item into which this will change upon use.
        /// Callers must check <see cref="ChangesOnUse"/> to verify this item does indeed have a target.
        /// </summary>
        /// <exception cref="InvalidOperationException">When there is no target to change to.</exception>
        public ushort ChangeOnUseTo
        {
            get
            {
                if (!this.Type.Flags.Contains(ItemFlag.ChangesOnUse))
                {
                    throw new InvalidOperationException($"Attempted to retrieve {nameof(this.ChangeOnUseTo)} on an item which doesn't have a target: {this.ThingId}");
                }

                return Convert.ToUInt16(this.Attributes[ItemAttribute.ChangeOnUseTo]);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this item can be rotated.
        /// </summary>
        public bool CanBeRotated => this.Type.Flags.Contains(ItemFlag.CanBeRotated);

        /// <summary>
        /// Gets the Id of the item into which this will rotate to.
        /// Callers must check <see cref="CanBeRotated"/> to verify this item does indeed have a target.
        /// </summary>
        /// <exception cref="InvalidOperationException">When there is no target to rotate to.</exception>
        public ushort RotateTo
        {
            get
            {
                if (!this.Type.Flags.Contains(ItemFlag.CanBeRotated))
                {
                    throw new InvalidOperationException($"Attempted to retrieve {nameof(this.RotateTo)} on an item which doesn't have a target: {this.ThingId}");
                }

                return Convert.ToUInt16(this.Attributes[ItemAttribute.RotateTo]);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this item can be moved.
        /// </summary>
        public override bool CanBeMoved => !this.Type.Flags.Contains(ItemFlag.IsUnmoveable);

        /// <summary>
        /// Gets a value indicating whether this item triggers a collision event.
        /// </summary>
        public bool HasCollision => this.Type.Flags.Contains(ItemFlag.TriggersCollision);

        /// <summary>
        /// Gets a value indicating whether this item triggers a separation event.
        /// </summary>
        public bool HasSeparation => this.Type.Flags.Contains(ItemFlag.TriggersSeparation);

        /// <summary>
        /// Gets a value indicating whether this item can be accumulated.
        /// </summary>
        public bool IsCumulative => this.Type.Flags.Contains(ItemFlag.IsCumulative);

        /// <summary>
        /// Gets or sets the amount of this item.
        /// </summary>
        public byte Amount
        {
            get
            {
                if (this.Attributes.ContainsKey(ItemAttribute.Amount))
                {
                    return (byte)Math.Min(ItemConstants.MaximumAmountOfCummulativeItems, Convert.ToInt32(this.Attributes[ItemAttribute.Amount]));
                }

                return 1;
            }

            set
            {
                this.Attributes[ItemAttribute.Amount] = Math.Min(ItemConstants.MaximumAmountOfCummulativeItems, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this item is a container.
        /// </summary>
        public bool IsContainer => this.Type.Flags.Contains(ItemFlag.IsContainer);

        /// <summary>
        /// Gets a value indicating whether this item can be dressed.
        /// </summary>
        public bool CanBeDressed => this.Type.Flags.Contains(ItemFlag.IsDressable);

        /// <summary>
        /// Gets the position at which the item can be dressed.
        /// </summary>
        public Slot DressPosition => this.Attributes.ContainsKey(ItemAttribute.DressPosition) && Enum.TryParse(this.Attributes[ItemAttribute.DressPosition].ToString(), out Slot parsedSlot) ? parsedSlot : Slot.Anywhere;

        /// <summary>
        /// Gets a value indicating whether this item is ground floor.
        /// </summary>
        public bool IsGround => this.Type.Flags.Contains(ItemFlag.IsGround);

        /// <summary>
        /// Gets the movement cost for walking over this item, assuming it <see cref="IsGround"/>.
        /// </summary>
        public byte MovementPenalty
        {
            get
            {
                if (!this.IsGround || !this.Attributes.ContainsKey(ItemAttribute.MovementPenalty))
                {
                    return 0;
                }

                return Convert.ToByte(this.Attributes[ItemAttribute.MovementPenalty]);
            }
        }

        /// <summary>
        /// Gets the location where this thing is being carried at, if any.
        /// </summary>
        public override Location? CarryLocation
        {
            get
            {
                return this.ParentContainer?.CarryLocation;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this item stays on top of the stack.
        /// </summary>
        public bool StaysOnTop => this.Type.Flags.Contains(ItemFlag.StaysOnTop);

        /// <summary>
        /// Gets a value indicating whether this item stays on the bottom of the stack.
        /// </summary>
        public bool StaysOnBottom => this.Type.Flags.Contains(ItemFlag.IsClipped) || this.Type.Flags.Contains(ItemFlag.StaysOnBottom);

        /// <summary>
        /// Gets a value indicating whether this item is a liquid pool.
        /// </summary>
        public bool IsLiquidPool => this.Type.Flags.Contains(ItemFlag.IsLiquidPool);

        /// <summary>
        /// Gets a value indicating whether this item is a liquid source.
        /// </summary>
        public bool IsLiquidSource => this.Type.Flags.Contains(ItemFlag.IsLiquidSource);

        /// <summary>
        /// Gets a value indicating whether this item is a liquid container.
        /// </summary>
        public bool IsLiquidContainer => this.Type.Flags.Contains(ItemFlag.IsLiquidContainer);

        /// <summary>
        /// Gets the type of liquid in this item, assuming it: <see cref="IsLiquidPool"/>, <see cref="IsLiquidSource"/>, or <see cref="IsLiquidContainer"/>.
        /// </summary>
        public LiquidType LiquidType
        {
            get
            {
                if ((this.Type.Flags.Contains(ItemFlag.IsLiquidSource) || this.Type.Flags.Contains(ItemFlag.IsLiquidContainer) || this.Type.Flags.Contains(ItemFlag.IsLiquidPool)) &&
                    this.Attributes.TryGetValue(ItemAttribute.LiquidType, out IConvertible typeValue) &&
                    Enum.IsDefined(typeof(LiquidType), typeValue.ToInt32(null)))
                {
                    return (LiquidType)typeValue;
                }

                return LiquidType.None;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the item blocks throwing through it.
        /// </summary>
        public bool BlocksThrow => this.Type.Flags.Contains(ItemFlag.BlocksThrow);

        /// <summary>
        /// Gets a value indicating whether the item blocks walking on it.
        /// </summary>
        public bool BlocksPass => this.Type.Flags.Contains(ItemFlag.BlocksWalk);

        /// <summary>
        /// Gets a value indicating whether the item blocks laying anything on it.
        /// </summary>
        public bool BlocksLay => this.Type.Flags.Contains(ItemFlag.BlocksLay);

        /// <summary>
        /// Determines if this item is blocks pathfinding.
        /// </summary>
        /// <param name="avoidTypes">The damage types to avoid when checking for path blocking. By default, all types are considered path blocking.</param>
        /// <returns>True if the tile is considered path blocking, false otherwise.</returns>
        public bool IsPathBlocking(byte avoidTypes = (byte)AvoidDamageType.All)
        {
            var blocking = this.BlocksPass;

            if (blocking)
            {
                return true;
            }

            blocking |= this.Type.Flags.Contains(ItemFlag.ShouldBeAvoided) && (Convert.ToByte(this.Attributes[ItemAttribute.DamageTypesToAvoid]) ^ avoidTypes) > 0;

            return blocking;
        }

        /// <summary>
        /// Attempts to join an item to this item's content at the default index.
        /// </summary>
        /// <param name="otherItem">The item to join with.</param>
        /// <returns>True if the operation was successful, false otherwise. Along with any surplus of the item after merge.</returns>
        public (bool success, IItem surplusItem) Merge(IItem otherItem)
        {
            otherItem.ThrowIfNull(nameof(otherItem));

            if (this.Type.TypeId != otherItem.Type.TypeId || !this.IsCumulative)
            {
                return (false, otherItem);
            }

            // We can join these two, figure out if we have any remainder.
            if (otherItem.Amount + this.Amount <= ItemConstants.MaximumAmountOfCummulativeItems)
            {
                this.Amount += otherItem.Amount;

                return (true, null);
            }

            // We've gone over the limit, send back a remainder.
            otherItem.Amount = Convert.ToByte(this.Amount + otherItem.Amount - ItemConstants.MaximumAmountOfCummulativeItems);

            this.Amount = ItemConstants.MaximumAmountOfCummulativeItems;

            return (true, otherItem);
        }

        /// <summary>
        /// Attempts to split this item into two based on the amount provided.
        /// </summary>
        /// <param name="itemFactory">A reference to the item factory in use.</param>
        /// <param name="amount">The amount of the item to split.</param>
        /// <returns>True if the operation was successful, false otherwise, along with the item produced, if any.</returns>
        public (bool success, IItem itemProduced) Split(IItemFactory itemFactory, byte amount)
        {
            itemFactory.ThrowIfNull(nameof(itemFactory));

            if (!this.IsCumulative || this.Amount <= amount)
            {
                return (false, null);
            }

            this.Amount -= amount;

            var remainder = itemFactory.CreateItem(new ItemCreationArguments() { TypeId = this.Type.TypeId });

            remainder.Amount = amount;

            return (true, remainder);
        }

        /// <summary>
        /// Provides a string describing the current thing for logging purposes.
        /// </summary>
        /// <returns>The string to log.</returns>
        public override string DescribeForLogger()
        {
            return $"[{this.Type.TypeId}] {this.GetType().Name}: {this.Amount} {this.Type.Name}";
        }
    }
}
