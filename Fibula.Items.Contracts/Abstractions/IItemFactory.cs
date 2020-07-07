// -----------------------------------------------------------------
// <copyright file="IItemFactory.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Items.Contracts.Abstractions
{
    using Fibula.Common.Contracts.Abstractions;

    /// <summary>
    /// Interface for an item factory.
    /// </summary>
    public interface IItemFactory : IThingFactory
    {
        /// <summary>
        /// Creates a new <see cref="IItem"/>.
        /// </summary>
        /// <param name="creationArguments">The arguments for the <see cref="IItem"/> creation.</param>
        /// <returns>A new instance of the <see cref="IItem"/>.</returns>
        IItem CreateItem(IThingCreationArguments creationArguments);
    }
}
