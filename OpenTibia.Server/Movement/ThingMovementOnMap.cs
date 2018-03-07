using System;
using System.Linq;
using OpenTibia.Data.Contracts;
using OpenTibia.Server.Data.Interfaces;
using OpenTibia.Server.Data.Models.Structs;
using OpenTibia.Server.Events;
using OpenTibia.Server.Movement.Policies;
using OpenTibia.Server.Notifications;

namespace OpenTibia.Server.Movement
{
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
            : base (creatureRequestingId)
        {
            // intentionally left thing null check out. Handled by Perform().
            var requestor = RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(RequestorId);

            if (count == 0)
            {
                throw new ArgumentException("Invalid count zero.");
            }

            FromLocation = fromLocation;
            FromStackPos = fromStackPos;
            FromTile = Game.Instance.GetTileAt(FromLocation);

            ToLocation = toLocation;
            ToTile = Game.Instance.GetTileAt(ToLocation);

            Thing = thingMoving;
            Count = count;
            IsTeleport = isTeleport;

            if (!isTeleport && requestor != null)
            {
                Policies.Add(new CanThrowBetweenPolicy(RequestorId, FromLocation, ToLocation));
            }

            Policies.Add(new RequestorIsInRangeToMovePolicy(RequestorId, FromLocation));
            Policies.Add(new LocationNotObstructedPolicy(RequestorId, Thing, ToLocation));
            Policies.Add(new LocationHasTileWithGroundPolicy(ToLocation));
            Policies.Add(new UnpassItemsInRangePolicy(RequestorId, Thing, ToLocation));
            Policies.Add(new LocationsMatchPolicy(Thing?.Location ?? new Location(), FromLocation));
            Policies.Add(new TileContainsThingPolicy(Thing, FromLocation, Count));
        }

        public override void Perform()
        {
            var requestor = RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(RequestorId);

            if (FromTile == null || ToTile == null)
            {
                return;
            }
            
            var thing = Thing;

            FromTile.RemoveThing(ref thing, Count);

            ToTile.AddThing(ref thing, thing.Count);

            if (thing is ICreature)
            {
                Game.Instance.NotifySpectatingPlayers(
                    conn => new CreatureMovedNotification(
                        conn,
                        (thing as ICreature).CreatureId,
                        FromLocation,
                        FromStackPos,
                        ToLocation,
                        ToTile.GetStackPosition(Thing),
                        IsTeleport
                    ),
                    FromLocation,
                    ToLocation
                );
            }
            else
            {
                // TODO: see if we can save network bandwith here:
                //Game.Instance.NotifySpectatingPlayers(
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

                Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, FromLocation, Game.Instance.GetMapTileDescription(conn.PlayerId, FromLocation)), FromLocation);

                Game.Instance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, ToLocation, Game.Instance.GetMapTileDescription(conn.PlayerId, ToLocation)), ToLocation);
            }

            if (FromTile.HandlesSeparation)
            {
                foreach (var itemWithSeparation in FromTile.ItemsWithSeparation)
                {
                    var separationEvents = Game.Instance.EventsCatalog[EventType.Separation].Cast<SeparationEvent>();

                    var candidate = separationEvents.FirstOrDefault(e => e.ThingIdOfSeparation == itemWithSeparation.Type.TypeId && e.Setup(itemWithSeparation, thing, requestor as IPlayer) && e.CanBeExecuted);

                    // Execute all actions.
                    candidate?.Execute();
                }
            }

            if (ToTile.HandlesCollision)
            {
                foreach (var itemWithCollision in ToTile.ItemsWithCollision)
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