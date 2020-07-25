// -----------------------------------------------------------------
// <copyright file="ISkilledCreature.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Contracts.Abstractions
{
    using System.Collections.Generic;
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Data.Entities.Contracts.Enumerations;
    using Fibula.Mechanics.Contracts.Delegates;

    /// <summary>
    /// Interface for any entity in the game that has skills.
    /// </summary>
    public interface ISkilledCreature : ICreature
    {
        /// <summary>
        /// Event triggered when this skilled creature advances a skill to the next level.
        /// </summary>
        event OnSkillLevelChanged SkillLevelChanged;

        /// <summary>
        /// Event triggered when this skilled creature skill percent changes.
        /// </summary>
        event OnSkillPerecentualChanged SkillPerecentualChanged;

        /// <summary>
        /// Gets the current skills information for the entity.
        /// </summary>
        /// <remarks>
        /// The key is a <see cref="SkillType"/>, and the value is a <see cref="ISkill"/>.
        /// </remarks>
        IDictionary<SkillType, ISkill> Skills { get; }
    }
}
