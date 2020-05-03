// -----------------------------------------------------------------
// <copyright file="ThinkingOperationCreationArguments.cs" company="2Dudes">
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
    using System;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;

    /// <summary>
    /// Class that represents creation arguments for an <see cref="ThinkingOperation"/>.
    /// </summary>
    public class ThinkingOperationCreationArguments : IOperationCreationArguments
    {
        /// <summary>
        /// The default thinking cadence for thinking operations.
        /// </summary>
        private static readonly TimeSpan DefaultThinkingCadence = TimeSpan.FromSeconds(1);

        /// <summary>
        /// Initializes a new instance of the <see cref="ThinkingOperationCreationArguments"/> class.
        /// </summary>
        /// <param name="requestorId"></param>
        /// <param name="creature"></param>
        /// <param name="target"></param>
        /// <param name="thinkingCadence"></param>
        public ThinkingOperationCreationArguments(uint requestorId, ICreature creature, TimeSpan? thinkingCadence = null)
        {
            creature.ThrowIfNull(nameof(creature));

            this.RequestorId = requestorId;
            this.Creature = creature;
            this.Cadence = thinkingCadence ?? DefaultThinkingCadence;
        }

        public uint RequestorId { get; }

        public ICreature Creature { get; }

        public TimeSpan Cadence { get; }
    }
}
