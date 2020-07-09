// -----------------------------------------------------------------
// <copyright file="LookAtOperationCreationArguments.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Operations.Arguments
{
    using Fibula.Common.Contracts.Structs;
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Enumerations;

    /// <summary>
    /// Class that represents creation arguments for a <see cref="LookAtOperation"/>.
    /// </summary>
    public class LookAtOperationCreationArguments : IOperationCreationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LookAtOperationCreationArguments"/> class.
        /// </summary>
        /// <param name="thingId">The id of the thing to describe.</param>
        /// <param name="location">The location where the thing to describe is.</param>
        /// <param name="stackPosition">The position in the stack at the location of the thing to describe is.</param>
        /// <param name="playerToDescribeFor">The player to describe the thing for.</param>
        public LookAtOperationCreationArguments(ushort thingId, Location location, byte stackPosition, IPlayer playerToDescribeFor)
        {
            playerToDescribeFor.ThrowIfNull(nameof(playerToDescribeFor));

            this.RequestorId = playerToDescribeFor.Id;
            this.ThingId = thingId;
            this.Location = location;
            this.StackPosition = stackPosition;
            this.PlayerToDescribeFor = playerToDescribeFor;
        }

        /// <summary>
        /// Gets the type of operation being created.
        /// </summary>
        public OperationType Type => OperationType.LookAt;

        /// <summary>
        /// Gets the id of the requestor of the operation.
        /// </summary>
        public uint RequestorId { get; }

        /// <summary>
        /// Gets the id of the thing to describe.
        /// </summary>
        public ushort ThingId { get; }

        /// <summary>
        /// Gets the location where the thing to describe is.
        /// </summary>
        public Location Location { get; }

        /// <summary>
        /// Gets the position in the stack at the location of the thing to describe is.
        /// </summary>
        public byte StackPosition { get; }

        /// <summary>
        /// Gets the player to describe for.
        /// </summary>
        public IPlayer PlayerToDescribeFor { get; }
    }
}
