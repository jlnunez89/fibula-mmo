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
    using Fibula.Parsing.Contracts.Abstractions;
    using Serilog;

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
        public bool ChangesOnUse => this.Type.Flags.Contains(ItemFlag.ChangeUse);

        /// <summary>
        /// Gets the Id of the item into which this will change upon use.
        /// Callers must check <see cref="ChangesOnUse"/> to verify this item does indeed have a target.
        /// </summary>
        /// <exception cref="InvalidOperationException">When there is no target to change to.</exception>
        public ushort ChangeOnUseTo
        {
            get
            {
                if (!this.Type.Flags.Contains(ItemFlag.ChangeUse))
                {
                    throw new InvalidOperationException($"Attempted to retrieve {nameof(this.ChangeOnUseTo)} on an item which doesn't have a target: {this.ThingId}");
                }

                return Convert.ToUInt16(this.Attributes[ItemAttribute.ChangeTarget]);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this item can be rotated.
        /// </summary>
        public bool CanBeRotated => this.Type.Flags.Contains(ItemFlag.Rotate);

        /// <summary>
        /// Gets the Id of the item into which this will rotate to.
        /// Callers must check <see cref="CanBeRotated"/> to verify this item does indeed have a target.
        /// </summary>
        /// <exception cref="InvalidOperationException">When there is no target to rotate to.</exception>
        public ushort RotateTo
        {
            get
            {
                if (!this.Type.Flags.Contains(ItemFlag.Rotate))
                {
                    throw new InvalidOperationException($"Attempted to retrieve {nameof(this.RotateTo)} on an item which doesn't have a target: {this.ThingId}");
                }

                return Convert.ToUInt16(this.Attributes[ItemAttribute.RotateTarget]);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this item can be moved.
        /// </summary>
        public override bool CanBeMoved => !this.Type.Flags.Contains(ItemFlag.Unmove);

        public bool HasCollision => this.Type.Flags.Contains(ItemFlag.CollisionEvent);

        public bool HasSeparation => this.Type.Flags.Contains(ItemFlag.SeparationEvent);

        /// <summary>
        /// Gets a value indicating whether this item can be accumulated.
        /// </summary>
        public bool IsCumulative => this.Type.Flags.Contains(ItemFlag.Cumulative);

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
                this.Attributes[ItemAttribute.Amount] = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this item is a container.
        /// </summary>
        public bool IsContainer => this.Type.Flags.Contains(ItemFlag.Container);

        /// <summary>
        /// Gets a value indicating whether this item can be dressed.
        /// </summary>
        public bool CanBeDressed => this.Type.Flags.Contains(ItemFlag.Clothes);

        public Slot DressPosition => this.Attributes.ContainsKey(ItemAttribute.BodyPosition) && Enum.TryParse(this.Attributes[ItemAttribute.BodyPosition].ToString(), out Slot parsedSlot) ? parsedSlot : Slot.Anywhere;

        public bool IsGround => this.Type.Flags.Contains(ItemFlag.Bank);

        public byte MovementPenalty
        {
            get
            {
                if (!this.IsGround || !this.Attributes.ContainsKey(ItemAttribute.Waypoints))
                {
                    return 0;
                }

                return Convert.ToByte(this.Attributes[ItemAttribute.Waypoints]);
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

        public bool StaysOnTop => this.Type.Flags.Contains(ItemFlag.Top) || this.Type.Flags.Contains(ItemFlag.Clip);

        public bool StaysOnBottom => this.Type.Flags.Contains(ItemFlag.Bottom);

        public bool IsLiquidPool => this.Type.Flags.Contains(ItemFlag.LiquidPool);

        public bool IsLiquidSource => this.Type.Flags.Contains(ItemFlag.LiquidSource);

        public bool IsLiquidContainer => this.Type.Flags.Contains(ItemFlag.LiquidContainer);

        public LiquidType LiquidType
        {
            get
            {
                if (((this.Type.Flags.Contains(ItemFlag.LiquidSource) && this.Attributes.TryGetValue(ItemAttribute.SourceLiquidType, out IConvertible typeValue)) ||
                     (this.Type.Flags.Contains(ItemFlag.LiquidContainer) && this.Attributes.TryGetValue(ItemAttribute.ContainerLiquidType, out typeValue)) ||
                     (this.Type.Flags.Contains(ItemFlag.LiquidPool) && this.Attributes.TryGetValue(ItemAttribute.PoolLiquidType, out typeValue))) &&
                     Enum.IsDefined(typeof(LiquidType), typeValue))
                {
                    return (LiquidType)typeValue;
                }

                return LiquidType.None;
            }
        }

        public byte Attack
        {
            get
            {
                if (this.Type.Flags.Contains(ItemFlag.Weapon))
                {
                    return Convert.ToByte(this.Attributes[ItemAttribute.WeaponAttackValue]);
                }

                if (this.Type.Flags.Contains(ItemFlag.Throw))
                {
                    return Convert.ToByte(this.Attributes[ItemAttribute.ThrowAttackValue]);
                }

                return 0;
            }
        }

        public byte Defense
        {
            get
            {
                if (this.Type.Flags.Contains(ItemFlag.Shield))
                {
                    return Convert.ToByte(this.Attributes[ItemAttribute.ShieldDefendValue]);
                }

                if (this.Type.Flags.Contains(ItemFlag.Weapon))
                {
                    return Convert.ToByte(this.Attributes[ItemAttribute.WeaponAttackValue]);
                }

                if (this.Type.Flags.Contains(ItemFlag.Throw))
                {
                    return Convert.ToByte(this.Attributes[ItemAttribute.ThrowDefendValue]);
                }

                return 0;
            }
        }

        public byte Armor
        {
            get
            {
                if (this.Type.Flags.Contains(ItemFlag.Armor))
                {
                    return Convert.ToByte(this.Attributes[ItemAttribute.ArmorValue]);
                }

                return 0;
            }
        }

        public int Range
        {
            get
            {
                if (this.Type.Flags.Contains(ItemFlag.Throw))
                {
                    return Convert.ToByte(this.Attributes[ItemAttribute.ThrowRange]);
                }

                if (this.Type.Flags.Contains(ItemFlag.Bow))
                {
                    return Convert.ToByte(this.Attributes[ItemAttribute.BowRange]);
                }

                return 0x01;
            }
        }

        public bool BlocksThrow => this.Type.Flags.Contains(ItemFlag.Unthrow);

        public bool BlocksPass => this.Type.Flags.Contains(ItemFlag.Unpass);

        public bool BlocksLay => this.Type.Flags.Contains(ItemFlag.Unlay);

        public decimal Weight => (this.Type.Flags.Contains(ItemFlag.Take) ? Convert.ToDecimal(this.Attributes[ItemAttribute.Weight]) / 100 : default) * this.Amount;

        public bool IsPathBlocking(byte avoidTypes = (byte)AvoidDamageType.All)
        {
            var blocking = this.BlocksPass;

            if (blocking)
            {
                return true;
            }

            blocking |= this.Type.Flags.Contains(ItemFlag.Avoid) && (Convert.ToByte(this.Attributes[ItemAttribute.AvoidDamageTypes]) ^ avoidTypes) > 0;

            return blocking;
        }

        public void SetAmount(byte amount)
        {
            this.Amount = Math.Min(ItemConstants.MaximumAmountOfCummulativeItems, amount);
        }

        public void SetAttributes(ILogger logger, IItemFactory itemFactory, IList<IParsedAttribute> attributes)
        {
            logger.ThrowIfNull(nameof(logger));

            if (attributes == null)
            {
                return;
            }

            foreach (var attribute in attributes)
            {
                if ("Content".Equals(attribute.Name) && this is IContainerItem containerItem)
                {
                    containerItem.AddContent(logger, itemFactory, attribute.Value as IEnumerable<IParsedElement>);

                    continue;
                }

                // These are safe to add as Attributes of the item.
                if (!Enum.TryParse(attribute.Name, out ItemAttribute itemAttr))
                {
                    logger.Warning($"Unsupported attribute {attribute.Name} on {this.Type.Name}, ignoring.");
                    continue;
                }

                try
                {
                    this.Attributes[itemAttr] = attribute.Value as IConvertible;
                }
                catch
                {
                    logger.Warning($"Unexpected attribute {attribute.Name} with illegal value {attribute.Value} on item {this.Type.Name}, ignoring.");
                }
            }
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
            otherItem.SetAmount(Convert.ToByte(this.Amount + otherItem.Amount - ItemConstants.MaximumAmountOfCummulativeItems));

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

            remainder.SetAmount(amount);

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
