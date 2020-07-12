// -----------------------------------------------------------------
// <copyright file="ChangeItemOperation.cs" company="2Dudes">
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
    using Fibula.Common.Contracts.Abstractions;
    using Fibula.Common.Contracts.Enumerations;
    using Fibula.Common.Contracts.Structs;
    using Fibula.Creatures.Contracts.Abstractions;
    using Fibula.Items;
    using Fibula.Items.Contracts.Abstractions;
    using Fibula.Map.Contracts.Abstractions;
    using Fibula.Map.Contracts.Extensions;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Enumerations;
    using Fibula.Mechanics.Contracts.Extensions;
    using Fibula.Notifications;
    using Fibula.Notifications.Arguments;

    /// <summary>
    /// Class that represents an event for an item change.
    /// </summary>
    public class ChangeItemOperation : BaseEnvironmentOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeItemOperation"/> class.
        /// </summary>
        /// <param name="requestorId">The id of the creature requesting the change.</param>
        /// <param name="typeId">The type id of the item being changed.</param>
        /// <param name="fromLocation">The location from which the item is being changed.</param>
        /// <param name="toTypeId">The type id of the item to change to.</param>
        /// <param name="carrierCreature">The creature who is carrying the thing, if any.</param>
        public ChangeItemOperation(
            uint requestorId,
            ushort typeId,
            Location fromLocation,
            ushort toTypeId,
            ICreature carrierCreature = null)
            : base(requestorId)
        {
            this.FromLocation = fromLocation;
            this.FromTypeId = typeId;
            this.FromCreature = carrierCreature;
            this.ToTypeId = toTypeId;
        }

        /// <summary>
        /// Gets the type of exhaustion that this operation produces.
        /// </summary>
        public override ExhaustionType ExhaustionType => ExhaustionType.MentalCombat;

        /// <summary>
        /// Gets the location from which the item is being changed.
        /// </summary>
        public Location FromLocation { get; }

        /// <summary>
        /// Gets the type id from which the item is being changed.
        /// </summary>
        public ushort FromTypeId { get; }

        /// <summary>
        /// Gets the creature from which the item is being changed, if any.
        /// </summary>
        public ICreature FromCreature { get; }

        /// <summary>
        /// Gets the type id of the item to change to.
        /// </summary>
        public ushort ToTypeId { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        /// <param name="context">A reference to the operation context.</param>
        protected override void Execute(IElevatedOperationContext context)
        {
            const byte FallbackIndex = 0xFF;

            var inThingContainer = this.FromLocation.DecodeContainer(context.Map, context.ContainerManager, out byte index, this.FromCreature);

            // Adjust index if this a map location.
            var existingThing = (this.FromLocation.Type == LocationType.Map && (inThingContainer is ITile fromTile)) ? fromTile.FindItemWithId(this.FromTypeId) : inThingContainer?.FindThingAtIndex(index);

            if (existingThing == null || !(existingThing is IItem existingItem))
            {
                // Silent fail.
                return;
            }

            var creationArguments = new ItemCreationArguments() { TypeId = this.ToTypeId };

            IThing thingCreated = context.ItemFactory.Create(creationArguments);

            if (thingCreated == null)
            {
                return;
            }

            // At this point, we have an item to change, and we were able to generate the new one, let's proceed.
            (bool replaceSuccessful, IThing replaceRemainder) = inThingContainer.ReplaceContent(context.ItemFactory, existingThing, thingCreated, index, existingItem.Amount);

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
                        new TileUpdatedNotificationArguments(atTile.Location, context.MapDescriptor.DescribeTile))
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
                            new DeleteItemOperation(requestorId: 0, thingCreated.ThingId, thingCreated.Location) as IOperation
                            :
                            new ChangeItemOperation(requestorId: 0, thingCreated.ThingId, thingCreated.Location, itemCreated.ExpirationTarget) as IOperation;

                        context.Scheduler.ScheduleEvent(expirationOp, itemCreated.ExpirationTimeLeft);
                    }
                }
            }
        }
    }
}
