// -----------------------------------------------------------------
// <copyright file="CustomConvertersFactory.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts
{
    using System;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Scripting;

    /// <summary>
    /// Static class that represents a factory for converters.
    /// </summary>
    public static class CustomConvertersFactory
    {
        /// <summary>
        /// Gets a converter for the specified type.
        /// </summary>
        /// <param name="newType">The type for which to get a converter.</param>
        /// <returns>A new implementation of <see cref="IConverter"/>.</returns>
        internal static IConverter GetConverter(Type newType)
        {
            if (newType == typeof(Location))
            {
                return new LocationConverter();
            }

            return null;
        }
    }
}