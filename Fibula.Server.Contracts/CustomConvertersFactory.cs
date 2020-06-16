// -----------------------------------------------------------------
// <copyright file="CustomConvertersFactory.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Contracts
{
    using System;
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Server.Contracts.Structs;

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