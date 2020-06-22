// -----------------------------------------------------------------
// <copyright file="IMovementOperation.cs" company="2Dudes">
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
    using Fibula.Server.Contracts.Structs;

    /// <summary>
    /// Interface for a movement operation.
    /// </summary>
    public interface IMovementOperation : IOperation
    {
        /// <summary>
        /// Gets the location from which the movement happens.
        /// </summary>
        Location FromLocation { get; }

        /// <summary>
        /// Gets the location to which the movement happens.
        /// </summary>
        Location ToLocation { get; }
    }
}
