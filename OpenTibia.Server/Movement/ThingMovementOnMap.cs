// <copyright file="ThingMovementOnMap.cs" company="2Dudes">
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

    internal class ThingMovementOnMap : MovementBase
    {
        public Location FromLocation { get; }

        public byte FromStackPos { get; }

        public ITile FromTile { get; }

        public Location ToLocation { get; }

        public ITile ToTile { get; }

        public IThing Thing { get; }

        public byte Count { get; }

        public bool IsTeleport { get; }

        public ThingMovementOnMap(uint creatureRequestingId, IThing thingMoving, Location fromLocation, byte fromStackPos, Location toLocation, byte count = 1, bool isTeleport = false)
            : base(creatureRequestingId)
        {
            // intentionally left thing null check out. Handled by Perform().
            var requestor = this.RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(this.RequestorId);

            if (count == 0)
            {
                throw new ArgumentException("Invalid count zero.");
            }

            this.FromLocation = fromLocation;
            this.FromStackPos = fromStackPos;
            this.FromTile = Game.Instance.GetTileAt(this.FromLocation);

            this.ToLocation = toLocation;
            this.ToTile = Game.Instance.GetTileAt(this.ToLocation);

            this.Thing = thingMoving;
            this.Count = count;
            this.IsTeleport = isTeleport;

            if (!isTeleport && requestor != null)
            {
                this.Policies.Add(new CanThrowBetweenPolicy(this.RequestorId, this.FromLocation, this.ToLocation));
            }

            this.Policies.Add(new RequestorIsInRangeToMovePolicy(this.RequestorId, this.FromLocation));
            this.Policies.Add(new LocationNotObstructedPolicy(this.RequestorId, this.Thing, this.ToLocation));
            this.Policies.Add(new LocationHasTileWithGroundPolicy(this.ToLocation));
            this.Policies.Add(new UnpassItemsInRangePolicy(this.RequestorId, this.Thing, this.ToLocation));
            this.Policies.Add(new LocationsMatchPolicy(this.Thing?.Location ?? new Location(), this.FromLocation));
            this.Policies.Add(new TileContainsThingPolicy(this.Thing, this.FromLocation, this.Count));
        }

        public override void Perform()
        {
            var requestor = this.RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(this.RequestorId);

            if (this.FromTile == null || this.ToTile == null)
            {
                return;
            }

            var thing = this.Thing;

            this.FromTile.RemoveThing(ref thing, this.Count);

            this.ToTile.AddThing(ref thing, thing.Count);

            if (thing is ICreature)
            {
                Game.Instance.NotifySpectatingPlayers(
                    conn => new CreatureMovedNotification(
                        conn,
                        (thing as ICreature).CreatureId,
                        this.FromLocation,
                        this.FromStackPos,
                        this.ToLocation,
                        this.ToTile.GetStackPosition(this.Thing),
                        this.IsTeleport),
                    this.FromLocation,
                    this.ToLocation);
            }
            else
            {
                // TODO: see if we can save network bandwith here:
                // Game.Instance.NotifySpectatingPlayers(
                //        (conn) => new ItemMovedNotification(
                //            conn,
                //            (IItem)this.Thing,
                //            this.FromLocation,
                //            oldStackpos,
                //            this.ToLocation,
                //            destinationTile.GetStackPosition(this.Thing),
                //            false
                //        ),
                //        this.FromLocation,
                //        this.ToLocation
                //    );
                Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, this.FromLocation, Game.Instance.GetMapTileDescription(conn.PlayerId, this.FromLocation)), this.FromLocation);

                Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, this.ToLocation, Game.Instance.GetMapTileDescription(conn.PlayerId, this.ToLocation)), this.ToLocation);
            }

            if (this.FromTile.HandlesSeparation)
            {
                foreach (var itemWithSeparation in this.FromTile.ItemsWithSeparation)
                {
                    var separationEvents = Game.Instance.EventsCatalog[EventType.Separation].Cast<SeparationEvent>();

                    var candidate = separationEvents.FirstOrDefault(e => e.ThingIdOfSeparation == itemWithSeparation.Type.TypeId && e.Setup(itemWithSeparation, thing, requestor as IPlayer) && e.CanBeExecuted);

                    // Execute all actions.
                    candidate?.Execute();
                }
            }

            if (this.ToTile.HandlesCollision)
            {
                foreach (var itemWithCollision in this.ToTile.ItemsWithCollision)
                {
                    var collisionEvents = Game.Instance.EventsCatalog[EventType.Collision].Cast<CollisionEvent>();

                    var candidate = collisionEvents.FirstOrDefault(e => e.ThingIdOfCollision == itemWithCollision.Type.TypeId && e.Setup(itemWithCollision, thing, requestor as IPlayer) && e.CanBeExecuted);

                    // Execute all actions.
                    candidate?.Execute();
                }
            }
        }
    }
}