// -----------------------------------------------------------------
// <copyright file="ItemType.cs" company="2Dudes">
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
    using System.IO;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Class that represents an item type.
    /// </summary>
    public class ItemType : IItemType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemType"/> class.
        /// </summary>
        public ItemType()
        {
            this.TypeId = 0;
            this.Name = string.Empty;
            this.Description = string.Empty;
            this.Flags = new HashSet<ItemFlag>();
            this.DefaultAttributes = new Dictionary<ItemAttribute, IConvertible>();
            this.Locked = false;
        }

        /// <summary>
        /// Gets the id of the type of this item.
        /// </summary>
        public ushort TypeId { get; private set; }

        /// <summary>
        /// Gets the name of this type of item.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the decription of this type of item.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the flags for this type of item.
        /// </summary>
        public ISet<ItemFlag> Flags { get; }

        /// <summary>
        /// Gets the attributes of this type of item.
        /// </summary>
        public IDictionary<ItemAttribute, IConvertible> DefaultAttributes { get; }

        public bool Locked { get; private set; }

        /// <summary>
        /// Gets the client id of the type of this item.
        /// </summary>
        public ushort ClientId => this.Flags.Contains(ItemFlag.Disguise) ? Convert.ToUInt16(this.DefaultAttributes[ItemAttribute.DisguiseTarget]) : this.TypeId;

        public void LockChanges()
        {
            this.Locked = true;
        }

        public void SetId(ushort typeId)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException("This ItemType is locked and cannot be altered.");
            }

            this.TypeId = typeId;
        }

        public void SetName(string name)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException("This ItemType is locked and cannot be altered.");
            }

            this.Name = name;
        }

        public void SetDescription(string description)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException("This ItemType is locked and cannot be altered.");
            }

            this.Description = description.Trim('"');
        }

        public void SetFlag(ItemFlag flag)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException("This ItemType is locked and cannot be altered.");
            }

            this.Flags.Add(flag);
        }

        public void SetAttribute(string attributeName, int attributeValue)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException("This ItemType is locked and cannot be altered.");
            }

            if (!Enum.TryParse(attributeName, out ItemAttribute attribute))
            {
                throw new InvalidDataException($"Attempted to set an unknown Item attribute [{attributeName}].");
            }

            this.DefaultAttributes[attribute] = attributeValue;
        }
    }
}
