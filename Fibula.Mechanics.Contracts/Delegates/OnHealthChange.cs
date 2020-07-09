// -----------------------------------------------------------------
// <copyright file="OnHealthChange.cs" company="2Dudes">
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
    /// Delegate meant for when a combatant's health level changes.
    /// </summary>
    /// <param name="combatant">The combatant that had a health change.</param>
    /// <param name="oldHealthValue">The old health value before this change.</param>
    public delegate void OnHealthChange(ICombatant combatant, ushort oldHealthValue);
}
