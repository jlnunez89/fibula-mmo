// -----------------------------------------------------------------
// <copyright file="IPlayerCreationMetadata.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Creatures.Contracts.Abstractions
{
    using Fibula.Client.Contracts.Abstractions;

    /// <summary>
    /// Interface for player creation metadata information.
    /// </summary>
    public interface IPlayerCreationMetadata : ICreatureCreationMetadata
    {
        /// <summary>
        /// Gets the client that this player links to.
        /// </summary>
        IClient Client { get; }
    }
}
