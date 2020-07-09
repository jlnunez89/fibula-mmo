// -----------------------------------------------------------------
// <copyright file="CustomConvertersFactory.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common.Contracts
{
    using System;
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Common.Contracts.Structs;

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
