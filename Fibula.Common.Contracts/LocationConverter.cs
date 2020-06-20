// -----------------------------------------------------------------
// <copyright file="LocationConverter.cs" company="2Dudes">
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
    using Fibula.Common.Utilities;
    using Fibula.Server.Contracts.Structs;

    /// <summary>
    /// Class that represents a converter for a location.
    /// </summary>
    internal class LocationConverter : IConverter
    {
        /// <summary>
        /// Converts a string into a <see cref="Location"/>.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <returns>The location converted.</returns>
        public object Convert(string value)
        {
            value.ThrowIfNullOrWhiteSpace(nameof(value));

            var coordsArray = value.TrimStart('[').TrimEnd(']').Split(',');

            if (coordsArray.Length != 3)
            {
                throw new ArgumentException($"Invalid location string '{value}'.");
            }

            return new Location
            {
                X = System.Convert.ToInt32(coordsArray[0]),
                Y = System.Convert.ToInt32(coordsArray[1]),
                Z = System.Convert.ToSByte(coordsArray[2]),
            };
        }
    }
}