﻿// -----------------------------------------------------------------
// <copyright file="LogInOperationCreationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
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
        /// <param name="client"></param>
        /// <param name="metadata"></param>
        /// <param name="currentWorldLightLevel"></param>
        /// <param name="currentWorldLightColor"></param>
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

        public IClient Client { get; }

        public ICreatureCreationMetadata CreationMetadata { get; }

        public byte WorldLightLevel { get; }

        public byte WorldLightColor { get; }
    }
}