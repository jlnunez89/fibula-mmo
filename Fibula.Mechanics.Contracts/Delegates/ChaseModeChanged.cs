﻿// -----------------------------------------------------------------
// <copyright file="ChaseModeChanged.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Delegates
{
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Delegate meant for when a combatant changes chase modes.
    /// </summary>
    /// <param name="combatant">The combatant that changed target.</param>
    /// <param name="oldMode">The mode set before this change.</param>
    public delegate void ChaseModeChanged(ICombatant combatant, ChaseMode oldMode);
}
