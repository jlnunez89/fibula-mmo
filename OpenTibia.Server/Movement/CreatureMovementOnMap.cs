using System;
using OpenTibia.Data.Contracts;
using OpenTibia.Server.Data.Interfaces;
using OpenTibia.Server.Data.Models.Structs;
using OpenTibia.Server.Movement.Policies;

namespace OpenTibia.Server.Movement
{
    internal class CreatureMovementOnMap : ThingMovementOnMap
    {
        public Direction AttemptedDirection { get; }

        public CreatureMovementOnMap(uint creatureRequestingId, ICreature creatureMoving, Location fromLocation, Location toLocation, bool isTeleport = false, byte count = 1)
            : base(creatureRequestingId, creatureMoving, fromLocation, creatureMoving.GetStackPosition(), toLocation, count, isTeleport)
        {
            if (count == 0)
            {
                throw new ArgumentException("Invalid count zero.");
            }

            var requestor = RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(RequestorId);

            AttemptedDirection = fromLocation.DirectionTo(toLocation, true);

            if (IsTeleport || requestor == null)
            {
                return;
            }

            Policies.Add(new LocationNotAviodPolicy(RequestorId, Thing, ToLocation));
            Policies.Add(new LocationsAreDistantByPolicy(FromLocation, ToLocation));
            Policies.Add(new CreatureThrowBetweenFloorsPolicy(RequestorId, Thing, ToLocation));
        }

        public override void Perform()
        {
            base.Perform();

            if (IsTeleport)
            {
                return;
            }

            var requestor = Game.Instance.GetCreatureWithId(RequestorId);

            // update both creature's to face the push direction... a *real* push!
            if (requestor != Thing)
            {
                requestor?.TurnToDirection(requestor.Location.DirectionTo(Thing.Location));
            }

            ((Creature) Thing)?.TurnToDirection(AttemptedDirection);

            if (requestor != null && requestor == Thing)
            {
                requestor.UpdateLastStepInfo(requestor.NextStepId, wasDiagonal: AttemptedDirection > Direction.West);
            }
        }
    }
}