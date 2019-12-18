// -----------------------------------------------------------------
// <copyright file="IConverter.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Scripting
{
    /// <summary>
    /// Inteface for a converter.
    /// </summary>
    internal interface IConverter
    {
        /// <summary>
        /// Converts a string into a strong type (boxed into object) implementation.
        /// </summary>
        /// <param name="value">The string value to convert.</param>
        /// <returns>A boxed strongly typed implementation.</returns>
        object Convert(string value);
    }
}