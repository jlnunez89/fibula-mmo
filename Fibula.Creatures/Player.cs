// -----------------------------------------------------------------
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

            /*
            this.Skills[SkillType.Experience] = new Skill(SkillType.Experience, 1, 1.0, 100, 100, 150);
            this.Skills[SkillType.Magic] = new Skill(SkillType.Magic, 1, 1.0, 10, 1, 150);
            this.Skills[SkillType.NoWeapon] = new Skill(SkillType.NoWeapon, 10, 1.0, 10, 10, 150);
            this.Skills[SkillType.Axe] = new Skill(SkillType.Axe, 10, 1.0, 10, 10, 150);
            this.Skills[SkillType.Club] = new Skill(SkillType.Club, 10, 1.0, 10, 10, 150);
            this.Skills[SkillType.Sword] = new Skill(SkillType.Sword, 10, 1.0, 10, 10, 150);
            this.Skills[SkillType.Shield] = new Skill(SkillType.Shield, 10, 1.0, 10, 10, 150);
            this.Skills[SkillType.Ranged] = new Skill(SkillType.Ranged, 10, 1.0, 10, 10, 150);
            this.Skills[SkillType.Fishing] = new Skill(SkillType.Fishing, 10, 1.0, 10, 10, 150);
            */

            this.Speed = this.CalculateMovementBaseSpeed();

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
        public bool IsAllowedToLogOut => true; // this.AutoAttackTarget == null;

        /// <summary>
        /// Gets or sets the inventory for the player.
        /// </summary>
        public sealed override IInventory Inventory { get; protected set; }

        /// <summary>
        /// Gets the client associated to this player.
        /// </summary>
        public IClient Client { get; }

        private ushort CalculateMovementBaseSpeed()
        {
            var expLevel = 1; // this.Skills.TryGetValue(SkillType.Experience, out ISkill expSkill) ? expSkill.Level : 0;

            return (ushort)(220 + (2 * (expLevel - 1)));
        }
    }
}
