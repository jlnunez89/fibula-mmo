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
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Parsing.Contracts.Abstractions;

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
            // this.Name = this.Type.Name;
            // this.Description = this.Type.Description;
            // this.Flags = new HashSet<ItemFlag>(this.Type.Flags);
            this.Attributes = new Dictionary<ItemAttribute, IConvertible>(this.Type.DefaultAttributes);
        }

        public IDictionary<ItemAttribute, IConvertible> Attributes { get; }

        public IItemType Type { get; }

        /// <summary>
        /// Gets the id of this thing, which is the item's client id.
        /// </summary>
        public override ushort ThingId => this.Type.ClientId;

        // public override byte Count => this.Amount;

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

        // public uint HolderId => this.holder;

        public new Location Location
        {
            get
            {
                // if (this.HolderId != 0)
                // {
                //    return this.CarryLocation;
                // }

                return base.Location;
            }
        }

        public Location CarryLocation { get; private set; }

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

        public virtual void AddContent(IEnumerable<object> content)
        {
            Console.WriteLine($"Item.AddContent: Item with id {this.Type.TypeId} is not a Container, ignoring content.");
        }

        public bool IsContainer => this.Type.Flags.Contains(ItemFlag.Container) || this.Type.Flags.Contains(ItemFlag.Chest); // TODO: chest actually means a quest chest...

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

        public bool IsTop1 => this.Type.Flags.Contains(ItemFlag.Top) || this.Type.Flags.Contains(ItemFlag.Clip) || this.Type.Flags.Contains(ItemFlag.Hang);

        public bool IsTop2 => this.Type.Flags.Contains(ItemFlag.Bottom);

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

        // public IContainer Parent { get; private set; }

        public void SetAmount(byte amount)
        {
            var oldAmount = this.Amount;

            this.Amount = Math.Min((byte)IItem.MaximumAmountOfCummulativeItems, amount);

            //this.OnAmountChanged?.Invoke(this, oldAmount);
        }

        public void AddAttributes(IList<IParsedAttribute> attributes)
        {
            foreach (var attribute in attributes)
            {
                if ("Content".Equals(attribute.Name))
                {
                    // recursive add
                    this.AddContent(attribute.Value as IEnumerable<IParsedAttribute>);
                }
                else
                {
                    // these are safe to add as Attributes of the item.

                    if (!Enum.TryParse(attribute.Name, out ItemAttribute itemAttr))
                    {
                        // if (!ServerConfiguration.SupressInvalidItemWarnings)
                        // {
                        //    Console.WriteLine($"Item.AddContent: Unexpected attribute with name {attribute.Name} on item {this.Type.Name}, igoring.");
                        // }

                        continue;
                    }

                    try
                    {
                        this.Attributes.Add(itemAttr, attribute.Value as IConvertible);
                    }
                    catch
                    {
                        Console.WriteLine($"Item.AddContent: Unexpected attribute {attribute.Name} with illegal value {attribute.Value} on item {this.Type.Name}, igoring.");
                    }
                }
            }
        }

        // public void SetHolder(ICreature holder, Location holdingLoc = default)
        // {
        //    var oldHolder = this.holder;
        //    this.holder = holder?.Id ?? 0;
        //    this.CarryLocation = holdingLoc;

        // this.OnHolderChanged?.Invoke(this, oldHolder);
        // }

        // public void SetParent(IContainer parentContainer)
        // {
        //    this.Parent = parentContainer;
        // }

        // public virtual bool Join(IItem otherItem)
        // {
        //    if (!this.IsCumulative || otherItem?.Type.TypeId != this.Type.TypeId)
        //    {
        //        return false;
        //    }

        // var totalAmount = this.Amount + otherItem.Amount;

        // this.SetAmount((byte)Math.Min(totalAmount, 100));
        //    otherItem.SetAmount((byte)Math.Max(totalAmount - 100, 0));

        // return otherItem.Amount == 0;
        // }

        // public virtual bool Separate(byte amount, out IItem splitItem)
        // {
        //    splitItem = null;

        // if (amount > this.Amount)
        //    {
        //        return false;
        //    }

        // this.SetAmount((byte)Math.Max(this.Amount - amount, 0));

        // if (this.Amount > 0)
        //    {
        //        splitItem = ItemFactory.Create(this.Type.TypeId);
        //        splitItem.SetAmount(amount);
        //    }

        // return true;
        // }
    }
}
