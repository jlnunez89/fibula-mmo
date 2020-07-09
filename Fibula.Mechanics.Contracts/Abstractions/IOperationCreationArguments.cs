// -----------------------------------------------------------------
// <copyright file="IOperationCreationArguments.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Contracts.Abstractions
{
    using Fibula.Mechanics.Contracts.Enumerations;

    /// <summary>
    /// Interface for all operation creation arguments.
    /// </summary>
    public interface IOperationCreationArguments
    {
        /// <summary>
        /// Gets the type of operation being created.
        /// </summary>
        OperationType Type { get; }

        /// <summary>
        /// Gets the id of the requestor of the operation, if any.
        /// </summary>
        uint RequestorId { get; }
    }
}
