// -----------------------------------------------------------------
// <copyright file="ChangeModesOperationCreationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Operations.Arguments
{
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Enumerations;

    /// <summary>
    /// Class that represents creation arguments for a <see cref="ChangeModesOperation"/>.
    /// </summary>
    public class ChangeModesOperationCreationArguments : IOperationCreationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeModesOperationCreationArguments"/> class.
        /// </summary>
        /// <param name="requestorId">The id of the creature requesting the operation.</param>
        /// <param name="fightMode">The fight mode to set.</param>
        /// <param name="chaseMode">The chase mode to set.</param>
        /// <param name="safeModeOn">The value to set for the safety lock.</param>
        public ChangeModesOperationCreationArguments(uint requestorId, FightMode fightMode, ChaseMode chaseMode, bool safeModeOn)
        {
            this.RequestorId = requestorId;

            this.FightMode = fightMode;
            this.ChaseMode = chaseMode;
            this.IsSafeModeOn = safeModeOn;
        }

        /// <summary>
        /// Gets the type of operation being created.
        /// </summary>
        public OperationType Type => OperationType.ChangeModes;

        /// <summary>
        /// Gets the id of the requestor of the operation.
        /// </summary>
        public uint RequestorId { get; }

        /// <summary>
        /// Gets the fight mode to set.
        /// </summary>
        public FightMode FightMode { get; }

        /// <summary>
        /// Gets the chase mode to set.
        /// </summary>
        public ChaseMode ChaseMode { get; }

        /// <summary>
        /// Gets a value indicating whether the safety lock is on.
        /// </summary>
        public bool IsSafeModeOn { get; }
    }
}
