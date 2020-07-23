// -----------------------------------------------------------------
// <copyright file="OnSkillAdvanced.cs" company="2Dudes">
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
    using Fibula.Data.Entities.Contracts.Enumerations;

    /// <summary>
    /// Delegate meant for a skill level advancement.
    /// </summary>
    /// <param name="skillType">The type of skill that advanced.</param>
    public delegate void OnSkillAdvanced(SkillType skillType);
}
