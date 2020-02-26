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
            byte index = 0, subIndex = 0xFF;

            this.TypeId = typeId;
            this.AtCylinder = atLocation.GetCyclinder(this.Context.TileAccessor, this.Context.ContainerManager, ref index, ref subIndex, this.Requestor);
            this.AtIndex = subIndex;
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
        /// Gets the type id of the item to create.
        /// </summary>
        public ushort TypeId { get; }

        /// <summary>
        /// Gets the cylinder at which to create the item.
        /// </summary>
        public ICylinder AtCylinder { get; }

        /// <summary>
        /// Gets the index at which to create the item in the cylinder.
        /// </summary>
        public byte AtIndex { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        public override void Execute()
        {
            if (this.AtCylinder == null || !(this.Context.ItemFactory.Create(this.TypeId) is IThing thingCreated))
            {
                return;
            }

            // At this point, we were able to generate the new one, let's proceed to add it.
            this.AddContentToCylinderChain(this.AtCylinder.GetCylinderHierarchy(), this.AtIndex, ref thingCreated, this.Requestor);
        }
    }
}
