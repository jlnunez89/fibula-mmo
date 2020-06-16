// -----------------------------------------------------------------
// <copyright file="IAuctionResult.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Packets.Contracts.Abstractions
{
    /// <summary>
    /// Provides an interface for auction results.
    /// </summary>
    public interface IAuctionResult
    {
        /// <summary>
        /// Gets the house id.
        /// </summary>
        ushort HouseId { get; }

        /// <summary>
        /// Gets the id of the auction winner.
        /// </summary>
        uint WinnerId { get; }

        /// <summary>
        /// Gets the name of the auction winner.
        /// </summary>
        string WinnerName { get; }

        /// <summary>
        /// Gets the bid amount that won this auction.
        /// </summary>
        uint Bid { get; }
    }
}