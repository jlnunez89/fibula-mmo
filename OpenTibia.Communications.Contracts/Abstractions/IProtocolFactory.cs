// -----------------------------------------------------------------
// <copyright file="IProtocolFactory.cs" company="2Dudes">
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
    /// Interface for a protocol factory.
    /// </summary>
    public interface IProtocolFactory
    {
        /// <summary>
        /// Creates an instance of an implementation of <see cref="IProtocol"/> depending on the provided type.
        /// </summary>
        /// <param name="type">The type of protocol to implement.</param>
        /// <returns>A new <see cref="IProtocol"/> implementation instance.</returns>
        IProtocol CreateForType(OpenTibiaProtocolType type);
    }
}