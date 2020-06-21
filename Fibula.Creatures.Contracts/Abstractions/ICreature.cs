// -----------------------------------------------------------------
// <copyright file="ICreature.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Creatures.Contracts.Abstractions
{
    using System;
    using Fibula.Creatures.Contracts.Enumerations;
    using Fibula.Creatures.Contracts.Structs;
    using Fibula.Server.Contracts.Abstractions;
    using Fibula.Server.Contracts.Enumerations;
    using Fibula.Server.Contracts.Structs;

    /// <summary>
    /// Interface for all creatures in the game.
    /// </summary>
    public interface ICreature : IThing, IThingContainer, IEquatable<ICreature>
    {
        /// <summary>
        /// The id for things that are creatures.
        /// </summary>
        public const ushort CreatureThingId = 0x63;

        /// <summary>
        /// Gets the creature's in-game id.
        /// </summary>
        uint Id { get; }

        /// <summary>
        /// Gets the creature's blood type.
        /// </summary>
        BloodType Blood { get; }

        /// <summary>
        /// Gets the article in the name of the creature.
        /// </summary>
        string Article { get; }

        /// <summary>
        /// Gets the name of the creature.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the creature's corpse.
        /// </summary>
        ushort Corpse { get; }

        /// <summary>
        /// Gets the creature's current hitpoints.
        /// </summary>
        ushort Hitpoints { get; }

        /// <summary>
        /// Gets the creature's maximum hitpoints.
        /// </summary>
        ushort MaxHitpoints { get; }

        /// <summary>
        /// Gets the creature's current manapoints.
        /// </summary>
        ushort Manapoints { get; }

        /// <summary>
        /// Gets the creature's maximum manapoints.
        /// </summary>
        ushort MaxManapoints { get; }

        /// <summary>
        /// Gets the creature's strength value for carrying stuff.
        /// </summary>
        decimal CarryStrength { get; }

        /// <summary>
        /// Gets the creature's outfit.
        /// </summary>
        Outfit Outfit { get; }

        /// <summary>
        /// Gets the creatures facing direction.
        /// </summary>
        Direction Direction { get; }

        /// <summary>
        /// Gets this creature's emitted light level.
        /// </summary>
        byte EmittedLightLevel { get; }

        /// <summary>
        /// Gets this creature's emitted light color.
        /// </summary>
        byte EmittedLightColor { get; }

        /// <summary>
        /// Gets this creature's movement speed.
        /// </summary>
        ushort Speed { get; }

        /// <summary>
        /// Gets this creature's flags.
        /// </summary>
        uint Flags { get; }

        /// <summary>
        /// Gets a value indicating whether this creature is currently thinking.
        /// </summary>
        bool IsThinking { get; }

        /// <summary>
        /// Gets a value indicating whether this creature is currently invisible from any source.
        /// </summary>
        // TODO: rethink this one? might want to make creatures invisible depending on who's querying.
        bool IsInvisible { get; }

        /// <summary>
        /// Gets a value indicating whether this creature can perceive all invisible creatures.
        /// </summary>
        // TODO: rethink this one?
        bool CanSeeInvisible { get; }

        /// <summary>
        /// Gets this creature's skull status.
        /// </summary>
        // TODO: implement or decide what to do with this.
        byte Skull { get; }

        /// <summary>
        /// Gets this creature's shield status.
        /// </summary>
        // TODO: implement or decide what to do with this.
        byte Shield { get; }

        /// <summary>
        /// Gets the inventory for the entity.
        /// </summary>
        IInventory Inventory { get; }

        /// <summary>
        /// Checks if this creature can see a given creature.
        /// </summary>
        /// <param name="creature">The creature to check against.</param>
        /// <returns>True if this creature can see the given creature, false otherwise.</returns>
        bool CanSee(ICreature creature);

        /// <summary>
        /// Checks if this creature can see a given location.
        /// </summary>
        /// <param name="location">The location to check against.</param>
        /// <returns>True if this creature can see the given location, false otherwise.</returns>
        bool CanSee(Location location);

        /// <summary>
        /// Turns this creature to a given direction.
        /// </summary>
        /// <param name="direction">The direction to turn the creature to.</param>
        void TurnToDirection(Direction direction);

        /// <summary>
        /// Attempts to set this creature's <see cref="Outfit"/>.
        /// </summary>
        /// <param name="outfit">The new outfit to change to.</param>
        void SetOutfit(Outfit outfit);
    }
}
