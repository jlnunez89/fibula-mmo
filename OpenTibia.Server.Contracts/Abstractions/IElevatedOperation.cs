// -----------------------------------------------------------------
// <copyright file="IElevatedOperation.cs" company="2Dudes">
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
    /// Interface for an elevated game operation.
    /// </summary>
    public interface IElevatedOperation : IOperation
    {
        /// <summary>
        /// Gets the reference to this operation's context.
        /// </summary>
        new IElevatedOperationContext Context { get; }
    }
}
