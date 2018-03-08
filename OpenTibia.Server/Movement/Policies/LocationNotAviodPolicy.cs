// <copyright file="LocationNotAviodPolicy.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Movement.Policies
{
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;

    internal class LocationNotAviodPolicy : IMovementPolicy
    {
        public Location Location { get; }

        public IThing Thing { get; }

        public uint RequestorId { get; }

        public string ErrorMessage => "Sorry, not possible.";

        public LocationNotAviodPolicy(uint requestingCreatureId, IThing thing, Location location)
        {
            this.RequestorId = requestingCreatureId;
            this.Thing = thing;
            this.Location = location;
        }

        public bool Evaluate()
        {
            var requestor = this.RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(this.RequestorId);
            var destTile = Game.Instance.GetTileAt(this.Location);

            if (requestor == null || this.Thing == null || destTile == null)
            {
                // requestor being null means this was probably called from a script.
                // Not this policy's job to restrict this.
                return true;
            }

            return !(this.Thing is ICreature) || requestor == this.Thing || destTile.CanBeWalked();
        }
    }
}