// <copyright file="UnpassItemsInRangePolicy.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Movement.Policies
{
    using OpenTibia.Data.Contracts;
    using OpenTibia.Server.Data.Interfaces;
    using OpenTibia.Server.Data.Models.Structs;

    internal class UnpassItemsInRangePolicy : IMovementPolicy
    {
        public Location ToLocation { get; }

        public IThing Thing { get; }

        public uint MoverId { get; }

        public string ErrorMessage => "Sorry, not possible.";

        public UnpassItemsInRangePolicy(uint moverId, IThing thingMoving, Location targetLoc)
        {
            this.MoverId = moverId;
            this.Thing = thingMoving;
            this.ToLocation = targetLoc;
        }

        public bool Evaluate()
        {
            var mover = this.MoverId == 0 ? null : Game.Instance.GetCreatureWithId(this.MoverId);
            var item = this.Thing as IItem;

            if (item == null || mover == null || !item.Type.Flags.Contains(ItemFlag.Unpass))
            {
                // MoverId being null means this is probably a script's action.
                // Policy does not apply to this thing.
                return true;
            }

            var locDiff = mover.Location - this.ToLocation;

            return locDiff.Z == 0 && locDiff.MaxValueIn2D <= 2;
        }
    }
}