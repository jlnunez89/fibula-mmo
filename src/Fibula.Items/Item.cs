// -----------------------------------------------------------------
// <copyright file="Item.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Items
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Fibula.Common;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Contracts.Structs;
    using Fibula.Common.Utilities;
    using Fibula.Data.Entities.Contracts.Abstractions;
    using Fibula.Items.Contracts.Abstractions;
    using Fibula.Items.Contracts.Constants;
    using Fibula.Items.Contracts.Enumerations;
    using Fibula.Items.Contracts.Extensions;

    /// <summary>
    /// Class that represents all items in the game.
    /// </summary>
    public class Item : Thing, IItem
    {
        /// <summary>
        /// Stores the expiration time left on this item.
        /// </summary>
        private TimeSpan expirationTimeLeft;

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        /// <param name="type">The type of this item.</param>
        public Item(IItemTypeEntity type)
        {
            this.Type = type;

            // make a copy of the type we are based on...
            this.Attributes = new Dictionary<ItemAttribute, IConvertible>(this.Type.DefaultAttributes.Select(kvp => KeyValuePair.Create((ItemAttribute)kvp.Key, kvp.Value)));

            this.InitializeExpirationTime();
        }

        /// <summary>
        /// Gets a reference to this item's <see cref="IItemTypeEntity"/>.
        /// </summary>
        public IItemTypeEntity Type { get; }

        /// <summary>
        /// Gets the attributes of this item.
        /// </summary>
        public IDictionary<ItemAttribute, IConvertible> Attributes { get; }

        /// <summary>
        /// Gets the type id of this item.
        /// </summary>
        public override ushort TypeId => this.Type.TypeId;

        /// <summary>
        /// Gets a value indicating whether this item changes on use.
        /// </summary>
        public bool ChangesOnUse => this.Type.HasItemFlag(ItemFlag.ChangesOnUse);

        /// <summary>
        /// Gets the Id of the item into which this will change upon use.
        /// Callers must check <see cref="ChangesOnUse"/> to verify this item does indeed have a target.
        /// </summary>
        /// <exception cref="InvalidOperationException">When there is no target to change to.</exception>
        public ushort ChangeOnUseTo
        {
            get
            {
                if (!this.Type.HasItemFlag(ItemFlag.ChangesOnUse))
                {
                    throw new InvalidOperationException($"Attempted to retrieve {nameof(this.ChangeOnUseTo)} on an item which doesn't have a target: {this}");
                }

                return Convert.ToUInt16(this.Attributes[ItemAttribute.ChangeOnUseTo]);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this item can be rotated.
        /// </summary>
        public bool CanBeRotated => this.Type.HasItemFlag(ItemFlag.CanBeRotated);

        /// <summary>
        /// Gets the Id of the item into which this will rotate to.
        /// Callers must check <see cref="CanBeRotated"/> to verify this item does indeed have a target.
        /// </summary>
        /// <exception cref="InvalidOperationException">When there is no target to rotate to.</exception>
        public ushort RotateTo
        {
            get
            {
                if (!this.Type.HasItemFlag(ItemFlag.CanBeRotated))
                {
                    throw new InvalidOperationException($"Attempted to retrieve {nameof(this.RotateTo)} on an item which doesn't have a target: {this}");
                }

                return Convert.ToUInt16(this.Attributes[ItemAttribute.RotateTo]);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this item expires.
        /// </summary>
        public bool HasExpiration => this.Type.HasItemFlag(ItemFlag.HasExpiration);

        /// <summary>
        /// Gets the time left before this item expires, if it <see cref="HasExpiration"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">When the item does not expire.</exception>
        public TimeSpan ExpirationTimeLeft
        {
            get
            {
                if (!this.Type.HasItemFlag(ItemFlag.HasExpiration))
                {
                    throw new InvalidOperationException($"Attempted to retrieve {nameof(this.ExpirationTimeLeft)} on an item which doesn't have expiration: {this}");
                }

                return this.expirationTimeLeft;
            }
        }

        /// <summary>
        /// Gets the Id of the item into which this will expire to, if it <see cref="HasExpiration"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">When the item does not expire.</exception>
        public ushort ExpirationTarget
        {
            get
            {
                if (!this.Type.HasItemFlag(ItemFlag.HasExpiration))
                {
                    throw new InvalidOperationException($"Attempted to retrieve {nameof(this.ExpirationTarget)} on an item which doesn't have expiration: {this}");
                }

                return Convert.ToUInt16(this.Attributes[ItemAttribute.ExpirationTarget]);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this item can be moved.
        /// </summary>
        public override bool CanBeMoved => !this.Type.HasItemFlag(ItemFlag.IsUnmoveable);

        /// <summary>
        /// Gets a value indicating whether this item triggers a collision event.
        /// </summary>
        public bool HasCollision => this.Type.HasItemFlag(ItemFlag.TriggersCollision);

        /// <summary>
        /// Gets a value indicating whether this item triggers a separation event.
        /// </summary>
        public bool HasSeparation => this.Type.HasItemFlag(ItemFlag.TriggersSeparation);

        /// <summary>
        /// Gets a value indicating whether this item can be accumulated.
        /// </summary>
        public bool IsCumulative => this.Type.HasItemFlag(ItemFlag.IsCumulative);

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
        public bool IsContainer => this.Type.HasItemFlag(ItemFlag.IsContainer);

        /// <summary>
        /// Gets a value indicating whether this item can be dressed.
        /// </summary>
        public bool CanBeDressed => this.Type.HasItemFlag(ItemFlag.IsDressable);

        /// <summary>
        /// Gets the position at which the item can be dressed.
        /// </summary>
        public Slot DressPosition => this.Attributes.ContainsKey(ItemAttribute.DressPosition) && Enum.TryParse(this.Attributes[ItemAttribute.DressPosition].ToString(), out Slot parsedSlot) ? parsedSlot : Slot.Anywhere;

        /// <summary>
        /// Gets a value indicating whether this item is ground floor.
        /// </summary>
        public bool IsGround => this.Type.HasItemFlag(ItemFlag.IsGround);

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
        /// Gets a value indicating whether this item is a ground aesthetic fix.
        /// </summary>
        public bool IsGroundFix => this.Type.HasItemFlag(ItemFlag.IsGroundBorder);

        /// <summary>
        /// Gets a value indicating whether this item stays on top of the stack.
        /// </summary>
        public bool StaysOnTop => this.Type.HasItemFlag(ItemFlag.StaysOnTop);

        /// <summary>
        /// Gets a value indicating whether this item stays on the bottom of the stack.
        /// </summary>
        public bool StaysOnBottom => this.Type.HasItemFlag(ItemFlag.StaysOnBottom);

        /// <summary>
        /// Gets a value indicating whether this item is a liquid pool.
        /// </summary>
        public bool IsLiquidPool => this.Type.HasItemFlag(ItemFlag.IsLiquidPool);

        /// <summary>
        /// Gets a value indicating whether this item is a liquid source.
        /// </summary>
        public bool IsLiquidSource => this.Type.HasItemFlag(ItemFlag.IsLiquidSource);

        /// <summary>
        /// Gets a value indicating whether this item is a liquid container.
        /// </summary>
        public bool IsLiquidContainer => this.Type.HasItemFlag(ItemFlag.IsLiquidContainer);

        /// <summary>
        /// Gets the type of liquid in this item, assuming it: <see cref="IsLiquidPool"/>, <see cref="IsLiquidSource"/>, or <see cref="IsLiquidContainer"/>.
        /// </summary>
        public LiquidType LiquidType
        {
            get
            {
                if ((this.Type.HasItemFlag(ItemFlag.IsLiquidSource) || this.Type.HasItemFlag(ItemFlag.IsLiquidContainer) || this.Type.HasItemFlag(ItemFlag.IsLiquidPool)) &&
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
        public bool BlocksThrow => this.Type.HasItemFlag(ItemFlag.BlocksThrow);

        /// <summary>
        /// Gets a value indicating whether the item blocks walking on it.
        /// </summary>
        public bool BlocksPass => this.Type.HasItemFlag(ItemFlag.BlocksWalk);

        /// <summary>
        /// Gets a value indicating whether the item blocks laying anything on it.
        /// </summary>
        public bool BlocksLay => this.Type.HasItemFlag(ItemFlag.BlocksLay);

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

            blocking |= this.Type.HasItemFlag(ItemFlag.ShouldBeAvoided) && (Convert.ToByte(this.Attributes[ItemAttribute.DamageTypesToAvoid]) ^ avoidTypes) > 0;

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

            var remainder = itemFactory.CreateItem(ItemCreationArguments.WithTypeId(this.Type.TypeId));

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

        /// <summary>
        /// Initializes this item's expiration time.
        /// </summary>
        private void InitializeExpirationTime()
        {
            if (!this.HasExpiration)
            {
                this.expirationTimeLeft = TimeSpan.Zero;

                return;
            }

            if (this.Attributes.TryGetValue(ItemAttribute.ExpirationTimeLeft, out IConvertible expTimeLeft))
            {
                this.expirationTimeLeft = TimeSpan.FromSeconds(expTimeLeft.ToUInt32(CultureInfo.InvariantCulture));
            }
            else if (this.Attributes.TryGetValue(ItemAttribute.ExpirationStartTime, out IConvertible expStartTime))
            {
                this.expirationTimeLeft = TimeSpan.FromSeconds(expStartTime.ToUInt32(CultureInfo.InvariantCulture));
            }
            else
            {
                this.expirationTimeLeft = TimeSpan.Zero;
            }
        }
    }
}
