// -----------------------------------------------------------------
// <copyright file="OnSkillPerecentualChanged.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common.Contracts.Delegates
{
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Abstractions;

    /// <summary>
    /// Delegate meant for a skill percentual that changed on a skilled creature.
    /// </summary>
    /// <param name="skilledCreature">The creature for which the skill changed.</param>
    /// <param name="skillThatChanged">The skill that changed.</param>
    public delegate void OnSkillPerecentualChanged(ISkilledCreature skilledCreature, ISkill skillThatChanged);
}
