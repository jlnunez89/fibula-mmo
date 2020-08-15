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
        /// Gets the item that is expiring.
        /// </summary>
        public IItem Item { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        /// <param name="context">A reference to the operation context.</param>
        protected override void Execute(IElevatedOperationContext context)
        {
            var inThingContainer = this.Item.ParentContainer;

            if (!(this.Item is IThing existingThing) || !this.Item.HasExpiration)
            {
                // Silent fail.
                return;
            }

            var creationArguments = ItemCreationArguments.WithTypeId(this.Item.ExpirationTarget);

            if (this.Item.IsLiquidPool)
            {
                creationArguments.Attributes = new[]
                {
                    (ItemAttribute.LiquidType, this.Item.LiquidType as IConvertible),
                };
            }

            IThing thingCreated = context.ItemFactory.Create(creationArguments);

            if (thingCreated == null)
            {
                return;
            }

            // At this point, we have an item to change, and we were able to generate the new one, let's proceed.
            (bool replaceSuccessful, IThing replaceRemainder) = inThingContainer.ReplaceContent(context.ItemFactory, existingThing, thingCreated, byte.MaxValue, this.Item.Amount);

            if (replaceSuccessful)
            {
                if (inThingContainer is ITile atTile)
                {
                    this.SendNotification(
                        context,
                        new TileUpdatedNotification(
                            () => context.Map.PlayersThatCanSee(atTile.Location),
                            atTile.Location,
                            context.MapDescriptor.DescribeTile));

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
                            new DeleteItemOperation(requestorId: 0, thingCreated.TypeId, thingCreated.Location)
                            :
                            new ExpireItemOperation(requestorId: 0, itemCreated) as IOperation;

                        context.Scheduler.ScheduleEvent(expirationOp, itemCreated.ExpirationTimeLeft);
                    }
                }
            }
        }
    }
}
