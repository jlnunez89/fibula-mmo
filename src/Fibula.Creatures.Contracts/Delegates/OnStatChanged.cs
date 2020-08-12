// -----------------------------------------------------------------
// <copyright file="OnStatChanged.cs" company="2Dudes">
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
    using Fibula.Creatures.Contracts.Enumerations;

    /// <summary>
    /// Delegate meant for a stat changes.
    /// </summary>
    /// <param name="statType">The type of the stat that changed.</param>
    /// <param name="previousValue">The previous stat value.</param>
    /// <param name="previousPercent">The previous percentual value of the stat.</param>
    public delegate void OnStatChanged(CreatureStat statType, uint previousValue, byte previousPercent);
}
