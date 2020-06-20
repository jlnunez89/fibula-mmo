// -----------------------------------------------------------------
// <copyright file="IHasSkills.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Contracts.Abstractions
{
    using System.Collections.Generic;
    using Fibula.Server.Contracts.Enumerations;

    /// <summary>
    /// Interface for any entity in the game that has skills.
    /// </summary>
    public interface IHasSkills
    {
        /// <summary>
        /// Gets the current skills information for the entity.
        /// </summary>
        /// <remarks>
        /// The key is a <see cref="SkillType"/>, and the value is a <see cref="ISkill"/>.
        /// </remarks>
        IDictionary<SkillType, ISkill> Skills { get; }

        /// <summary>
        /// Calculates the current percentual value between current and target counts for the given skill.
        /// </summary>
        /// <param name="type">The type of skill to calculate for.</param>
        /// <returns>A value between [0, 99] representing the current percentual value.</returns>
        byte CalculateSkillPercent(SkillType type);
    }
}