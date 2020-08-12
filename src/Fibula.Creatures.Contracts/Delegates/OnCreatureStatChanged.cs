// -----------------------------------------------------------------
// <copyright file="OnCreatureStatChanged.cs" company="2Dudes">
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

    /// <summary>
    /// Delegate meant for a stat that changed on a creature.
    /// </summary>
    /// <param name="creature">The creature for which the stat changed.</param>
    /// <param name="statThatChanged">The stat that changed.</param>
    /// <param name="previousValue">The previous stat value.</param>
    /// <param name="previousPercent">The previous percent for the stat.</param>
    public delegate void OnCreatureStatChanged(ICreature creature, IStat statThatChanged, uint previousValue, byte previousPercent);
}
