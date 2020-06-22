// -----------------------------------------------------------------
// <copyright file="RestoreCombatCreditOperationCreationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Operations.Arguments
{
    using Fibula.Common.Utilities;
    using Fibula.Server.Contracts.Abstractions;
    using Fibula.Server.Contracts.Enumerations;
    using Fibula.Server.Operations.Contracts.Abstractions;

    /// <summary>
    /// Class that represents creation arguments for an <see cref="RestoreCombatCreditOperation"/>.
    /// </summary>
    public class RestoreCombatCreditOperationCreationArguments : IOperationCreationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RestoreCombatCreditOperationCreationArguments"/> class.
        /// </summary>
        /// <param name="requestorId"></param>
        /// <param name="combatant"></param>
        /// <param name="combatCreditType"></param>
        public RestoreCombatCreditOperationCreationArguments(uint requestorId, ICombatant combatant, CombatCreditType combatCreditType)
        {
            combatant.ThrowIfNull(nameof(combatant));

            this.RequestorId = requestorId;
            this.Combatant = combatant;
            this.CreditType = combatCreditType;
        }

        public uint RequestorId { get; }

        public ICombatant Combatant { get; }

        public CombatCreditType CreditType { get; }
    }
}
