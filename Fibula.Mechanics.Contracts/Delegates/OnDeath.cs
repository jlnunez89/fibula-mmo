// -----------------------------------------------------------------
// <copyright file="OnDeath.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Contracts.Delegates
{
    using Fibula.Mechanics.Contracts.Abstractions;

    /// <summary>
    /// Delegate meant for when a combatant dies.
    /// </summary>
    /// <param name="combatant">The combatant that died.</param>
    public delegate void OnDeath(ICombatant combatant);
}
