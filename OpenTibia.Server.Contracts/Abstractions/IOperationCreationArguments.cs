// -----------------------------------------------------------------
// <copyright file="IOperationCreationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Abstractions
{
    /// <summary>
    /// Interface for all operation creation arguments.
    /// </summary>
    public interface IOperationCreationArguments
    {
        /// <summary>
        /// Gets the id of the requestor of the operation, if any.
        /// </summary>
        uint RequestorId { get; }
    }
}