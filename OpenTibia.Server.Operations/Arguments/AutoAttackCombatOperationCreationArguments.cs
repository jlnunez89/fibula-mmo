// -----------------------------------------------------------------
// <copyright file="AutoAttackCombatOperationCreationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Operations.Arguments
{
    using System;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Operations.Combat;

    /// <summary>
    /// Class that represents creation arguments for an <see cref="AutoAttackCombatOperation"/>.
    /// </summary>
    public class AutoAttackCombatOperationCreationArguments : IOperationCreationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoAttackCombatOperationCreationArguments"/> class.
        /// </summary>
        /// <param name="requestorId"></param>
        /// <param name="attacker"></param>
        /// <param name="target"></param>
        /// <param name="exhaustionCost"></param>
        public AutoAttackCombatOperationCreationArguments(uint requestorId, ICombatant attacker, ICombatant target, TimeSpan? exhaustionCost = null)
        {
            target.ThrowIfNull(nameof(target));

            this.RequestorId = requestorId;
            this.Attacker = attacker;
            this.Target = target;
            this.ExhaustionCost = exhaustionCost ?? TimeSpan.Zero;
        }

        /// <summary>
        /// Gets the id of the requestor of the operation.
        /// </summary>
        public uint RequestorId { get; }

        public ICombatant Attacker { get; }

        public ICombatant Target { get; }

        public TimeSpan ExhaustionCost { get; }
    }
}
