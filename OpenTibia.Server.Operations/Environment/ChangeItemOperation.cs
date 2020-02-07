// -----------------------------------------------------------------
// <copyright file="ChangeItemOperation.cs" company="2Dudes">
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
    using OpenTibia.Server.Contracts;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Operations.Actions;
    using Serilog;

    /// <summary>
    /// Class that represents an event for an item change.
    /// </summary>
    public class ChangeItemOperation : BaseEnvironmentOperation
    {
        /// <summary>
        /// Caches the requestor creature, if defined.
        /// </summary>
        private ICreature requestor = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeItemOperation"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="context">The operation context.</param>
        /// <param name="requestorId">The id of the creature requesting the use.</param>
        /// <param name="typeId">The type id of the item being used.</param>
        /// <param name="fromLocation">The location from which the item is being used.</param>
        /// <param name="toTypeId">The type id of the item to change to.</param>
        /// <param name="fromStackPos">The position in the stack from which the item is being used.</param>
        /// <param name="index">The index of the item being used.</param>
        /// <param name="carrierCreature">The creature who is carrying the thing, if any.</param>
        public ChangeItemOperation(
            ILogger logger,
            IElevatedOperationContext context,
            uint requestorId,
            ushort typeId,
            Location fromLocation,
            ushort toTypeId,
            byte fromStackPos = byte.MaxValue,
            byte index = 1,
            ICreature carrierCreature = null)
            : base(logger, context, requestorId)
        {
            this.ActionsOnPass.Add(() =>
            {
                var fromCylinder = fromLocation.GetCyclinder(this.Context.TileAccessor, ref fromStackPos, ref index, carrierCreature);
                var item = fromLocation.FindItemById(this.Context.TileAccessor, typeId, carrierCreature);

                // TODO: shouldn't really need a requestor here.
                bool successfulChange = this.PerformItemChange(item, toTypeId, fromCylinder, index, this.Requestor);

                if (!successfulChange)
                {
                    // handles check for isPlayer.
                    // this.NotifyOfFailure();
                    return;
                }
            });
        }

        /// <summary>
        /// Gets the creature that is requesting the event, if known.
        /// </summary>
        public ICreature Requestor
        {
            get
            {
                if (this.RequestorId > 0 && this.requestor == null)
                {
                    this.requestor = this.Context.CreatureFinder.FindCreatureById(this.RequestorId);
                }

                return this.requestor;
            }
        }
    }
}
