// <copyright file="ThingMovementContainerToSlot.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Movement
{
    using System;
    using OpenTibia.Scheduling.Contracts;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;
    using OpenTibia.Server.Movement.EventConditions;
    using OpenTibia.Server.Notifications;

    internal class ThingMovementContainerToSlot : MovementBase
    {
        public ThingMovementContainerToSlot(uint creatureRequestingId, IThing thingMoving, Location fromLocation, Location toLocation, byte count = 1)
            : base(creatureRequestingId)
        {
            // intentionally left thing null check out. Handled by Perform().
            var requestor = this.RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(this.RequestorId);

            if (count == 0)
            {
                throw new ArgumentException("Invalid count zero.");
            }

            if (requestor == null)
            {
                throw new ArgumentNullException(nameof(requestor));
            }

            this.Thing = thingMoving;
            this.Count = count;

            this.FromLocation = fromLocation;
            this.FromContainer = (requestor as IPlayer)?.GetContainer(this.FromLocation.Container);
            this.FromIndex = (byte)this.FromLocation.Z;

            this.ToLocation = toLocation;
            this.ToSlot = (byte)this.ToLocation.Slot;

            var droppingItem = requestor.Inventory?[this.ToSlot];

            if (this.FromContainer != null && this.FromContainer.HolderId != requestor.CreatureId)
            {
                this.Conditions.Add(new GrabberHasEnoughCarryStrengthEventCondition(this.RequestorId, this.Thing, droppingItem));
            }

            this.Conditions.Add(new SlotHasContainerAndContainerHasEnoughCapacityEventCondition(this.RequestorId, droppingItem));
            this.Conditions.Add(new GrabberHasContainerOpenEventCondition(this.RequestorId, this.FromContainer));
            this.Conditions.Add(new ContainerHasItemAndEnoughAmountEventCondition(this.Thing as IItem, this.FromContainer, this.FromIndex, this.Count));
        }

        public override EvaluationTime EvaluateAt => EvaluationTime.OnExecute;

        public Location FromLocation { get; }

        public IContainer FromContainer { get; }

        public byte FromIndex { get; }

        public Location ToLocation { get; }

        public byte ToSlot { get; }

        public IThing Thing { get; }

        public byte Count { get; }

        /// <inheritdoc/>
        public override void Process()
        {
            IItem addedItem;
            var updateItem = this.Thing as IItem;
            var requestor = this.RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(this.RequestorId);

            if (this.FromContainer == null || updateItem == null || requestor == null)
            {
                return;
            }

            // attempt to remove from the source container
            if (!this.FromContainer.RemoveContent(updateItem.Type.TypeId, this.FromIndex, this.Count, out addedItem))
            {
                return;
            }

            if (addedItem != null)
            {
                updateItem = addedItem;
            }

            IThing currentThing = null;
            // attempt to place the intended item at the slot.
            if (!requestor.Inventory.Add(updateItem, out addedItem, this.ToSlot, this.Count))
            {
                // Something went wrong, add back to the source container...
                if (this.FromContainer.AddContent(updateItem, 0xFF))
                {
                    return;
                }

                // and we somehow failed to re-add it to the source container...
                // throw to the ground.
                currentThing = updateItem;
            }
            else
            {
                // added the new item to the slot
                if (addedItem == null)
                {
                    return;
                }

                // we exchanged or got some leftover item, place back in the source container at any index.
                if (this.FromContainer.AddContent(addedItem, 0xFF))
                {
                    return;
                }

                // and we somehow failed to re-add it to the source container...
                // throw to the ground.
                currentThing = addedItem;
            }

            requestor.Tile.AddThing(ref currentThing, currentThing.Count);

            // notify all spectator players of that tile.
            Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, requestor.Location, Game.Instance.GetMapTileDescription(conn.PlayerId, requestor.Location)), requestor.Location);
        }
    }
}