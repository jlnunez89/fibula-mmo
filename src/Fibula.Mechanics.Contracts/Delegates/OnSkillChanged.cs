// -----------------------------------------------------------------
// <copyright file="OnSkillChanged.cs" company="2Dudes">
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
    using Fibula.Data.Entities.Contracts.Enumerations;

    /// <summary>
    /// Delegate meant for a skill change.
    /// </summary>
    /// <param name="skillType">The type of skill that changed.</param>
    /// <param name="previousLevel">The previous skill level.</param>
    /// <param name="previousPercent">The previous percent of completion to next level.</param>
    /// <param name="countDelta">Optional. The delta in the count for this skill. Not always sent.</param>
    public delegate void OnSkillChanged(SkillType skillType, uint previousLevel, byte previousPercent, long? countDelta = null);
}
