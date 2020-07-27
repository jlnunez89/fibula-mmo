﻿// -----------------------------------------------------------------
// <copyright file="OnSkillLevelChanged.cs" company="2Dudes">
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
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Abstractions;

    /// <summary>
    /// Delegate meant for a skill level that changed on a skilled creature.
    /// </summary>
    /// <param name="skilledCreature">The creature for which the skill changed.</param>
    /// <param name="skillThatChanged">The skill that changed.</param>
    /// <param name="previousLevel">The previous skill level.</param>
    public delegate void OnSkillLevelUpdated(ISkilledCreature skilledCreature, ISkill skillThatChanged, uint previousLevel);
}