// -----------------------------------------------------------------
// <copyright file="OnCreatureSeen.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Contracts.Delegates
{
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Abstractions;

    /// <summary>
    /// Delegate meant for when a creature sees a new creature.
    /// </summary>
    /// <param name="creature">The creature that sees the other.</param>
    /// <param name="creatureSeen">The creature that was seen.</param>
    public delegate void OnCreatureSeen(ICreatureThatSensesOthers creature, ICreature creatureSeen);
}
