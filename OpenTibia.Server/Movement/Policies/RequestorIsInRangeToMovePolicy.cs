// <copyright file="RequestorIsInRangeToMovePolicy.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Movement.Policies
{
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;

    internal class RequestorIsInRangeToMovePolicy : IMovementPolicy
    {
        public uint RequestorId { get; }

        public Location FromLocation { get; }

        public string ErrorMessage => "You are too far away.";

        public RequestorIsInRangeToMovePolicy(uint requestorId, Location movingFrom)
        {
            this.RequestorId = requestorId;
            this.FromLocation = movingFrom;
        }

        public bool Evaluate()
        {
            var requestor = this.RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(this.RequestorId);

            if (requestor == null)
            {
                // script called, probably
                return true;
            }

            return (requestor.Location - this.FromLocation).MaxValueIn2D <= 1;
        }
    }
}