// -----------------------------------------------------------------
// <copyright file="IUseItemInfo.cs" company="2Dudes">
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
    using Fibula.Communications.Contracts.Abstractions;
    using Fibula.Server.Contracts.Structs;

    /// <summary>
    /// Interface for an item use information.
    /// </summary>
    public interface IUseItemInfo : IIncomingPacket
    {
        /// <summary>
        /// Gets the location form which the item is being used.
        /// </summary>
        Location FromLocation { get; }

        /// <summary>
        /// Gets the position in the stack from which the item is being used.
        /// </summary>
        byte FromStackPos { get; }

        /// <summary>
        /// Gets the id of the item as seen by the client.
        /// </summary>
        ushort ItemClientId { get; }

        /// <summary>
        /// Gets the index of the item being used.
        /// </summary>
        byte Index { get; }
    }
}