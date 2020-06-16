// -----------------------------------------------------------------
// <copyright file="IUseItemOnInfo.cs" company="2Dudes">
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
    /// Interface for an item use on information.
    /// </summary>
    public interface IUseItemOnInfo : IIncomingPacket
    {
        /// <summary>
        /// Gets the location from which the item is being used.
        /// </summary>
        Location FromLocation { get; }

        /// <summary>
        /// Gets the id of the item as seen by the client.
        /// </summary>
        ushort FromItemClientId { get; }

        /// <summary>
        /// Gets the position in the stack from which the item is being used.
        /// </summary>
        byte FromIndex { get; }

        /// <summary>
        /// Gets the location to which the item is being used.
        /// </summary>
        Location ToLocation { get; }

        /// <summary>
        /// Gets the id of the item as seen by the client.
        /// </summary>
        ushort ToItemClientId { get; }

        /// <summary>
        /// Gets the position in the stack to which the item is being used.
        /// </summary>
        byte ToIndex { get; }
    }
}