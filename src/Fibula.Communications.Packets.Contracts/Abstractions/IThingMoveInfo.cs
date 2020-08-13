// -----------------------------------------------------------------
// <copyright file="IThingMoveInfo.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Communications.Packets.Contracts.Abstractions
{
    using Fibula.Common.Contracts.Structs;

    /// <summary>
    /// Interface for a thing movement information.
    /// </summary>
    public interface IThingMoveInfo
    {
        /// <summary>
        /// Gets the id of the thing, as seen by the client.
        /// </summary>
        ushort ThingClientId { get; }

        /// <summary>
        /// Gets the location from which the thing is being moved.
        /// </summary>
        Location FromLocation { get; }

        /// <summary>
        /// Gets the position in the stack at the location from which the thing is being moved.
        /// </summary>
        byte FromStackPos { get; }

        /// <summary>
        /// Gets the location to which the thing is being moved.
        /// </summary>
        Location ToLocation { get; }

        /// <summary>
        /// Gets the amount of the thing being moved.
        /// </summary>
        byte Amount { get; }
    }
}
