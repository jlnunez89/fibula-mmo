// -----------------------------------------------------------------
// <copyright file="IHandlerSelector.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Contracts.Abstractions
{
    using OpenTibia.Communications.Contracts.Enumerations;

    /// <summary>
    /// Interface that contains methods to select the appropriate incoming packet handler.
    /// </summary>
    public interface IHandlerSelector
    {
        /// <summary>
        /// Gets the protocol type for which this handler works.
        /// </summary>
        OpenTibiaProtocolType ForProtocol { get; }

        /// <summary>
        /// Returns the most appropriate handler for the specified packet type.
        /// </summary>
        /// <param name="forType">The packet type to select the handler for.</param>
        /// <returns>An instance of an <see cref="IHandler"/> implementaion.</returns>
        IHandler SelectForType(byte forType);
    }
}