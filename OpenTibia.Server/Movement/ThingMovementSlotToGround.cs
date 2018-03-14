// <copyright file="ThingMovementSlotToGround.cs" company="2Dudes">
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

    internal class ThingMovementSlotToGround : MovementBase
    {
        public ThingMovementSlotToGround(uint creatureRequestingId, IThing thingMoving, Location fromLocation, Location toLocation, byte count = 1)
            : base(creatureRequestingId)
        {
            // intentionally left thing null check out. Handled by Perform().
            var requestor = this.RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(this.RequestorId);

            if (count == 0)
            {
                throw new ArgumentException("Invalid count zero.");
            }

            this.FromLocation = fromLocation;
            this.FromSlot = (byte)fromLocation.Slot;

            this.ToLocation = toLocation;
            this.ToTile = Game.Instance.GetTileAt(this.ToLocation);

            this.Item = thingMoving as IItem;
            this.Count = count;

            if (requestor != null)
            {
                this.Conditions.Add(new CanThrowBetweenEventCondition(this.RequestorId, requestor.Location, this.ToLocation));
            }

            this.Conditions.Add(new SlotContainsItemAndCountEventCondition(creatureRequestingId, this.Item, this.FromSlot, this.Count));
            this.Conditions.Add(new LocationNotObstructedEventCondition(this.RequestorId, this.Item, this.ToLocation));
            this.Conditions.Add(new LocationHasTileWithGroundEventCondition(this.ToLocation));
        }

        public override EvaluationTime EvaluateAt => EvaluationTime.OnExecute;

        public Location FromLocation { get; }

        public byte FromSlot { get; }

        public Location ToLocation { get; }

        public ITile ToTile { get; }

        public IItem Item { get; }

        public byte Count { get; }

        public override void Process()
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

            // add the remaining item to the destination tile.
            IThing thing = movingItem;
            this.ToTile.AddThing(ref thing, movingItem.Count);

            // notify all spectator players of that tile.
            Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, this.ToTile.Location, Game.Instance.GetMapTileDescription(conn.PlayerId, this.ToTile.Location)), this.ToTile.Location);

            // and handle collision.
            if (!this.ToTile.HandlesCollision)
            {
                return;
            }

            foreach (var itemWithCollision in this.ToTile.ItemsWithCollision)
            {
                var collisionEvents = Game.Instance.EventsCatalog[ItemEventType.Collision].Cast<CollisionItemEvent>();

                var candidate = collisionEvents.FirstOrDefault(e => e.ThingIdOfCollision == itemWithCollision.Type.TypeId && e.Setup(itemWithCollision, thing, requestor as IPlayer) && e.CanBeExecuted);

                // Execute all actions.
                candidate?.Execute();
            }
        }
    }
}