// -----------------------------------------------------------------
// <copyright file="ICylinder.cs" company="2Dudes">
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
    using System.Collections.Generic;
    using OpenTibia.Server.Parsing.Contracts.Abstractions;
    using Serilog;

    /// <summary>
    /// Interface for all cylinders.
    /// </summary>
    public interface ICylinder : ILocatable
    {
        /// <summary>
        /// Forcefully adds parsed content elements to this cylinder.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="itemFactory">A reference to the item factory in use.</param>
        /// <param name="contentElements">The content elements to add.</param>
        void AddContent(ILogger logger, IItemFactory itemFactory, IEnumerable<IParsedElement> contentElements);

        /// <summary>
        /// Attempts to add a thing to this cylinder.
        /// </summary>
        /// <param name="itemFactory">A reference to the item factory in use.</param>
        /// <param name="thing">The thing to add to the cylinder.</param>
        /// <param name="index">Optional. The index at which to add the thing. Defaults to 0xFF, which instructs to add the thing at any free index.</param>
        /// <returns>A tuple with a value indicating whether the attempt was at least partially successful, and false otherwise. If the result was only partially successful, a remainder of the item may be returned.</returns>
        (bool result, IThing remainder) AddContent(IItemFactory itemFactory, IThing thing, byte index = 0xFF);

        /// <summary>
        /// Attempts to remove a thing from this cylinder.
        /// </summary>
        /// <param name="itemFactory">A reference to the item factory in use.</param>
        /// <param name="thing">The thing to remove from the cylinder.</param>
        /// <param name="index">Optional. The index from which to remove the thing. Defaults to 0xFF, which instructs to remove the thing if found at any index.</param>
        /// <param name="amount">Optional. The amount of the <paramref name="thing"/> to remove.</param>
        /// <returns>A tuple with a value indicating whether the attempt was at least partially successful, and false otherwise. If the result was only partially successful, a remainder of the item may be returned.</returns>
        (bool result, IThing remainder) RemoveContent(IItemFactory itemFactory, IThing thing, byte index = 0xFF, byte amount = 1);
    }
}