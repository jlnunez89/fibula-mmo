// -----------------------------------------------------------------
// <copyright file="ItemType.cs" company="2Dudes">
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
    using Fibula.Items.Contracts.Abstractions;
    using Fibula.Items.Contracts.Enumerations;

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

        /// <summary>
        /// Gets a value indicating whether this item type is locked and no longer accepting changes.
        /// </summary>
        public bool Locked { get; private set; }

        /// <summary>
        /// Gets the client id of the type of this item.
        /// </summary>
        public ushort ClientId => this.Flags.Contains(ItemFlag.IsDisguised) ? Convert.ToUInt16(this.DefaultAttributes[ItemAttribute.DisguiseAs]) : this.TypeId;

        /// <summary>
        /// Locks the type, preventing it from accepting changes.
        /// </summary>
        public void LockChanges()
        {
            this.Locked = true;
        }

        /// <summary>
        /// Sets the type's id.
        /// </summary>
        /// <param name="typeId">The id of the type.</param>
        public void SetId(ushort typeId)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException($"This ItemType is locked and cannot be altered. {nameof(this.SetId)}({nameof(typeId)}={typeId}");
            }

            this.TypeId = typeId;
        }

        /// <summary>
        /// Sets the type's name.
        /// </summary>
        /// <param name="name">The name of the type.</param>
        public void SetName(string name)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException($"This ItemType is locked and cannot be altered. {nameof(this.SetName)}({nameof(name)}={name}");
            }

            this.Name = name;
        }

        /// <summary>
        /// Sets the type's description.
        /// </summary>
        /// <param name="description">The description of the type.</param>
        public void SetDescription(string description)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException($"This ItemType is locked and cannot be altered. {nameof(this.SetDescription)}({nameof(description)}={description}");
            }

            this.Description = description.Trim('"');
        }

        /// <summary>
        /// Sets a flag in this type.
        /// </summary>
        /// <param name="flag">The flag to set in the type.</param>
        public void SetFlag(ItemFlag flag)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException($"This ItemType is locked and cannot be altered. {nameof(this.SetFlag)}({nameof(flag)}={flag}");
            }

            this.Flags.Add(flag);
        }

        /// <summary>
        /// Sets an attribute in this type.
        /// </summary>
        /// <param name="attribute">The attribute to set in the type.</param>
        /// <param name="attributeValue">The value of the attribute to set in the type.</param>
        public void SetAttribute(ItemAttribute attribute, int attributeValue)
        {
            if (this.Locked)
            {
                throw new InvalidOperationException($"This ItemType is locked and cannot be altered. {nameof(this.SetAttribute)}({nameof(attribute)}={attribute},{nameof(attributeValue)}={attributeValue}");
            }

            this.DefaultAttributes[attribute] = attributeValue;
        }
    }
}
