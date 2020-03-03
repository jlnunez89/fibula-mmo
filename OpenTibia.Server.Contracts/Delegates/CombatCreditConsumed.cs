// -----------------------------------------------------------------
// <copyright file="CombatCreditConsumed.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Delegate meant for when a combatant's combat credits are consumed.
    /// </summary>
    /// <param name="combatant">The combatant.</param>
    /// <param name="creditType">The type of credit consumed.</param>
    /// <param name="amount">The amount of credits consumed.</param>
    public delegate void CombatCreditConsumed(ICombatant combatant, CombatCreditType creditType, byte amount);
}
