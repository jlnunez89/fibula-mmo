// -----------------------------------------------------------------
// <copyright file="PredefinedItemSet_v772.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Protocol.V772
{
    using System.Collections.Generic;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Utilities;
    using Fibula.Items.Contracts.Abstractions;
    using Fibula.Items.Contracts.Enumerations;

    /// <summary>
    /// Class that implements a <see cref="IPredefinedItemSet"/> for the protocol version 7.72.
    /// </summary>
    public class PredefinedItemSet_v772 : IPredefinedItemSet
    {
        /// <summary>
        /// The id of the type for a blood pool on the floor.
        /// </summary>
        public const ushort BloodPoolTypeId = 2886;

        /// <summary>
        /// The id of the type for a blood splatter on the floor.
        /// </summary>
        public const ushort BloodSplatterTypeId = 2889;

        /// <summary>
        /// The id of the type for a small file.
        /// </summary>
        public const ushort SmallFireTypeId = 2120;

        /// <summary>
        /// The id of the type for a small bone.
        /// </summary>
        public const ushort SmallBoneTypeId = 3115;

        /// <summary>
        /// Stores the reference to the item factory.
        /// </summary>
        private readonly IItemFactory itemFactory;

        /// <summary>
        /// Gets the item types for liquid splatter per blood type.
        /// </summary>
        private IDictionary<BloodType, IItemType> liquidSplatterPerBloodType;

        /// <summary>
        /// Gets the item types for liquid pool per blood type.
        /// </summary>
        private IDictionary<BloodType, IItemType> liquidPoolPerBloodType;

        /// <summary>
        /// Initializes a new instance of the <see cref="PredefinedItemSet_v772"/> class.
        /// </summary>
        /// <param name="itemFactory">A reference to the item factory in use.</param>
        public PredefinedItemSet_v772(IItemFactory itemFactory)
        {
            itemFactory.ThrowIfNull(nameof(itemFactory));

            this.itemFactory = itemFactory;

            this.InitializeItemSet();
        }

        /// <summary>
        /// Finds the splatter <see cref="IItemType"/> for a given blood type.
        /// </summary>
        /// <param name="bloodType">The type of blood to look the item type for.</param>
        /// <returns>The <see cref="IItemType"/> that's predefined for that blood type, or null if none is.</returns>
        public IItemType FindSplatterForBloodType(BloodType bloodType)
        {
            if (this.liquidSplatterPerBloodType != null && this.liquidSplatterPerBloodType.TryGetValue(bloodType, out IItemType splatterItemType))
            {
                return splatterItemType;
            }

            return null;
        }

        /// <summary>
        /// Finds the splatter <see cref="IItemType"/> for a given blood type.
        /// </summary>
        /// <param name="bloodType">The type of blood to look the item type for.</param>
        /// <returns>The <see cref="IItemType"/> that's predefined for that blood type, or null if none is.</returns>
        public IItemType FindPoolForBloodType(BloodType bloodType)
        {
            if (this.liquidPoolPerBloodType != null && this.liquidPoolPerBloodType.TryGetValue(bloodType, out IItemType splatterItemType))
            {
                return splatterItemType;
            }

            return null;
        }

        /// <summary>
        /// Initializes the item set.
        /// </summary>
        private void InitializeItemSet()
        {
            // Add blood splatters:
            this.liquidSplatterPerBloodType = new Dictionary<BloodType, IItemType>
            {
                { BloodType.Blood, this.itemFactory.FindTypeById(BloodSplatterTypeId).Clone() as IItemType },
                { BloodType.Slime, this.itemFactory.FindTypeById(BloodSplatterTypeId).Clone() as IItemType },
                { BloodType.Fire, this.itemFactory.FindTypeById(SmallFireTypeId).Clone() as IItemType },
                { BloodType.Bones, this.itemFactory.FindTypeById(SmallBoneTypeId).Clone() as IItemType },
            };

            this.liquidSplatterPerBloodType[BloodType.Blood].DefaultAttributes.Add(ItemAttribute.LiquidType, LiquidType.Blood);
            this.liquidSplatterPerBloodType[BloodType.Slime].DefaultAttributes.Add(ItemAttribute.LiquidType, LiquidType.Slime);

            // Add blood pools:
            this.liquidPoolPerBloodType = new Dictionary<BloodType, IItemType>
            {
                { BloodType.Blood, this.itemFactory.FindTypeById(BloodPoolTypeId).Clone() as IItemType },
                { BloodType.Slime, this.itemFactory.FindTypeById(BloodPoolTypeId).Clone() as IItemType },
            };

            this.liquidPoolPerBloodType[BloodType.Blood].DefaultAttributes.Add(ItemAttribute.LiquidType, LiquidType.Blood);
            this.liquidPoolPerBloodType[BloodType.Slime].DefaultAttributes.Add(ItemAttribute.LiquidType, LiquidType.Slime);
        }
    }
}
