// -----------------------------------------------------------------
// <copyright file="Item.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server
{
    using System;
    using System.Collections.Generic;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Parsing.Contracts.Abstractions;
    using Serilog;

    /// <summary>
    /// Class that represents all items in the game.
    /// </summary>
    public class Item : Thing, IItem
    {
        // public event ItemHolderChangeEvent OnHolderChanged;

        // public event ItemAmountChangeEvent OnAmountChanged;

        // private uint holder;

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        /// <param name="type">The type of this item.</param>
        public Item(IItemType type)
        {
            this.Type = type;
            // this.UniqueId = Guid.NewGuid().ToString().Substring(0, 8);

            // make a copy of the type we are based on...
            this.Attributes = new Dictionary<ItemAttribute, IConvertible>(this.Type.DefaultAttributes);
        }

        public IDictionary<ItemAttribute, IConvertible> Attributes { get; }

        public IItemType Type { get; }

        /// <summary>
        /// Gets the id of this thing, which is the item's client id.
        /// </summary>
        public override ushort ThingId => this.Type.ClientId;

        public bool ChangesOnUse => this.Type.Flags.Contains(ItemFlag.ChangeUse);

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

        public override bool CanBeMoved => !this.Type.Flags.Contains(ItemFlag.Unmove);

        public bool HasCollision => this.Type.Flags.Contains(ItemFlag.CollisionEvent);

        public bool HasSeparation => this.Type.Flags.Contains(ItemFlag.SeparationEvent);

        public override string Description => $"{this.Type.Name}{(string.IsNullOrWhiteSpace(this.Type.Description) ? string.Empty : "\n" + this.Type.Description)}";

        public override string InspectionText => this.Description;

        public bool IsCumulative => this.Type.Flags.Contains(ItemFlag.Cumulative);

        public byte Amount
        {
            get
            {
                if (this.Attributes.ContainsKey(ItemAttribute.Amount))
                {
                    return (byte)Math.Min(IItem.MaximumAmountOfCummulativeItems, Convert.ToInt32(this.Attributes[ItemAttribute.Amount]));
                }

                return 1;
            }

            set
            {
                this.Attributes[ItemAttribute.Amount] = value;
            }
        }

        public bool IsPathBlocking(byte avoidType = 0)
        {
            var blocking = this.Type.Flags.Contains(ItemFlag.Unpass);

            if (blocking)
            {
                return true;
            }

            blocking |= this.Type.Flags.Contains(ItemFlag.Avoid) && (avoidType == 0 || Convert.ToByte(this.Attributes[ItemAttribute.AvoidDamageTypes]) == avoidType);

            return blocking;
        }

        public bool IsContainer => this.Type.Flags.Contains(ItemFlag.Container);

        public bool IsDressable => this.Type.Flags.Contains(ItemFlag.Clothes);

        public byte DressPosition => this.Attributes.ContainsKey(ItemAttribute.BodyPosition) ? Convert.ToByte(this.Attributes[ItemAttribute.BodyPosition]) : (byte)Slot.Anywhere;

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

        public bool StaysOnTop => this.Type.Flags.Contains(ItemFlag.Top) || this.Type.Flags.Contains(ItemFlag.Clip);

        public bool StaysOnBottom => this.Type.Flags.Contains(ItemFlag.Bottom);

        public bool CanBeDressed => this.Type.Flags.Contains(ItemFlag.Clothes);

        public bool IsLiquidPool => this.Type.Flags.Contains(ItemFlag.LiquidPool);

        public bool IsLiquidSource => this.Type.Flags.Contains(ItemFlag.LiquidSource);

        public bool IsLiquidContainer => this.Type.Flags.Contains(ItemFlag.LiquidContainer);

        public byte LiquidType
        {
            get
            {
                if (!this.Type.Flags.Contains(ItemFlag.LiquidSource))
                {
                    return 0;
                }

                return Convert.ToByte(this.Attributes[ItemAttribute.SourceLiquidType]);
            }

            set
            {
                this.Attributes[ItemAttribute.SourceLiquidType] = value;
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

        public void SetAmount(byte amount)
        {
            this.Amount = Math.Min((byte)IItem.MaximumAmountOfCummulativeItems, amount);
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
        /// <param name="itemFactory">A reference to the item factory in use.</param>
        /// <param name="otherItem">The item to join with.</param>
        /// <returns>True if the operation was successful, false otherwise.</returns>
        public (bool success, IItem remainderItem) JoinWith(IItemFactory itemFactory, IItem otherItem)
        {
            itemFactory.ThrowIfNull(nameof(itemFactory));
            otherItem.ThrowIfNull(nameof(otherItem));

            if (this.Type.TypeId != otherItem.Type.TypeId || !this.IsCumulative)
            {
                return (false, otherItem);
            }

            // We can join these two, figure out if we have any remainder.
            if (otherItem.Amount + this.Amount <= IItem.MaximumAmountOfCummulativeItems)
            {
                this.Amount += otherItem.Amount;

                return (true, null);
            }

            // We've gone over the limit, send back a remainder.
            otherItem.SetAmount(Convert.ToByte(this.Amount + otherItem.Amount - IItem.MaximumAmountOfCummulativeItems));

            this.Amount = IItem.MaximumAmountOfCummulativeItems;

            return (true, otherItem);
        }

        public (bool success, IItem remainderItem) SeparateFrom(IItemFactory itemFactory, byte amount)
        {
            itemFactory.ThrowIfNull(nameof(itemFactory));

            if (!this.IsCumulative || this.Amount <= amount)
            {
                return (false, null);
            }

            this.Amount -= amount;

            var remainder = itemFactory.Create(this.Type.TypeId);

            remainder.SetAmount(amount);

            return (true, remainder);
        }
    }
}
