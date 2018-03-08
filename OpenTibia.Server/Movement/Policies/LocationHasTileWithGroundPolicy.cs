// <copyright file="LocationHasTileWithGroundPolicy.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Movement.Policies
{
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;

    internal class LocationHasTileWithGroundPolicy : IMovementPolicy
    {
        public Location Location { get; }

        public string ErrorMessage => "There is not enough room.";

        public LocationHasTileWithGroundPolicy(Location location)
        {
            this.Location = location;
        }

        public bool Evaluate()
        {
            return Game.Instance.GetTileAt(this.Location)?.Ground != null;
        }
    }
}