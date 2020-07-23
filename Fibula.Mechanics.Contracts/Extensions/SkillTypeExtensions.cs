// -----------------------------------------------------------------
// <copyright file="SkillTypeExtensions.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Contracts.Extensions
{
    using System;
    using Fibula.Data.Entities.Contracts.Enumerations;

    /// <summary>
    /// Helper class for extension methods of skill types.
    /// </summary>
    public static class SkillTypeExtensions
    {
        /// <summary>
        /// Gets a friendly name for the given skill type.
        /// </summary>
        /// <param name="skillType">The type of skill.</param>
        /// <returns>The friendly name.</returns>
        public static string ToFriendlySkillName(this SkillType skillType)
        {
            return skillType switch
            {
                SkillType.NoWeapon => "fist fighting",
                SkillType.Axe => "axe fighting",
                SkillType.Club => "club fighting",
                SkillType.Sword => "sword fighting",
                SkillType.Ranged => "distance fighting",
                SkillType.Shield => "shielding",
                SkillType.Fishing => "fishing",
                SkillType.Experience => "experience level",
                SkillType.Magic => "magic level",
                _ => throw new NotSupportedException($"Unsupported skill type {skillType} when attempting to get friendly name.")
            };
        }
    }
}
