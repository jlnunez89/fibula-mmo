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
        /// <param name="requestorId"></param>
        /// <param name="fightMode"></param>
        /// <param name="chaseMode"></param>
        /// <param name="safeModeOn"></param>
        public ChangeModesOperationCreationArguments(uint requestorId, FightMode fightMode, ChaseMode chaseMode, bool safeModeOn)
        {
            this.RequestorId = requestorId;

            this.FightMode = fightMode;
            this.ChaseMode = chaseMode;
            this.IsSafeModeOn = safeModeOn;
        }

        public OperationType Type => OperationType.ChangeModes;

        /// <summary>
        /// Gets the id of the requestor of the operation.
        /// </summary>
        public uint RequestorId { get; }

        public FightMode FightMode { get; }

        public ChaseMode ChaseMode { get; }

        public bool IsSafeModeOn { get; }
    }
}
