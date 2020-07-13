// -----------------------------------------------------------------
// <copyright file="DeleteItemOperation.cs" company="2Dudes">
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
    using Fibula.Items.Contracts.Abstractions;
    using Fibula.Map.Contracts.Abstractions;
    using Fibula.Map.Contracts.Extensions;
    using Fibula.Mechanics.Contracts.Abstractions;
    using Fibula.Mechanics.Contracts.Enumerations;
    using Fibula.Mechanics.Contracts.Extensions;
    using Fibula.Mechanics.Notifications;

    /// <summary>
    /// Class that represents an event for an item deletion.
    /// </summary>
    public class DeleteItemOperation : BaseEnvironmentOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteItemOperation"/> class.
        /// </summary>
        /// <param name="requestorId">The id of the creature requesting the deletion.</param>
        /// <param name="typeId">The type id of the item being deleted.</param>
        /// <param name="atLocation">The location from which the item is being deleted.</param>
        public DeleteItemOperation(
            uint requestorId,
            ushort typeId,
            Location atLocation)
            : base(requestorId)
        {
            this.AtLocation = atLocation;
            this.TypeId = typeId;
        }

        /// <summary>
        /// Gets the type of exhaustion that this operation produces.
        /// </summary>
        public override ExhaustionType ExhaustionType => ExhaustionType.MentalCombat;

        /// <summary>
        /// Gets the location at which to delete the item.
        /// </summary>
        public Location AtLocation { get; }

        /// <summary>
        /// Gets the type id of the item to be deleted.
        /// </summary>
        public ushort TypeId { get; }

        /// <summary>
        /// Executes the operation's logic.
        /// </summary>
        /// <param name="context">A reference to the operation context.</param>
        protected override void Execute(IElevatedOperationContext context)
        {
            var requestor = this.GetRequestor(context.CreatureFinder);
            var inThingContainer = this.AtLocation.DecodeContainer(context.Map, context.ContainerManager, out byte index, requestor);

            // Adjust index if this a map location.
            var existingThing = (this.AtLocation.Type == LocationType.Map && (inThingContainer is ITile fromTile)) ? fromTile.FindItemWithId(this.TypeId) : inThingContainer?.FindThingAtIndex(index);

            if (inThingContainer == null || !(existingThing is IItem existingItem))
            {
                return;
            }

            // At this point, we have an item to remove, let's proceed.
            (bool removeSuccessful, IThing remainder) = inThingContainer.RemoveContent(context.ItemFactory, ref existingThing, index, amount: existingItem.Amount);

            if (!removeSuccessful)
            {
                // Failing to remove the item from the original cylinder stops the entire operation.
                return;
            }

            if (inThingContainer is ITile atTile)
            {
                new TileUpdatedNotification(
                    () => context.Map.PlayersThatCanSee(atTile.Location),
                    atTile.Location,
                    context.MapDescriptor.DescribeTile)
                .Send(new NotificationContext(context.Logger, context.MapDescriptor, context.CreatureFinder));
            }

            // Evaluate if the new item triggers a separation.
            // context.EventRulesApi.EvaluateRules(this, EventRuleType.Separation, new SeparationEventRuleArguments(atCylinder.Location, item, this.GetRequestor(context.CreatureFinder)));
        }
    }
}
