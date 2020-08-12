// -----------------------------------------------------------------
// <copyright file="IMonsterTypeEntity.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Data.Entities.Contracts.Abstractions
{
    using System.Collections.Generic;
    using Fibula.Data.Entities.Contracts.Enumerations;
    using Fibula.Data.Entities.Contracts.Structs;

    /// <summary>
    /// Interface for 'types of monster' entities.
    /// </summary>
    public interface IMonsterTypeEntity : IIdentifiableEntity
    {
        /// <summary>
        /// Gets the id of the monster race.
        /// </summary>
        ushort RaceId { get; }

        /// <summary>
        /// Gets the name of the monster type.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the article to use with the name.
        /// </summary>
        string Article { get; }

        /// <summary>
        /// Gets the amount of experience that this type of monster deals.
        /// </summary>
        uint BaseExperienceYield { get; }

        /// <summary>
        /// Gets the base cost to summon this type of monster.
        /// </summary>
        ushort SummonCost { get; }

        /// <summary>
        /// Gets the threshold value under which this type of monster begins to flee battle.
        /// </summary>
        ushort HitpointFleeThreshold { get; }

        /// <summary>
        /// Gets the distance in tiles after which this type of monster looses track of their target.
        /// </summary>
        byte LoseTargetDistance { get; }

        ///// <summary>
        ///// Gets a collection of spells known by this type of monster.
        ///// </summary>
        // ISet<ISpell> KnownSpells { get; }

        /// <summary>
        /// Gets the flags set for this type of monster.
        /// </summary>
        /// <remarks>The flags are stored as bits in a 64 bit unsigned integer.</remarks>
        ulong Flags { get; }

        /// <summary>
        /// Gets the skills that this type of monster starts with.
        /// </summary>
        IDictionary<SkillType, (int DefaultLevel, int CurrentLevel, int MaximumLevel, uint TargetCount, uint CountIncreaseFactor, byte IncreaserPerLevel)> Skills { get; }

        /// <summary>
        /// Gets the phrases that this monster type uses.
        /// </summary>
        IList<string> Phrases { get; }

        /// <summary>
        /// Gets the composition of the inventory that this type of monster has a chance to be created with.
        /// </summary>
        IList<(ushort typeId, byte maxAmount, ushort chance)> InventoryComposition { get; }

        /// <summary>
        /// Gets this type of monster outfit.
        /// </summary>
        Outfit Outfit { get; }

        /// <summary>
        /// Gets this type of monster's corpse item type id.
        /// </summary>
        ushort Corpse { get; }

        /// <summary>
        /// Gets the maximum hitpoints that this monster type starts with.
        /// </summary>
        ushort MaxHitpoints { get; }

        /// <summary>
        /// Gets the type of blood of this monster type.
        /// </summary>
        BloodType BloodType { get; }

        /// <summary>
        /// Gets the base movement speed for this type of monster.
        /// </summary>
        ushort BaseSpeed { get; }

        /// <summary>
        /// Gets the maximum capacity for this type of monster.
        /// </summary>
        ushort Capacity { get; }

        /// <summary>
        /// Gets the fighting strategy of this type of monster.
        /// </summary>
        (byte switchToClosest, byte switchToLowestHp, byte switchToHigestDmgDealt, byte randomSwitch) Strategy { get; }

        /// <summary>
        /// Gets the base attack for this type of monster.
        /// </summary>
        ushort BaseAttack { get; }

        /// <summary>
        /// Gets the base defense for this type of monster.
        /// </summary>
        ushort BaseDefense { get; }

        /// <summary>
        /// Gets the base armor rating for this type of monster.
        /// </summary>
        ushort BaseArmorRating { get; }
    }
}
