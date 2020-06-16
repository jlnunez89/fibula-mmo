// -----------------------------------------------------------------
// <copyright file="IConverter.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
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