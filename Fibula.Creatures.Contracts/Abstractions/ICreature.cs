// -----------------------------------------------------------------
// <copyright file="ICreature.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Creatures.Contracts.Abstractions
{
    using System;
    using System.Collections.Generic;
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Contracts.Structs;
    using Fibula.Creatures.Contracts.Enumerations;
    using Fibula.Creatures.Contracts.Structs;
    using Fibula.Data.Entities.Contracts.Enumerations;
    using Fibula.Data.Entities.Contracts.Structs;
    using Fibula.Mechanics.Contracts.Delegates;

    /// <summary>
    /// Interface for all creatures in the game.
    /// </summary>
    public interface ICreature : IThing, IThingContainer, IEquatable<ICreature>
    {
        /// <summary>
        /// Event triggered when this creature's stat has changed.
        /// </summary>
        event OnCreatureStatChanged StatChanged;

        /// <summary>
        /// Gets the creature's in-game id.
        /// </summary>
        uint Id { get; }

        /// <summary>
        /// Gets the creature's blood type.
        /// </summary>
        BloodType BloodType { get; }

        /// <summary>
        /// Gets the article in the name of the creature.
        /// </summary>
        string Article { get; }

        /// <summary>
        /// Gets the name of the creature.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the creature's corpse type id.
        /// </summary>
        ushort CorpseTypeId { get; }

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
        /// Gets the inventory for the entity.
        /// </summary>
        IInventory Inventory { get; }

        /// <summary>
        /// Gets a value indicating whether the creature is dead.
        /// </summary>
        bool IsDead { get; }

        /// <summary>
        /// Gets a value indicating whether this creature can walk.
        /// </summary>
        bool CanWalk { get; }

        /// <summary>
        /// Gets or sets the creature's outfit.
        /// </summary>
        Outfit Outfit { get; set; }

        /// <summary>
        /// Gets or sets the creature's facing direction.
        /// </summary>
        Direction Direction { get; set; }

        /// <summary>
        /// Gets or sets the creature's last move modifier.
        /// </summary>
        decimal LastMovementCostModifier { get; set; }

        /// <summary>
        /// Gets or sets this creature's walk plan.
        /// </summary>
        WalkPlan WalkPlan { get; set; }

        /// <summary>
        /// Gets the current stats information for the creature.
        /// </summary>
        /// <remarks>
        /// The key is a <see cref="CreatureStat"/>, and the value is an <see cref="IStat"/>.
        /// </remarks>
        IDictionary<CreatureStat, IStat> Stats { get; }

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
    }
}
