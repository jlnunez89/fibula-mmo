using System;
using System.Collections.Generic;
using System.Linq;
using OpenTibia.Data.Contracts;

namespace OpenTibia.Server.Events
{
    internal class SeparationEvent : BaseEvent
    {
        public ushort ThingIdOfSeparation { get; }

        public SeparationEvent(IList<string> conditionSet, IList<string> actionSet)
            : base (conditionSet, actionSet)
        {
            var isTypeCondition = Conditions.FirstOrDefault(func => IsTypeFunctionName.Equals(func.FunctionName));

            if (isTypeCondition == null)
            {
                throw new ArgumentNullException($"Unable to find {IsTypeFunctionName} function.");
            }

            ThingIdOfSeparation = Convert.ToUInt16(isTypeCondition.Parameters[1]);
        }

        public override EventType Type => EventType.Separation;
    }
}