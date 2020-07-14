// -----------------------------------------------------------------
// <copyright file="ExpireItemOperation.cs" company="2Dudes">
// Copyright (c) | Jose L. Nunez de Caceres et al.
// https://linkedin.com/in/nunezdecaceres
//
// All Rights Reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// -----------------------------------------------------------------

namespace Fibula.Mechanics.Operations
{
    using System;
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Items;
    using Fibula.Items.Contracts.Abstractions;
    using Fibula.Items.Contracts.Enumerations;
    using Fibula.Map.Contracts.Abstractions;
    using Fibula.Map.Contracts.Extensions;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Enumerations;
    using Fibula.Mechanics.Notifications;

    /// <summary>
    /// Class that represents an event for an item expiring.
    /// </summary>
    public class ExpireItemOperation : BaseEnvironmentOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpireItemOperation"/> class.
        /// </summary>
        /// <param name="requestorId">The id of the creature requesting the change.</param>
        /// <param name="item">The item that's expiring.</param>
        public ExpireItemOperation(uint requestorId, IItem item)
            : base(requestorId)
        {
            this.Item = item;
        }

        /// <summary>
        /// Gets the type of exhaustion that this operation produces.
        /// </summary>
        public override ExhaustionType ExhaustionType => ExhaustionType.MentalCombat;

        /// <summary>
        /// Gets the item that is expiring.
        /// </summary>
        public IItem Item { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        /// <param name="context">A reference to the operation context.</param>
        protected override void Execute(IElevatedOperationContext context)
        {
            const byte FallbackIndex = 0xFF;

            var inThingContainer = this.Item.ParentContainer;

            if (!(this.Item is IThing existingThing) || !(existingThing is IItem existingItem) || !existingItem.HasExpiration)
            {
                // Silent fail.
                return;
            }

            var creationArguments = new ItemCreationArguments() { TypeId = existingItem.ExpirationTarget };

            if (existingItem.IsLiquidPool)
            {
                creationArguments.Attributes = new[]
                {
                    (ItemAttribute.LiquidType, existingItem.LiquidType as IConvertible),
                };
            }

            IThing thingCreated = context.ItemFactory.Create(creationArguments);

            if (thingCreated == null)
            {
                return;
            }

            // At this point, we have an item to change, and we were able to generate the new one, let's proceed.
            (bool replaceSuccessful, IThing replaceRemainder) = inThingContainer.ReplaceContent(context.ItemFactory, existingThing, thingCreated, byte.MaxValue, existingItem.Amount);

            if (!replaceSuccessful || replaceRemainder != null)
            {
                this.AddContentToContainerOrFallback(context, inThingContainer, ref replaceRemainder, FallbackIndex, includeTileAsFallback: true, this.GetRequestor(context.CreatureFinder));
            }

            if (replaceSuccessful)
            {
                if (inThingContainer is ITile atTile)
                {
                    new TileUpdatedNotification(
                        () => context.Map.PlayersThatCanSee(atTile.Location),
                        atTile.Location,
                        context.MapDescriptor.DescribeTile)
                    .Send(new NotificationContext(context.Logger, context.MapDescriptor, context.CreatureFinder));

                    // Evaluate if the new item triggers a collision.
                    // context.EventRulesApi.EvaluateRules(this, EventRuleType.Collision, new CollisionEventRuleArguments(fromCylinder.Location, existingThing, this.GetRequestor(context.CreatureFinder)));
                }

                if (thingCreated is IItem itemCreated)
                {
                    // Start decay for items that need it.
                    if (itemCreated.HasExpiration)
                    {
                        // TODO: the item location will change and this will break.
                        var expirationOp = itemCreated.ExpirationTarget == 0 ?
                            new DeleteItemOperation(requestorId: 0, thingCreated.ThingId, thingCreated.Location)
                            :
                            new ExpireItemOperation(requestorId: 0, itemCreated) as IOperation;

                        context.Scheduler.ScheduleEvent(expirationOp, itemCreated.ExpirationTimeLeft);
                    }
                }
            }
        }
    }
}
