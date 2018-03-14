// <copyright file="ThingMovementSlotToContainer.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Movement
{
    using System;
    using System.Linq;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;
    using OpenTibia.Server.Events;
    using OpenTibia.Server.Movement.Policies;
    using OpenTibia.Server.Notifications;

    internal class ThingMovementSlotToContainer : MovementBase
    {
        public Location FromLocation { get; }

        public byte FromSlot { get; }

        public Location ToLocation { get; }

        public IContainer ToContainer { get; }

        public byte ToIndex { get; }

        public IItem Item { get; }

        public byte Count { get; }

        public ThingMovementSlotToContainer(uint creatureRequestingId, IThing thingMoving, Location fromLocation, Location toLocation, byte count = 1)
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

            this.FromLocation = fromLocation;
            this.FromSlot = (byte)this.FromLocation.Slot;

            this.ToLocation = toLocation;
            this.ToContainer = (requestor as IPlayer)?.GetContainer(this.ToLocation.Container);
            this.ToIndex = (byte)this.ToLocation.Z;

            this.Item = thingMoving as IItem;
            this.Count = count;

            this.Policies.Add(new SlotContainsItemAndCountPolicy(creatureRequestingId, this.Item, this.FromSlot, this.Count));
            this.Policies.Add(new GrabberHasContainerOpenPolicy(this.RequestorId, this.ToContainer));
            this.Policies.Add(new ContainerHasEnoughCapacityPolicy(this.ToContainer));
        }

        public override void Perform()
        {
            var requestor = this.RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(this.RequestorId);

            if (this.Item == null || requestor == null)
            {
                return;
            }

            bool partialRemove;
            // attempt to remove the item from the inventory
            var movingItem = requestor.Inventory?.Remove(this.FromSlot, this.Count, out partialRemove);

            if (movingItem == null)
            {
                return;
            }

            // successfully removed thing from the source container
            // attempt to add to the dest container
            if (this.ToContainer.AddContent(movingItem, this.ToIndex))
            {
                return;
            }

            // failed to add to the slot, add again to the source slot
            if (!requestor.Inventory.Add(movingItem, out movingItem, this.FromSlot, movingItem.Count))
            {
                // and we somehow failed to re-add it to the source container...
                // throw to the ground.
                IThing thing = movingItem;
                requestor.Tile.AddThing(ref thing, movingItem.Count);

                // notify all spectator players of that tile.
                Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, requestor.Location, Game.Instance.GetMapTileDescription(conn.PlayerId, requestor.Location)), requestor.Location);

                // call any collision events again.
                if (requestor.Tile.HandlesCollision)
                {
                    foreach (var itemWithCollision in requestor.Tile.ItemsWithCollision)
                    {
                        var collisionEvents = Game.Instance.EventsCatalog[ItemEventType.Collision].Cast<CollisionItemEvent>();

                        var candidate =
                            collisionEvents.FirstOrDefault(
                                e => e.ThingIdOfCollision == itemWithCollision.Type.TypeId &&
                                     e.Setup(itemWithCollision, thing) && e.CanBeExecuted);

                        // Execute all actions.
                        candidate?.Execute();
                    }
                }
            }
        }
    }
}