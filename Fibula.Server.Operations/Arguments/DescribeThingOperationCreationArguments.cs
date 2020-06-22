// -----------------------------------------------------------------
// <copyright file="DescribeThingOperationCreationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Operations.Arguments
{
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Server.Contracts.Structs;
    using Fibula.Server.Operations.Contracts.Abstractions;
    using Fibula.Server.Operations.Contracts.Enumerations;

    /// <summary>
    /// Class that represents creation arguments for a <see cref="DescribeThingOperation"/>.
    /// </summary>
    public class DescribeThingOperationCreationArguments : IOperationCreationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DescribeThingOperationCreationArguments"/> class.
        /// </summary>
        /// <param name="thingId">The id of the thing to describe.</param>
        /// <param name="location">The location where the thing to describe is.</param>
        /// <param name="stackPosition">The position in the stack at the location of the thing to describe is.</param>
        /// <param name="playerToDescribeFor">The player to describe the thing for.</param>
        public DescribeThingOperationCreationArguments(ushort thingId, Location location, byte stackPosition, IPlayer playerToDescribeFor)
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
        public OperationType Type => OperationType.DescribeThing;

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
