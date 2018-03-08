// <copyright file="CanThrowBetweenPolicy.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Movement.Policies
{
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;

    internal class CanThrowBetweenPolicy : IMovementPolicy
    {
        public Location FromLocation { get; }

        public Location ToLocation { get; }

        public bool CheckSight { get; }

        public uint RequestorId { get; }

        public string ErrorMessage => "You may not throw there.";

        public CanThrowBetweenPolicy(uint requestorId, Location fromLocation, Location toLocation, bool checkLineOfSight = true)
        {
            this.RequestorId = requestorId;
            this.FromLocation = fromLocation;
            this.ToLocation = toLocation;
            this.CheckSight = checkLineOfSight;
        }

        public bool Evaluate()
        {
            var requestor = this.RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(this.RequestorId);

            if (requestor == null)
            {
                // Empty requestorId means not a creature generated event...
                return true;
            }

            return Game.Instance.CanThrowBetween(this.FromLocation, this.ToLocation, this.CheckSight);
        }
    }
}