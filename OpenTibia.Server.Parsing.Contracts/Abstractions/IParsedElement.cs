// -----------------------------------------------------------------
// <copyright file="IParsedElement.cs" company="2Dudes">
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
    using System.Collections.Generic;

    /// <summary>
    /// Interface for a parsed element.
    /// </summary>
    public interface IParsedElement
    {
        /// <summary>
        /// Gets the id of the element.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Gets a value indicating whether  this element is a flag.
        /// </summary>
        bool IsFlag { get; }

        /// <summary>
        /// Gets the attributes of the element.
        /// </summary>
        IList<IParsedAttribute> Attributes { get; }
    }
}
