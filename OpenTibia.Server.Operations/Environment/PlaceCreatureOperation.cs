// -----------------------------------------------------------------
// <copyright file="PlaceCreatureOperation.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Operations.Actions;
    using Serilog;

    /// <summary>
    /// Class that represents an operation for placing a creature on the map.
    /// </summary>
    public class PlaceCreatureOperation : BaseEnvironmentOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlaceCreatureOperation"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="context">The context of the operation.</param>
        /// <param name="requestorId">The id of the creature requesting the use.</param>
        /// <param name="atLocation">The location from which the item is being created.</param>
        /// <param name="creature">The creature being placed.</param>
        public PlaceCreatureOperation(
            ILogger logger,
            IElevatedOperationContext context,
            uint requestorId,
            Location atLocation,
            ICreature creature)
            : base(logger, context, requestorId)
        {
            this.ActionsOnPass.Add(() =>
            {
                bool successfulPlacement = this.PlaceCreature(atLocation, creature);

                if (!successfulPlacement)
                {
                    // handles check for isPlayer.
                    // this.NotifyOfFailure();
                    return;
                }
            });
        }
    }
}
