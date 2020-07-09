// -----------------------------------------------------------------
// <copyright file="AutoAttackOperationCreationArguments.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Operations.Arguments
{
    using System;
    using Fibula.Common.Utilities;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Enumerations;

    /// <summary>
    /// Class that represents creation arguments for a <see cref="AutoAttackOperation"/>.
    /// </summary>
    public class AutoAttackOperationCreationArguments : IOperationCreationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoAttackOperationCreationArguments"/> class.
        /// </summary>
        /// <param name="attacker">The attacker.</param>
        /// <param name="target">The target.</param>
        /// <param name="exhaustionCost">Optional. The exhaustion cost of this operation.</param>
        public AutoAttackOperationCreationArguments(ICombatant attacker, ICombatant target, TimeSpan exhaustionCost)
        {
            attacker.ThrowIfNull(nameof(attacker));
            target.ThrowIfNull(nameof(target));

            this.Attacker = attacker;
            this.Target = target;
            this.ExhaustionCost = exhaustionCost;
        }

        /// <summary>
        /// Gets the type of operation being created.
        /// </summary>
        public OperationType Type => OperationType.AutoAttack;

        /// <summary>
        /// Gets the id of the requestor of the operation.
        /// </summary>
        public uint RequestorId => this.Attacker.Id;

        /// <summary>
        /// Gets the creature that's attacking.
        /// </summary>
        public ICombatant Attacker { get; }

        /// <summary>
        /// Gets the creature that's being attacked.
        /// </summary>
        public ICombatant Target { get; }

        /// <summary>
        /// Gets the exhaustion cost if the attack is carried out.
        /// </summary>
        public TimeSpan ExhaustionCost { get; }
    }
}
