// -----------------------------------------------------------------
// <copyright file="PlaceCreatureOperationCreationArguments.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;

    public class PlaceCreatureOperationCreationArguments : IOperationCreationArguments
    {
        public PlaceCreatureOperationCreationArguments(uint requestorId, Location atLocation, ICreature creature)
        {
            creature.ThrowIfNull(nameof(creature));

            this.RequestorId = requestorId;
            this.AtLocation = atLocation;
            this.Creature = creature;
        }

        public Location AtLocation { get; }

        public ICreature Creature { get; }

        public uint RequestorId { get; }
    }
}
