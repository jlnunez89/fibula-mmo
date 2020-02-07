// -----------------------------------------------------------------
// <copyright file="IMovementOperation.cs" company="2Dudes">
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
    /// Interface for a movement operation.
    /// </summary>
    public interface IMovementOperation : IOperation
    {
        /// <summary>
        /// Gets the creature that is requesting the operation, if known.
        /// </summary>
        ICreature Requestor { get; }

        /// <summary>
        /// Gets the cylinder from which the movement happens.
        /// </summary>
        ICylinder FromCylinder { get; }

        /// <summary>
        /// Gets the cylinder to which the movement happens.
        /// </summary>
        ICylinder ToCylinder { get; }
    }
}
