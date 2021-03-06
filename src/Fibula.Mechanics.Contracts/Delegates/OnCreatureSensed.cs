﻿// -----------------------------------------------------------------
// <copyright file="OnCreatureSensed.cs" company="2Dudes">
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
    /// Delegate meant for when a creature senses a new creature.
    /// </summary>
    /// <param name="creature">The creature that senses the other.</param>
    /// <param name="creatureSensed">The creature that was sensed.</param>
    public delegate void OnCreatureSensed(ICreatureThatSensesOthers creature, ICreature creatureSensed);
}
