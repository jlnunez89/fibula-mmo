// -----------------------------------------------------------------
// <copyright file="LogInOperationCreationArguments.cs" company="2Dudes">
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
    using Fibula.Client.Contracts.Abstractions;
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Enumerations;

    /// <summary>
    /// Class that represents creation arguments for a <see cref="LogInOperation"/>.
    /// </summary>
    public class LogInOperationCreationArguments : IOperationCreationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogInOperationCreationArguments"/> class.
        /// </summary>
        /// <param name="client">The client calling the log in operation.</param>
        /// <param name="metadata">The creation metadata for the player instance being created.</param>
        /// <param name="currentWorldLightLevel">The initial world light level to send to the player on log in.</param>
        /// <param name="currentWorldLightColor">The initial world light color to send to the player on log in.</param>
        public LogInOperationCreationArguments(IClient client, ICreatureCreationMetadata metadata, byte currentWorldLightLevel, byte currentWorldLightColor)
        {
            client.ThrowIfNull(nameof(client));
            metadata.ThrowIfNull(nameof(metadata));

            this.Client = client;
            this.CreationMetadata = metadata;
            this.WorldLightLevel = currentWorldLightLevel;
            this.WorldLightColor = currentWorldLightColor;
        }

        /// <summary>
        /// Gets the type of operation being created.
        /// </summary>
        public OperationType Type => OperationType.LogIn;

        /// <summary>
        /// Gets the id of the requestor of the operation.
        /// </summary>
        public uint RequestorId { get; }

        /// <summary>
        /// Gets the client requesting the log in.
        /// </summary>
        public IClient Client { get; }

        /// <summary>
        /// Gets the creation metadata for the creature instance to create.
        /// </summary>
        public ICreatureCreationMetadata CreationMetadata { get; }

        /// <summary>
        /// Gets the initial world light level to send to the player logging in.
        /// </summary>
        public byte WorldLightLevel { get; }

        /// <summary>
        /// Gets the initial world light color to send to the player logging in.
        /// </summary>
        public byte WorldLightColor { get; }
    }
}
