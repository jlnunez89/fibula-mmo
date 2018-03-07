using System.Collections.Generic;
using OpenTibia.Data.Contracts;

namespace OpenTibia.Server.Events
{
    internal class MovementEvent : BaseEvent
    {
        public MovementEvent(IList<string> conditionSet, IList<string> actionSet)
            : base (conditionSet, actionSet)
        {

        }

        public override EventType Type => EventType.Movement;
    }
}