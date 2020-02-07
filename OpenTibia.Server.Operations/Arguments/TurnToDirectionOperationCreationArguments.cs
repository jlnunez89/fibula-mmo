// -----------------------------------------------------------------
// <copyright file="TurnToDirectionOperationCreationArguments.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts.Enumerations;

    public class TurnToDirectionOperationCreationArguments : IOperationCreationArguments
    {
        public TurnToDirectionOperationCreationArguments(ICreature creature, Direction direction)
        {
            creature.ThrowIfNull(nameof(creature));

            this.RequestorId = creature.Id;
            this.Creature = creature;
            this.Direction = direction;
        }

        public uint RequestorId { get; }

        public ICreature Creature { get; }

        public Direction Direction { get; }
    }
}
