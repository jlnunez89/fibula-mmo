// -----------------------------------------------------------------
// <copyright file="ILookAtInfo.cs" company="2Dudes">
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
    /// Interface for a look at information.
    /// </summary>
    public interface ILookAtInfo
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
