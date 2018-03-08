// <copyright file="CreatureMovementOnMap.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Movement
{
    using System;
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;
    using OpenTibia.Server.Movement.Policies;

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

            var requestor = this.RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(this.RequestorId);

            this.AttemptedDirection = fromLocation.DirectionTo(toLocation, true);

            if (this.IsTeleport || requestor == null)
            {
                return;
            }

            this.Policies.Add(new LocationNotAviodPolicy(this.RequestorId, this.Thing, this.ToLocation));
            this.Policies.Add(new LocationsAreDistantByPolicy(this.FromLocation, this.ToLocation));
            this.Policies.Add(new CreatureThrowBetweenFloorsPolicy(this.RequestorId, this.Thing, this.ToLocation));
        }

        public override void Perform()
        {
            base.Perform();

            if (this.IsTeleport)
            {
                return;
            }

            var requestor = Game.Instance.GetCreatureWithId(this.RequestorId);

            // update both creature's to face the push direction... a *real* push!
            if (requestor != this.Thing)
            {
                requestor?.TurnToDirection(requestor.Location.DirectionTo(this.Thing.Location));
            }

            ((Creature)this.Thing)?.TurnToDirection(this.AttemptedDirection);

            if (requestor != null && requestor == this.Thing)
            {
                requestor.UpdateLastStepInfo(requestor.NextStepId, wasDiagonal: this.AttemptedDirection > Direction.West);
            }
        }
    }
}