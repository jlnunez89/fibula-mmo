// -----------------------------------------------------------------
// <copyright file="OnDeath.cs" company="2Dudes">
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
    using Fibula.Mechanics.Contracts.Abstractions;

    /// <summary>
    /// Delegate meant for when a combatant dies.
    /// </summary>
    /// <param name="combatant">The combatant that died.</param>
    public delegate void OnDeath(ICombatant combatant);
}
