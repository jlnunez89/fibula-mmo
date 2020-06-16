// -----------------------------------------------------------------
// <copyright file="ThingExtensions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Contracts.Extensions
{
    using System;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Server.Contracts.Abstractions;

    /// <summary>
    /// Static class that contains extension methods for an <see cref="IThing"/>.
    /// </summary>
    public static class ThingExtensions
    {
        /// <summary>
        /// Gets the description of the thing as seen by the given player.
        /// </summary>
        /// <param name="forPlayer">The player as which to get the description.</param>
        /// <returns>The description string.</returns>
        public static string GetDescription(this IThing thing, IPlayer forPlayer)
        {
            throw new NotImplementedException();
        }
    }
}
