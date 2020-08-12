// -----------------------------------------------------------------
// <copyright file="IConverter.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Common.Contracts.Abstractions
{
    /// <summary>
    /// Inteface for a converter.
    /// </summary>
    public interface IConverter
    {
        /// <summary>
        /// Converts a string into a strong type (boxed into object) implementation.
        /// </summary>
        /// <param name="value">The string value to convert.</param>
        /// <returns>A boxed strongly typed implementation.</returns>
        object Convert(string value);
    }
}
