// <copyright file="ThingMovementGroundToContainer.cs" company="2Dudes">
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

    internal class ThingMovementGroundToContainer : MovementBase
    {
        public Location FromLocation { get; }

        public byte FromStackPos { get; }

        public ITile FromTile { get; }

        public Location ToLocation { get; }

        public IContainer ToContainer { get; }

        public byte ToIndex { get; }

        public IThing Thing { get; }

        public byte Count { get; }

        public ThingMovementGroundToContainer(uint creatureRequestingId, IThing thingMoving, Location fromLocation, byte fromStackPos, Location toLocation, byte count = 1)
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
            this.FromStackPos = fromStackPos;
            this.FromTile = Game.Instance.GetTileAt(this.FromLocation);

            this.ToLocation = toLocation;
            this.ToContainer = (requestor as IPlayer)?.GetContainer(toLocation.Container);
            this.ToIndex = (byte)this.ToLocation.Z;

            if (this.ToContainer != null && this.ToContainer.HolderId == this.RequestorId)
            {
                this.Policies.Add(new GrabberHasEnoughCarryStrengthPolicy(this.RequestorId, this.Thing));
            }

            this.Policies.Add(new GrabberHasContainerOpenPolicy(this.RequestorId, this.ToContainer));
            this.Policies.Add(new ContainerHasEnoughCapacityPolicy(this.ToContainer));
            this.Policies.Add(new ThingIsTakeablePolicy(this.RequestorId, this.Thing));
            this.Policies.Add(new LocationsMatchPolicy(this.Thing?.Location ?? new Location(), this.FromLocation));
            this.Policies.Add(new TileContainsThingPolicy(this.Thing, this.FromLocation, this.Count));
        }

        public override void Perform()
        {
            var thingAsItem = this.Thing as IItem;
            var requestor = this.RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(this.RequestorId);

            if (this.FromTile == null || this.ToContainer == null || this.Thing == null || thingAsItem == null)
            {
                return;
            }

            var thingAtTile = this.FromTile.GetThingAtStackPosition(this.FromStackPos);

            if (thingAtTile == null)
            {
                return;
            }

            var thing = this.Thing;
            this.FromTile.RemoveThing(ref thing, this.Count);

            // notify all spectator players of that tile.
            Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, this.FromTile.Location, Game.Instance.GetMapTileDescription(conn.PlayerId, this.FromTile.Location)), this.FromTile.Location);

            if (thing != this.Thing)
            {
                // item got split cause we removed less than the total amount.
                // update the thing we're adding to the container.
                thingAsItem = thing as IItem;
            }

            // attempt to add the item to the container.
            if (thingAsItem == null || this.ToContainer.AddContent(thingAsItem, this.ToIndex))
            {
                // and call any separation events.
                if (this.FromTile.HandlesSeparation) // TODO: what happens on separation of less than required quantity, etc?
                {
                    foreach (var itemWithSeparation in this.FromTile.ItemsWithSeparation)
                    {
                        var separationEvents = Game.Instance.EventsCatalog[EventType.Separation].Cast<SeparationEvent>();

                        var candidate = separationEvents.FirstOrDefault(e => e.ThingIdOfSeparation == itemWithSeparation.Type.TypeId && e.Setup(itemWithSeparation, thing, requestor as IPlayer) && e.CanBeExecuted);

                        // Execute all actions.
                        candidate?.Execute();
                    }
                }

                return;
            }

            // failed to add to the dest container (whole or partial)
            // add again to the source tile
            IThing itemAsThing = thingAsItem;
            this.FromTile.AddThing(ref itemAsThing, thingAsItem.Count);

            // notify all spectator players of that tile.
            Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, this.FromTile.Location, Game.Instance.GetMapTileDescription(conn.PlayerId, this.FromTile.Location)), this.FromTile.Location);

            // call any collision events again.
            if (!this.FromTile.HandlesCollision)
            {
                return;
            }

            foreach (var itemWithCollision in this.FromTile.ItemsWithCollision)
            {
                var collisionEvents = Game.Instance.EventsCatalog[EventType.Collision].Cast<CollisionEvent>();

                var candidate = collisionEvents.FirstOrDefault(e => e.ThingIdOfCollision == itemWithCollision.Type.TypeId && e.Setup(itemWithCollision, this.Thing) && e.CanBeExecuted);

                // Execute all actions.
                candidate?.Execute();
            }
        }
    }
}