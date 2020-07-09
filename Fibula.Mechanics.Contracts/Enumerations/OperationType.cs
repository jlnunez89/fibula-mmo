// -----------------------------------------------------------------
// <copyright file="OperationType.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Contracts.Enumerations
{
    /// <summary>
    /// Class that enumerates the different types of operations available.
    /// </summary>
    public enum OperationType
    {
        /// <summary>
        /// A speech operation.
        /// </summary>
        Speech,

        /// <summary>
        /// An operation to cancel other operations.
        /// </summary>
        CancelOperations,

        /// <summary>
        /// An operation to change modes.
        /// </summary>
        ChangeModes,

        /// <summary>
        /// An operation to describe a thing.
        /// </summary>
        LookAt,

        /// <summary>
        /// A movement operation.
        /// </summary>
        Movement,

        /// <summary>
        /// The operation of logging in.
        /// </summary>
        LogIn,

        /// <summary>
        /// The operation of logging out.
        /// </summary>
        LogOut,

        /// <summary>
        /// An operation of a creature turn.
        /// </summary>
        Turn,

        /// <summary>
        /// The operationthat orchestrates auto attacking.
        /// </summary>
        AutoAttackOrchestrator,

        /// <summary>
        /// The actual attack operation when auto attacking.
        /// </summary>
        AutoAttack,

        /// <summary>
        /// An operation that restores combat credits to a combatant.
        /// </summary>
        RestoreCombatCredit,

        /// <summary>
        /// The operation that orchestrates auto walking.
        /// </summary>
        AutoWalkOrchestrator,

        /// <summary>
        /// An operation that creates an item.
        /// </summary>
        CreateItem,

        /// <summary>
        /// An operation to deal with a creature's death.
        /// </summary>
        Death,

        /*
        ContainerOpen,
        ContainerClose,
        ContainerMoveUp,
        UseItem,
        UseItemOn,

        ChangeItem,
        DeleteItem,
        PlaceCreature,
        RemoveCreature,
        SpawnMonsters,
        */
    }
}
