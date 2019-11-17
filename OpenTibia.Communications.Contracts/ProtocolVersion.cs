// -----------------------------------------------------------------
// <copyright file="ProtocolVersion.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Contracts
{
    /// <summary>
    /// Class that represents a protocol's version.
    /// </summary>
    public class ProtocolVersion
    {
        /// <summary>
        /// Gets or sets the numeric value of the version.
        /// </summary>
        public int Numeric { get; set; }

        /// <summary>
        /// Gets or sets the description of the version.
        /// </summary>
        public string Description { get; set; }
    }
}