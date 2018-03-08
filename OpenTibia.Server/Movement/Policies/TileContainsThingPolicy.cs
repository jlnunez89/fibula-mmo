// <copyright file="TileContainsThingPolicy.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Movement.Policies
{
    using System;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;

    internal class TileContainsThingPolicy : IMovementPolicy
    {
        public Location Location { get; }

        public IThing Thing { get; }

        public byte Count { get; }

        public TileContainsThingPolicy(IThing thing, Location location, byte count = 1)
        {
            if (count == 0 || count > 100)
            {
                throw new ArgumentException($"Invalid count {count}.", nameof(count));
            }

            this.Thing = thing;
            this.Count = count;
            this.Location = location;
        }

        public string ErrorMessage => "Sorry, not possible.";

        public bool Evaluate()
        {
            if (this.Thing == null)
            {
                return false;
            }

            var sourceTile = Game.Instance.GetTileAt(this.Location);

            if (sourceTile == null || !sourceTile.HasThing(this.Thing))
            {
                // This tile no longer has the thing, or it's obstructed (i.e. someone placed something on top of it).
                return false;
            }

            return true;
        }
    }
}