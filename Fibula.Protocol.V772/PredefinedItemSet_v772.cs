// -----------------------------------------------------------------
// <copyright file="PredefinedItemSet_v772.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Protocol.V772
{
    using System.Collections.Generic;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Utilities;
    using Fibula.Data.Entities.Contracts.Abstractions;
    using Fibula.Data.Entities.Contracts.Enumerations;
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
        private IDictionary<BloodType, IItemTypeEntity> liquidSplatterPerBloodType;

        /// <summary>
        /// Gets the item types for liquid pool per blood type.
        /// </summary>
        private IDictionary<BloodType, IItemTypeEntity> liquidPoolPerBloodType;

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
        /// Finds the splatter <see cref="IItemTypeEntity"/> for a given blood type.
        /// </summary>
        /// <param name="bloodType">The type of blood to look the item type for.</param>
        /// <returns>The <see cref="IItemTypeEntity"/> that's predefined for that blood type, or null if none is.</returns>
        public IItemTypeEntity FindSplatterForBloodType(BloodType bloodType)
        {
            if (this.liquidSplatterPerBloodType != null && this.liquidSplatterPerBloodType.TryGetValue(bloodType, out IItemTypeEntity splatterItemType))
            {
                return splatterItemType;
            }

            return null;
        }

        /// <summary>
        /// Finds the splatter <see cref="IItemTypeEntity"/> for a given blood type.
        /// </summary>
        /// <param name="bloodType">The type of blood to look the item type for.</param>
        /// <returns>The <see cref="IItemTypeEntity"/> that's predefined for that blood type, or null if none is.</returns>
        public IItemTypeEntity FindPoolForBloodType(BloodType bloodType)
        {
            if (this.liquidPoolPerBloodType != null && this.liquidPoolPerBloodType.TryGetValue(bloodType, out IItemTypeEntity splatterItemType))
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
            this.liquidSplatterPerBloodType = new Dictionary<BloodType, IItemTypeEntity>
            {
                { BloodType.Blood, this.itemFactory.FindTypeById(BloodSplatterTypeId).Clone() as IItemTypeEntity },
                { BloodType.Slime, this.itemFactory.FindTypeById(BloodSplatterTypeId).Clone() as IItemTypeEntity },
                { BloodType.Fire, this.itemFactory.FindTypeById(SmallFireTypeId).Clone() as IItemTypeEntity },
                { BloodType.Bones, this.itemFactory.FindTypeById(SmallBoneTypeId).Clone() as IItemTypeEntity },
            };

            this.liquidSplatterPerBloodType[BloodType.Blood].DefaultAttributes.Add((byte)ItemAttribute.LiquidType, LiquidType.Blood);
            this.liquidSplatterPerBloodType[BloodType.Slime].DefaultAttributes.Add((byte)ItemAttribute.LiquidType, LiquidType.Slime);

            // Add blood pools:
            this.liquidPoolPerBloodType = new Dictionary<BloodType, IItemTypeEntity>
            {
                { BloodType.Blood, this.itemFactory.FindTypeById(BloodPoolTypeId).Clone() as IItemTypeEntity },
                { BloodType.Slime, this.itemFactory.FindTypeById(BloodPoolTypeId).Clone() as IItemTypeEntity },
            };

            this.liquidPoolPerBloodType[BloodType.Blood].DefaultAttributes.Add((byte)ItemAttribute.LiquidType, LiquidType.Blood);
            this.liquidPoolPerBloodType[BloodType.Slime].DefaultAttributes.Add((byte)ItemAttribute.LiquidType, LiquidType.Slime);
        }
    }
}
