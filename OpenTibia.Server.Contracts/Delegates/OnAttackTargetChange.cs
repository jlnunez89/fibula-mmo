// -----------------------------------------------------------------
// <copyright file="OnAttackTargetChange.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Delegates
{
    using OpenTibia.Server.Contracts.Abstractions;

    /// <summary>
    /// Delegate meant for when a combatant changes attack targets.
    /// </summary>
    /// <param name="combatant">The combatant that changed target.</param>
    /// <param name="oldTargetId">The id of the old target.</param>
    /// <param name="newTargetId">The id of the new target.</param>
    public delegate void OnAttackTargetChange(ICombatant combatant, uint oldTargetId, uint newTargetId);
}
