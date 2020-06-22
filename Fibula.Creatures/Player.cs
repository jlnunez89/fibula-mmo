﻿// -----------------------------------------------------------------
// <copyright file="Player.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Creatures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fibula.Client.Contracts.Abstractions;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Creatures.Contracts.Structs;

    /// <summary>
    /// Class that represents all players in the game.
    /// </summary>
    public class Player : Creature, IPlayer
    {
        /// <summary>
        /// The limit of creatures that a player's client can keep track of.
        /// </summary>
        private const int KnownCreatureLimit = 150;

        /// <summary>
        /// Stores the set of creatures that are known to this player.
        /// </summary>
        private readonly IDictionary<uint, long> knownCreatures;

        //private ChaseMode chaseMode;

        //private FightMode fightMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="client">The client to associate this player to.</param>
        /// <param name="characterId">The id of the character that this player represents.</param>
        /// <param name="name">The name of the player.</param>
        /// <param name="maxHitpoints">The maximum number of hitpoints that the player starts with.</param>
        /// <param name="maxManapoints">The maximum number of manapoints that the player starts with.</param>
        /// <param name="corpse">The id of the corpse for the player.</param>
        /// <param name="hitpoints">Optional. The number of hitpoints that the player starts with. Defaults to <paramref name="maxHitpoints"/>.</param>
        /// <param name="manapoints">Optional. The number of manapoints that the player starts with. Defaults to <paramref name="maxManapoints"/>.</param>
        public Player(
            IClient client,
            string characterId,
            string name,
            ushort maxHitpoints,
            ushort maxManapoints,
            ushort corpse,
            ushort hitpoints = 0,
            ushort manapoints = 0)
            : base(name, string.Empty, maxHitpoints, maxManapoints, corpse, hitpoints, manapoints)
        {
            client.ThrowIfNull(nameof(client));
            characterId.ThrowIfNullOrWhiteSpace(nameof(characterId));

            this.knownCreatures = new Dictionary<uint, long>();

            this.Client = client;
            this.CharacterId = characterId;

            this.Outfit = new Outfit
            {
                Id = 128,
                Head = 114,
                Body = 114,
                Legs = 114,
                Feet = 114,
            };

            this.EmittedLightLevel = (byte)LightLevels.Torch;
            this.EmittedLightColor = (byte)LightColors.Orange;
            this.CarryStrength = 150;

            this.SoulPoints = 0;

            //this.Skills[SkillType.Experience] = new Skill(SkillType.Experience, 1, 1.0, 100, 100, 150);
            //this.Skills[SkillType.Magic] = new Skill(SkillType.Magic, 1, 1.0, 10, 1, 150);
            //this.Skills[SkillType.NoWeapon] = new Skill(SkillType.NoWeapon, 10, 1.0, 10, 10, 150);
            //this.Skills[SkillType.Axe] = new Skill(SkillType.Axe, 10, 1.0, 10, 10, 150);
            //this.Skills[SkillType.Club] = new Skill(SkillType.Club, 10, 1.0, 10, 10, 150);
            //this.Skills[SkillType.Sword] = new Skill(SkillType.Sword, 10, 1.0, 10, 10, 150);
            //this.Skills[SkillType.Shield] = new Skill(SkillType.Shield, 10, 1.0, 10, 10, 150);
            //this.Skills[SkillType.Ranged] = new Skill(SkillType.Ranged, 10, 1.0, 10, 10, 150);
            //this.Skills[SkillType.Fishing] = new Skill(SkillType.Fishing, 10, 1.0, 10, 10, 150);

            this.Speed = this.CalculateMovementBaseSpeed();

            // this.VipList = new Dictionary<string, bool>();
            this.Inventory = new PlayerInventory(this);
        }

        /// <summary>
        /// Gets the player's character id.
        /// </summary>
        public string CharacterId { get; }

        /// <summary>
        /// Gets the player's permissions level.
        /// </summary>
        public byte PermissionsLevel { get; }

        /// <summary>
        /// Gets a value indicating whether this player can be moved by others.
        /// </summary>
        public override bool CanBeMoved => this.PermissionsLevel == 0;

        /// <summary>
        /// Gets the player's soul points.
        /// </summary>
        // TODO: nobody likes soulpoints... figure out what to do with them :)
        public byte SoulPoints { get; }

        /// <summary>
        /// Gets a value indicating whether this player is allowed to logout.
        /// </summary>
        public bool IsAllowedToLogOut => true; // this.AutoAttackTarget == null;

        /// <summary>
        /// Gets or sets the inventory for the player.
        /// </summary>
        public sealed override IInventory Inventory { get; protected set; }

        ///// <summary>
        ///// Gets the range that the auto attack has.
        ///// </summary>
        //public override byte AutoAttackRange => this.Inventory.EquipmentAttackRange;

        ///// <summary>
        ///// Gets the attack power of this combatant.
        ///// </summary>
        //public override ushort AttackPower => this.Inventory.EquipmentAttackPower;

        ///// <summary>
        ///// Gets the defense power of this combatant.
        ///// </summary>
        //public override ushort DefensePower => this.Inventory.EquipmentDefensePower;

        ///// <summary>
        ///// Gets the armor rating of this combatant.
        ///// </summary>
        //public override ushort ArmorRating => this.Inventory.EquipmentArmorRating;

        ///// <summary>
        ///// Gets or sets the chase mode selected by this combatant.
        ///// </summary>
        //public override ChaseMode ChaseMode
        //{
        //    get
        //    {
        //        return this.chaseMode;
        //    }

        //    set
        //    {
        //        var oldMode = this.chaseMode;

        //        this.chaseMode = value;

        //        if (oldMode != this.chaseMode)
        //        {
        //            this.InvokeChaseModeChanged(oldMode);
        //        }
        //    }
        //}

        ///// <summary>
        ///// Gets or sets the fight mode selected by this combatant.
        ///// </summary>
        //public override FightMode FightMode
        //{
        //    get
        //    {
        //        return this.fightMode;
        //    }

        //    set
        //    {
        //        var oldMode = this.fightMode;

        //        this.fightMode = value;

        //        if (oldMode != this.fightMode)
        //        {
        //            this.InvokeFightModeChanged(oldMode);
        //        }
        //    }
        //}

        /// <summary>
        /// Gets a value indicating whether this player is thinking, which is always true.
        /// </summary>
        public override bool IsThinking => true;

        public IClient Client { get; }

        /// <summary>
        /// Checks if this player knows the given creature.
        /// </summary>
        /// <param name="creatureId">The id of the creature to check.</param>
        /// <returns>True if the player knows the creature, false otherwise.</returns>
        public bool KnowsCreatureWithId(uint creatureId)
        {
            return this.knownCreatures.ContainsKey(creatureId);
        }

        /// <summary>
        /// Adds the given creature to this player's known collection.
        /// </summary>
        /// <param name="creatureId">The id of the creature to add to the known creatures collection.</param>
        public void AddKnownCreature(uint creatureId)
        {
            try
            {
                this.knownCreatures[creatureId] = DateTimeOffset.UtcNow.Ticks;
            }
            catch
            {
                // happens when 2 try to add at the same time, which we don't care about.
            }
        }

        /// <summary>
        /// Chooses a creature to remove from this player's known creatures collection, if it has reached the collection size limit.
        /// </summary>
        /// <returns>The id of the chosen creature, if any, or <see cref="uint.MinValue"/> if no creature was chosen.</returns>
        public uint ChooseCreatureToRemoveFromKnownSet()
        {
            // If the buffer is full we need to choose a victim.
            while (this.knownCreatures.Count == KnownCreatureLimit)
            {
                // ToList() prevents modifiying an enumerating collection in the rare case we hit an exception down there.
                foreach (var candidate in this.knownCreatures.OrderBy(kvp => kvp.Value).ToList())
                {
                    if (this.knownCreatures.Remove(candidate.Key))
                    {
                        return candidate.Key;
                    }
                }
            }

            return uint.MinValue;
        }

        ///// <summary>
        ///// Sets a <see cref="ICombatant"/> now in view for this combatant.
        ///// </summary>
        ///// <param name="otherCombatant">The other combatant, now in view.</param>
        //public override void CombatantNowInView(ICombatant otherCombatant)
        //{
        //    return;
        //}

        ///// <summary>
        ///// Sets a <see cref="ICombatant"/> as no longer in view for this combatant.
        ///// </summary>
        ///// <param name="otherCombatant">The other combatant, now in view.</param>
        //public override void CombatantNoLongerInView(ICombatant otherCombatant)
        //{
        //    return;
        //}

        private ushort CalculateMovementBaseSpeed()
        {
            var expLevel = 1; // this.Skills.TryGetValue(SkillType.Experience, out ISkill expSkill) ? expSkill.Level : 0;

            return (ushort)(220 + (2 * (expLevel - 1)));
        }
    }
}
