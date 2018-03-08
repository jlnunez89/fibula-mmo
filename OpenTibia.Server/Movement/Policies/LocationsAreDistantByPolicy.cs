// <copyright file="LocationsAreDistantByPolicy.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Movement.Policies
{
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;

    internal class LocationsAreDistantByPolicy : IMovementPolicy
    {
        public Location FirstLocation { get; }

        public Location SecondLocation { get; }

        public byte Distance { get; }

        public bool SameFloorOnly { get; }

        public string ErrorMessage => "The destination is too far away.";

        public LocationsAreDistantByPolicy(Location locationOne, Location locationTwo, byte distance = 1, bool sameFloorOnly = false)
        {
            this.FirstLocation = locationOne;
            this.SecondLocation = locationTwo;
            this.Distance = distance;
            this.SameFloorOnly = sameFloorOnly;
        }

        public bool Evaluate()
        {
            var locationDiff = (this.FirstLocation - this.SecondLocation).MaxValueIn2D;
            var sameFloor = this.FirstLocation.Z == this.SecondLocation.Z;

            if (locationDiff <= this.Distance && (!this.SameFloorOnly || sameFloor))
            {
                // The thing is no longer in this position.
                return true;
            }

            return false;
        }
    }
}