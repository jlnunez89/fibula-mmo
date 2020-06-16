// -----------------------------------------------------------------
// <copyright file="CreateItemOperation.cs" company="2Dudes">
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
    /// <summary>
    /// Class that represents an event for an item creation.
    /// </summary>
    public class CreateItemOperation : BaseEnvironmentOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateItemOperation"/> class.
        /// </summary>
        /// <param name="requestorId">The id of the creature requesting the use.</param>
        /// <param name="typeId">The type id of the item being created.</param>
        /// <param name="atLocation">The location from which the item is being created.</param>
        public CreateItemOperation(uint requestorId, ushort typeId, Location atLocation)
            : base(requestorId)
        {
            this.TypeId = typeId;
            this.AtLocation = atLocation;
        }

        /// <summary>
        /// Gets the type id of the item to create.
        /// </summary>
        public ushort TypeId { get; }

        /// <summary>
        /// Gets the location at which to create the item.
        /// </summary>
        public Location AtLocation { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        /// <param name="context">A reference to the operation context.</param>
        protected override void Execute(IElevatedOperationContext context)
        {
            var requestor = this.GetRequestor(context.CreatureFinder);
            var atCylinder = this.AtLocation.DecodeCyclinder(context.TileAccessor, context.ContainerManager, out byte index, requestor);

            if (atCylinder == null || !(context.ItemFactory.Create(this.TypeId) is IThing thingCreated))
            {
                return;
            }

            // At this point, we were able to generate the new one, let's proceed to add it.
            this.AddContentToCylinderOrFallback(context, atCylinder, index, ref thingCreated, includeTileAsFallback: true, requestor);
        }
    }
}
