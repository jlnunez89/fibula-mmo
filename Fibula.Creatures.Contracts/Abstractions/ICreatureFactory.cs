// -----------------------------------------------------------------
// <copyright file="ICreatureFactory.cs" company="2Dudes">
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
    using Fibula.Server.Contracts.Abstractions;

    /// <summary>
    /// Interface for an <see cref="ICreature"/> factory.
    /// </summary>
    public interface ICreatureFactory : IThingFactory
    {
        /// <summary>
        /// Creates a new implementation instance of <see cref="ICreature"/> depending on the chosen type.
        /// </summary>
        /// <param name="creationArguments">The creation arguments for the new creature.</param>
        /// <returns>A new instance of the chosen <see cref="ICreature"/> implementation.</returns>
        ICreature CreateCreature(IThingCreationArguments creationArguments);
    }
}
