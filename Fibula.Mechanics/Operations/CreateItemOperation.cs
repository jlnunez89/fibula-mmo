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

namespace Fibula.Mechanics.Operations
{
    using System;
    using System.Collections.Generic;
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Common.Contracts.Structs;
    using Fibula.Items;
    using Fibula.Items.Contracts.Enumerations;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Enumerations;
    using Fibula.Mechanics.Contracts.Extensions;

    /// <summary>
    /// Class that represents an event for an item creation.
    /// </summary>
    public class CreateItemOperation : BaseEnvironmentOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateItemOperation"/> class.
        /// </summary>
        /// <param name="requestorId">The id of the creature requesting the use.</param>
        /// <param name="itemTypeId">The type id of the item being created.</param>
        /// <param name="atLocation">The location from which the item is being created.</param>
        /// <param name="attributes">The attributes to set in the item.</param>
        public CreateItemOperation(uint requestorId, ushort itemTypeId, Location atLocation, IDictionary<ItemAttribute, IConvertible> attributes)
            : base(requestorId)
        {
            this.TypeId = itemTypeId;
            this.WithAttributes = attributes;
            this.AtLocation = atLocation;
        }

        /// <summary>
        /// Gets the type of exhaustion that this operation produces.
        /// </summary>
        public override ExhaustionType ExhaustionType => ExhaustionType.MentalCombat;

        /// <summary>
        /// Gets or sets the exhaustion cost time of this operation.
        /// </summary>
        public override TimeSpan ExhaustionCost { get; protected set; }

        /// <summary>
        /// Gets the type id of the item to create.
        /// </summary>
        public ushort TypeId { get; }

        /// <summary>
        /// Gets or sets the attributes to set in the item to create.
        /// </summary>
        public IDictionary<ItemAttribute, IConvertible> WithAttributes { get; set; }

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
            var inThingContainer = this.AtLocation.DecodeContainer(context.Map, context.ContainerManager, out byte index, requestor);

            var creationArguments = new ItemCreationArguments()
            {
                TypeId = this.TypeId,
                Attributes = this.WithAttributes,
            };

            if (inThingContainer == null || !(context.ItemFactory.Create(creationArguments) is IThing thingCreated))
            {
                return;
            }

            // At this point, we were able to generate the new one, let's proceed to add it.
            this.AddContentToContainerOrFallback(context, inThingContainer, ref thingCreated, index, includeTileAsFallback: true, requestor);
        }
    }
}
