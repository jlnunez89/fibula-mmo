// -----------------------------------------------------------------
// <copyright file="IParsedAttribute.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Parsing.Contracts.Abstractions
{
    /// <summary>
    /// Interface for a parsed attribute.
    /// </summary>
    public interface IParsedAttribute
    {
        /// <summary>
        /// Gets or sets the attribute's name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the attribute's value.
        /// </summary>
        object Value { get; set; }
    }
}
