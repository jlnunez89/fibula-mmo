// -----------------------------------------------------------------
// <copyright file="Player.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Creatures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fibula.Client.Contracts.Abstractions;
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Data.Entities.Contracts.Enumerations;
    using Fibula.Data.Entities.Contracts.Structs;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Structs;

    /// <summary>
    /// Class that represents all players in the game.
    /// </summary>
    public class Player : CombatantCreature, IPlayer
    {
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

            this.Client = client;
            this.Client.PlayerId = this.Id;

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

            this.InitializeSkills();

            this.BaseSpeed = 220;
            this.Speed = this.CalculateMovementSpeed();

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
        // TODO: nobody likes soulpoints... figure out what to do with them.
        public byte SoulPoints { get; }

        /// <summary>
        /// Gets a value indicating whether this player is allowed to logout.
        /// </summary>
        public bool IsAllowedToLogOut => this.AutoAttackTarget == null;

        /// <summary>
        /// Gets or sets the inventory for the player.
        /// </summary>
        public sealed override IInventory Inventory { get; protected set; }

        /// <summary>
        /// Gets the range that the auto attack has.
        /// </summary>
        public override byte AutoAttackRange => 1;

        /// <summary>
        /// Gets the client associated to this player.
        /// </summary>
        public IClient Client { get; }

        /// <summary>
        /// Gets or sets this player's speed.
        /// </summary>
        public override ushort Speed { get; protected set; }

        /// <summary>
        /// Gets the collection of tracked combatants.
        /// </summary>
        public override IEnumerable<ICombatant> TrackedCombatants => Enumerable.Empty<ICombatant>();

        /// <summary>
        /// Starts tracking another <see cref="ICombatant"/>.
        /// </summary>
        /// <param name="otherCombatant">The other combatant, now in view.</param>
        public override void StartTrackingCombatant(ICombatant otherCombatant)
        {
        }

        /// <summary>
        /// Stops tracking another <see cref="ICombatant"/>.
        /// </summary>
        /// <param name="otherCombatant">The other combatant, now in view.</param>
        public override void StopTrackingCombatant(ICombatant otherCombatant)
        {
        }

        /// <summary>
        /// Applies damage modifiers to the damage information provided.
        /// </summary>
        /// <param name="damageInfo">The damage information.</param>
        protected override void ApplyDamageModifiers(ref DamageInfo damageInfo)
        {
            var rng = new Random();

            // 75% chance to block it?
            if (this.AutoDefenseCredits > 0 && rng.Next(4) > 0)
            {
                damageInfo.Effect = AnimatedEffect.Puff;
                damageInfo.Damage = 0;
            }

            // 25% chance to hit the armor...
            if (rng.Next(4) == 0)
            {
                damageInfo.Effect = AnimatedEffect.SparkYellow;
                damageInfo.Damage = 0;
            }

            if (damageInfo.Damage > 0 && damageInfo.Dealer != null && damageInfo.Dealer is IPlayer)
            {
                damageInfo.Damage = (int)Math.Ceiling((decimal)damageInfo.Damage / 2);
            }
        }

        /// <summary>
        /// Calculates the base movement speed of the player.
        /// </summary>
        /// <returns>The base movement speed of the player.</returns>
        protected override ushort CalculateMovementSpeed()
        {
            var expLevel = this.Skills.TryGetValue(SkillType.Experience, out ISkill expSkill) ? expSkill.Level : 0;

            return (ushort)(this.BaseSpeed + (2 * (expLevel - 1)));
        }

        private void InitializeSkills()
        {
            this.Skills[SkillType.Experience] = new Skill(SkillType.Experience, 1, rate: 1.1, 100, 20, 150);
            this.Skills[SkillType.Experience].Advanced += this.RaiseSkillLevelAdvance;
            this.Skills[SkillType.Experience].PercentChanged += this.RaiseSkillPercentChange;

            this.Skills[SkillType.Magic] = new Skill(SkillType.Magic, 0, rate: 1.1, 10, 0, 150);
            this.Skills[SkillType.Magic].Advanced += this.RaiseSkillLevelAdvance;
            this.Skills[SkillType.Magic].PercentChanged += this.RaiseSkillPercentChange;

            this.Skills[SkillType.NoWeapon] = new Skill(SkillType.NoWeapon, 10, rate: 1.1, 10, 10, 150);
            this.Skills[SkillType.NoWeapon].Advanced += this.RaiseSkillLevelAdvance;
            this.Skills[SkillType.NoWeapon].PercentChanged += this.RaiseSkillPercentChange;

            this.Skills[SkillType.Axe] = new Skill(SkillType.Axe, 10, rate: 1.1, 10, 10, 150);
            this.Skills[SkillType.Axe].Advanced += this.RaiseSkillLevelAdvance;
            this.Skills[SkillType.Axe].PercentChanged += this.RaiseSkillPercentChange;

            this.Skills[SkillType.Club] = new Skill(SkillType.Club, 10, rate: 1.1, 10, 10, 150);
            this.Skills[SkillType.Club].Advanced += this.RaiseSkillLevelAdvance;
            this.Skills[SkillType.Club].PercentChanged += this.RaiseSkillPercentChange;

            this.Skills[SkillType.Sword] = new Skill(SkillType.Sword, 10, rate: 1.1, 10, 10, 150);
            this.Skills[SkillType.Sword].Advanced += this.RaiseSkillLevelAdvance;
            this.Skills[SkillType.Sword].PercentChanged += this.RaiseSkillPercentChange;

            this.Skills[SkillType.Shield] = new Skill(SkillType.Shield, 10, rate: 1.1, 10, 10, 150);
            this.Skills[SkillType.Shield].Advanced += this.RaiseSkillLevelAdvance;
            this.Skills[SkillType.Shield].PercentChanged += this.RaiseSkillPercentChange;

            this.Skills[SkillType.Ranged] = new Skill(SkillType.Ranged, 10, rate: 1.1, 10, 10, 150);
            this.Skills[SkillType.Ranged].Advanced += this.RaiseSkillLevelAdvance;
            this.Skills[SkillType.Ranged].PercentChanged += this.RaiseSkillPercentChange;

            this.Skills[SkillType.Fishing] = new Skill(SkillType.Fishing, 10, rate: 1.1, 10, 10, 150);
            this.Skills[SkillType.Fishing].Advanced += this.RaiseSkillLevelAdvance;
            this.Skills[SkillType.Fishing].PercentChanged += this.RaiseSkillPercentChange;
        }
    }
}
