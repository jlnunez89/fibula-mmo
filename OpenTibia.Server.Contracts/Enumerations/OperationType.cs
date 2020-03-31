// -----------------------------------------------------------------
// <copyright file="OperationType.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Enumerations
{
    public enum OperationType
    {
        Thinking,
        Speech,

        // Actions
        ContainerOpen,
        ContainerClose,
        ContainerMoveUp,
        Turn,
        UseItem,
        UseItemOn,

        // Combat
        AutoAttack,
        RestoreCombatCredit,

        // Movement
        AutoWalk,
        Movement,

        // Elevated
        ChangeItem,
        CreateItem,
        DeleteItem,
        LogIn,
        LogOut,
        PlaceCreature,
        RemoveCreature,
        SpawnMonsters,
    }
}
