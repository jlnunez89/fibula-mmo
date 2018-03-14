// <copyright file="ThingMovementContainerToContainer.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Movement
{
    using System;
    using System.Linq;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Scheduling.Contracts;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;
    using OpenTibia.Server.Events;
    using OpenTibia.Server.Movement.EventConditions;
    using OpenTibia.Server.Notifications;

    internal class ThingMovementContainerToContainer : MovementBase
    {
        public ThingMovementContainerToContainer(uint creatureRequestingId, IThing thingMoving, Location fromLocation, Location toLocation, byte count = 1)
            : base(creatureRequestingId)
        {
            // intentionally left thing null check out. Handled by Perform().
            var requestor = Game.Instance.GetCreatureWithId(this.RequestorId);

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
            this.ToContainer = (requestor as IPlayer)?.GetContainer(this.ToLocation.Container);
            this.ToIndex = (byte)this.ToLocation.Z;

            if (this.FromContainer?.HolderId != this.ToContainer?.HolderId && this.ToContainer?.HolderId == this.RequestorId)
            {
                this.Conditions.Add(new GrabberHasEnoughCarryStrengthEventCondition(this.RequestorId, this.Thing));
            }

            this.Conditions.Add(new GrabberHasContainerOpenEventCondition(this.RequestorId, this.FromContainer));
            this.Conditions.Add(new ContainerHasItemAndEnoughAmountEventCondition(this.Thing as IItem, this.FromContainer, this.FromIndex, this.Count));
            this.Conditions.Add(new GrabberHasContainerOpenEventCondition(this.RequestorId, this.ToContainer));
        }

        public override EvaluationTime EvaluateAt => EvaluationTime.OnExecute;

        public Location FromLocation { get; }

        public IContainer FromContainer { get; }

        public byte FromIndex { get; }

        public Location ToLocation { get; }

        public IContainer ToContainer { get; }

        public byte ToIndex { get; }

        public IThing Thing { get; }

        public byte Count { get; }

        /// <inheritdoc/>
        public override void Process()
        {
            IItem extraItem;
            var updatedItem = this.Thing as IItem;
            var requestor = this.RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(this.RequestorId);

            if (this.FromContainer == null || this.ToContainer == null || updatedItem == null || requestor == null)
            {
                return;
            }

            // attempt to remove from the source container
            if (!this.FromContainer.RemoveContent(updatedItem.Type.TypeId, this.FromIndex, this.Count, out extraItem))
            {
                return;
            }

            if (extraItem != null)
            {
                updatedItem = extraItem;
            }

            // successfully removed thing from the source container
            // attempt to add to the dest container
            if (this.ToContainer.AddContent(updatedItem, this.ToIndex))
            {
                return;
            }

            // failed to add to the dest container (whole or partial)
            // attempt to add again to the source at any index.
            if (this.FromContainer.AddContent(updatedItem, 0xFF))
            {
                return;
            }

            // and we somehow failed to re-add it to the source container...
            // throw to the ground.
            IThing thing = updatedItem;
            requestor.Tile.AddThing(ref thing, updatedItem.Count);

            // notify all spectator players of that tile.
            Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, requestor.Location, Game.Instance.GetMapTileDescription(conn.PlayerId, requestor.Location)), requestor.Location);

            // and handle collision.
            if (!requestor.Tile.HandlesCollision)
            {
                return;
            }

            foreach (var itemWithCollision in requestor.Tile.ItemsWithCollision)
            {
                var collisionEvents = Game.Instance.EventsCatalog[ItemEventType.Collision].Cast<CollisionItemEvent>();

                var candidate = collisionEvents.FirstOrDefault(e => e.ThingIdOfCollision == itemWithCollision.Type.TypeId && e.Setup(itemWithCollision, thing) && e.CanBeExecuted);

                // Execute all actions.
                candidate?.Execute();
            }
        }
    }
}