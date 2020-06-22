// -----------------------------------------------------------------
// <copyright file="OperationMessage.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Operations
{
    /// <summary>
    /// Static class that contains common operation messages.
    /// </summary>
    public static class OperationMessage
    {
        /// <summary>
        /// The operation is not possible at the moment.
        /// </summary>
        public const string NotPossible = "Sorry, not possible.";

        /// <summary>
        /// The operation cannot be completed because the player is too far to perform it.
        /// </summary>
        public const string TooFarAway = "You are too far away.";

        /// <summary>
        /// The operation cannot be completed because the destination of the movement is too far to perform it.
        /// </summary>
        public const string DestinationTooFarAway = "The destination is too far away.";

        /// <summary>
        /// The operation cannot be completed because the throw angle is not allowed.
        /// </summary>
        public const string MayNotThrowThere = "You may not throw there.";

        /// <summary>
        /// The operation cannot be completed because the destination is obstructed.
        /// </summary>
        public const string NotEnoughRoom = "There is not enough room.";

        /// <summary>
        /// The operation cannot be completed because the thing cannot be moved or the player does not have enough privilege to do it.
        /// </summary>
        public const string MayNotMoveThis = "You may not move this.";

        /// <summary>
        /// The operation cannot be completed because the desired quantity is greater than what's available at the source.
        /// </summary>
        public const string NotEnoughQuantity = "There is not enough quantity to move.";

        /// <summary>
        /// The operation cannot be completed because the player no longer has access to the source container.
        /// </summary>
        public const string MustFirstOpenThatContainer = "You must first open the container.";

        /// <summary>
        /// The operation cannot be completed because there is no way for the player to get there.
        /// </summary>
        public const string ThereIsNoWay = "There is no way.";
    }
}
