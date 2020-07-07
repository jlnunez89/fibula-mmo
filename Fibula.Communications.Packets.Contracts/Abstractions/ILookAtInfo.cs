// -----------------------------------------------------------------
// <copyright file="ILookAtInfo.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Packets.Contracts.Abstractions
{
    using Fibula.Common.Contracts.Structs;
    using Fibula.Communications.Contracts.Abstractions;

    /// <summary>
    /// Interface for a look at information.
    /// </summary>
    public interface ILookAtInfo : IIncomingPacket
    {
        /// <summary>
        /// Gets the location we're looking at.
        /// </summary>
        Location Location { get; }

        /// <summary>
        /// Gets the id of the thing.
        /// </summary>
        ushort ThingId { get; }

        /// <summary>
        /// Gets the stack position we're looking at.
        /// </summary>
        byte StackPosition { get; }
    }
}
