﻿// -----------------------------------------------------------------
// <copyright file="SkillLevelAdvance.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts
{
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Delegate meant for skill level advancement.
    /// </summary>
    /// <param name="skillType">The type of skill that advanced.</param>
    public delegate void SkillLevelAdvance(SkillType skillType);
}
