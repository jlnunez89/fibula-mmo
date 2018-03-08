// <copyright file="CreatureThrowBetweenFloorsPolicy.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Movement.Policies
{
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;

    internal class CreatureThrowBetweenFloorsPolicy : IMovementPolicy
    {
        public Location ToLocation { get; }

        public uint RequestorId { get; }

        public IThing Thing { get; }

        public string ErrorMessage => "You my not throw there.";

        public CreatureThrowBetweenFloorsPolicy(uint creatureRequestingId, IThing thingMoving, Location toLocation)
        {
            this.RequestorId = creatureRequestingId;
            this.Thing = thingMoving;
            this.ToLocation = toLocation;
        }

        public bool Evaluate()
        {
            var thingAsCreature = this.Thing as ICreature;
            var requestor = this.RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(this.RequestorId);

            if (requestor == null || thingAsCreature == null)
            {
                // Not a creature requesting this one, possibly a script.
                // Or the thing moving is null, not this policy's job to restrict this...
                return true;
            }

            var locDiff = thingAsCreature.Location - this.ToLocation;

            return locDiff.Z == 0;
        }
    }
}