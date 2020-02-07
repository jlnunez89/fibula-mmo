// -----------------------------------------------------------------
// <copyright file="LogInOperationCreationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Operations.Arguments
{
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Abstractions;

    public class LogInOperationCreationArguments : IOperationCreationArguments
    {
        public LogInOperationCreationArguments(ICreatureCreationMetadata metadata, IConnection connection, byte currentWorldLightLevel, byte currentWorldLightColor)
        {
            metadata.ThrowIfNull(nameof(metadata));
            connection.ThrowIfNull(nameof(connection));

            this.CreationMetadata = metadata;
            this.Connection = connection;
            this.WorldLightLevel = currentWorldLightLevel;
            this.WorldLightColor = currentWorldLightColor;
        }

        public ICreatureCreationMetadata CreationMetadata { get; }

        public IConnection Connection { get; }

        public uint RequestorId { get; }

        public byte WorldLightLevel { get; }

        public byte WorldLightColor { get; }
    }
}
