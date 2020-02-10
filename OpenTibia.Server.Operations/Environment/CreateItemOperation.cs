// -----------------------------------------------------------------
// <copyright file="CreateItemOperation.cs" company="2Dudes">
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
    /// Class that represents an event for an item creation.
    /// </summary>
    public class CreateItemOperation : BaseEnvironmentOperation
    {
        /// <summary>
        /// Caches the requestor creature, if defined.
        /// </summary>
        private ICreature requestor = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateItemOperation"/> class.
        /// </summary>
        /// <param name="logger">A reference to the logger in use.</param>
        /// <param name="context">The operation's context.</param>
        /// <param name="requestorId">The id of the creature requesting the use.</param>
        /// <param name="typeId">The type id of the item being created.</param>
        /// <param name="atLocation">The location from which the item is being created.</param>
        public CreateItemOperation(
            ILogger logger,
            IElevatedOperationContext context,
            uint requestorId,
            ushort typeId,
            Location atLocation)
            : base(logger, context, requestorId)
        {
            this.ActionsOnPass.Add(() =>
            {
                byte index = 0, subIndex = 0;

                var fromCylinder = atLocation.GetCyclinder(this.Context.TileAccessor, this.Context.ContainerManager, ref index, ref subIndex, this.Requestor);

                // TODO: shouldn't really need a requestor here.
                bool successfulCreation = this.PerformItemCreation(typeId, fromCylinder, subIndex, this.Requestor);

                if (!successfulCreation)
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

        /// <summary>
        /// Immediately attempts to perform an item creation in behalf of the requesting creature, if any.
        /// </summary>
        /// <param name="typeId">The id of the item being being created.</param>
        /// <param name="atCylinder">The cylinder from which the use is happening.</param>
        /// <param name="index">Optional. The index within the cylinder from which to use the item.</param>
        /// <param name="requestor">Optional. The creature requesting the creation.</param>
        /// <returns>True if the item was successfully created, false otherwise.</returns>
        /// <remarks>Changes game state, should only be performed after all pertinent validations happen.</remarks>>
        private bool PerformItemCreation(ushort typeId, ICylinder atCylinder, byte index = 0xFF, ICreature requestor = null)
        {
            if (atCylinder == null)
            {
                return false;
            }

            IThing newItem = this.Context.ItemFactory.Create(typeId);

            if (newItem == null)
            {
                return false;
            }

            // At this point, we were able to generate the new one, let's proceed to add it.
            return this.AddContentToCylinderChain(atCylinder.GetCylinderHierarchy(), index, ref newItem, requestor);
        }
    }
}
