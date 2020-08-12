// -----------------------------------------------------------------
// <copyright file="RemoveCreatureOperation.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// jlnunez89@gmail.com
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE.txt file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Server.Operations.Environment
{
    using Fibula.Server.Contracts.Abstractions;
    using Fibula.Server.Operations.Contracts.Abstractions;

    /// <summary>
    /// Class that represents an operation for removing a creature from the map.
    /// </summary>
    public class RemoveCreatureOperation : BaseEnvironmentOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveCreatureOperation"/> class.
        /// </summary>
        /// <param name="requestorId">The id of the creature requesting the removal.</param>
        /// <param name="creature">The creature being removed.</param>
        public RemoveCreatureOperation(uint requestorId, ICreature creature)
            : base(requestorId)
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
        /// <param name="context">A reference to the operation context.</param>
        protected override void Execute(IElevatedOperationContext context)
        {
            bool successfulRemoval = this.RemoveCreature(context, this.Creature);

            if (!successfulRemoval)
            {
                // handles check for isPlayer.
                // this.NotifyOfFailure();
                return;
            }
        }
    }
}
