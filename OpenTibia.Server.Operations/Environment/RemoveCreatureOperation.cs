// -----------------------------------------------------------------
// <copyright file="RemoveCreatureOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Operations.Environment
{
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Operations.Actions;
    using Serilog;

    /// <summary>
    /// Class that represents an operation for removing a creature from the map.
    /// </summary>
    public class RemoveCreatureOperation : BaseEnvironmentOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveCreatureOperation"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="context">the context of the operation.</param>
        /// <param name="requestorId">The id of the creature requesting the use.</param>
        /// <param name="creature">The creature being placed.</param>
        public RemoveCreatureOperation(
            ILogger logger,
            IElevatedOperationContext context,
            uint requestorId,
            ICreature creature)
            : base(logger, context, requestorId)
        {
            this.Creature = creature;
        }

        /// <summary>
        /// Gets the creature to remove.
        /// </summary>
        public ICreature Creature { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        public override void Execute()
        {
            bool successfulRemoval = this.RemoveCreature(this.Creature);

            if (!successfulRemoval)
            {
                // handles check for isPlayer.
                // this.NotifyOfFailure();
                return;
            }
        }
    }
}
