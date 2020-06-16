// -----------------------------------------------------------------
// <copyright file="IOperationFactory.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Operations.Contracts.Abstractions
{
    using Fibula.Server.Operations.Contracts.Enumerations;
    using Serilog;

    /// <summary>
    /// Interface for factories of operations.
    /// </summary>
    public interface IOperationFactory
    {
        /// <summary>
        /// Gets the reference to the factory's logger.
        /// </summary>
        ILogger Logger { get; }

        /// <summary>
        /// Creates a new <see cref="IOperation"/> based on the type specified with the given arguments.
        /// </summary>
        /// <param name="type">The type of operation to create.</param>
        /// <param name="arguments">The arguments for creation.</param>
        /// <returns>A new instance of <see cref="IOperation"/>.</returns>
        IOperation Create(OperationType type, IOperationCreationArguments arguments);
    }
}
