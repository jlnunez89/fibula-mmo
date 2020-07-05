// -----------------------------------------------------------------
// <copyright file="DeathOperation.cs" company="2Dudes">
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
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Utilities;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Items;
    using Fibula.Items.Contracts.Constants;
    using Fibula.Items.Contracts.Enumerations;
    using Fibula.Map.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Enumerations;
    using Fibula.Notifications;
    using Fibula.Notifications.Arguments;

    /// <summary>
    /// Class that represents an operation for a creature's death.
    /// </summary>
    public class DeathOperation : BaseEnvironmentOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeathOperation"/> class.
        /// </summary>
        /// <param name="requestorId">The id of the creature requesting the operation.</param>
        /// <param name="creature">The creature that died.</param>
        public DeathOperation(uint requestorId, ICreature creature)
            : base(requestorId)
        {
            this.Creature = creature;
        }

        /// <summary>
        /// Gets the type of exhaustion that this operation produces.
        /// </summary>
        public override ExhaustionType ExhaustionType => ExhaustionType.None;

        /// <summary>
        /// Gets or sets the exhaustion cost time of this operation.
        /// </summary>
        public override TimeSpan ExhaustionCost { get; protected set; }

        /// <summary>
        /// Gets the creature that died.
        /// </summary>
        public ICreature Creature { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        /// <param name="context">A reference to the operation context.</param>
        protected override void Execute(IElevatedOperationContext context)
        {
            if (this.Creature is IPlayer player)
            {
                new TextMessageNotification(
                    () => player.YieldSingleItem(),
                    new TextMessageNotificationArguments(MessageType.EventAdvance, "You are dead."))
                .Send(new NotificationContext(context.Logger, context.MapDescriptor, context.CreatureFinder, context.Scheduler));
            }

            // Remove the creature...
            if (context.Map.GetTileAt(this.Creature.Location) is ITile creatureTile)
            {
                // Add the corpse.
                var corpseCreationArguments = new ItemCreationArguments()
                {
                    TypeId = this.Creature.Corpse,
                };

                if (context.ItemFactory.Create(corpseCreationArguments) is IThing corpseCreated && this.AddContentToContainerOrFallback(context, creatureTile, ref corpseCreated))
                {
                    context.GameApi.CreateItemAtLocation(
                        creatureTile.Location,
                        ItemConstants.BloodPoolTypeId,
                        new KeyValuePair<ItemAttribute, IConvertible>(ItemAttribute.LiquidType, LiquidType.Blood));
                }

                this.RemoveCreature(context, this.Creature);
            }
        }
    }
}
