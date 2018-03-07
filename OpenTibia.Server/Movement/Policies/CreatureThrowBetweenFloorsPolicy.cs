using OpenTibia.Server.Data.Interfaces;
using OpenTibia.Server.Data.Models.Structs;

namespace OpenTibia.Server.Movement.Policies
{
    internal class CreatureThrowBetweenFloorsPolicy : IMovementPolicy
    {
        public Location ToLocation { get; }
        public uint RequestorId { get; }
        public IThing Thing { get; }

        public string ErrorMessage => "You my not throw there.";

        public CreatureThrowBetweenFloorsPolicy(uint creatureRequestingId, IThing thingMoving, Location toLocation)
        {
            RequestorId = creatureRequestingId;
            Thing = thingMoving;
            ToLocation = toLocation;
        }

        public bool Evaluate()
        {
            var thingAsCreature = Thing as ICreature;
            var requestor = RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(RequestorId);

            if (requestor == null || thingAsCreature == null)
            {
                // Not a creature requesting this one, possibly a script.
                // Or the thing moving is null, not this policy's job to restrict this...
                return true;
            }

            var locDiff = thingAsCreature.Location - ToLocation;

            return locDiff.Z == 0;
        }
    }
}