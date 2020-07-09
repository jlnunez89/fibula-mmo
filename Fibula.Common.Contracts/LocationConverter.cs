// -----------------------------------------------------------------
// <copyright file="LocationConverter.cs" company="2Dudes">
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
    using Fibula.Common.Utilities;

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
