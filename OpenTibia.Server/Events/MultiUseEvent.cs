using System;
using System.Collections.Generic;
using System.Linq;
using OpenTibia.Data.Contracts;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Server.Events
{
    internal class MultiUseEvent : BaseEvent
    {
        public ushort ItemToUseId { get; }

        public ushort ItemToUseOnId { get; }

        public MultiUseEvent(IList<string> conditionSet, IList<string> actionSet)
            : base (conditionSet, actionSet)
        {
            // Look for a IsType condition. 

            var isTypeConditions = Conditions.Where(func => IsTypeFunctionName.Equals(func.FunctionName));

            var typeConditionsList = isTypeConditions as IList<IEventFunction> ?? isTypeConditions.ToList();
            var firstTypeCondition = typeConditionsList.FirstOrDefault();
            var secondTypeCondition = typeConditionsList.Skip(1).FirstOrDefault();

            if (firstTypeCondition == null)
            {
                throw new ArgumentNullException($"Unable to find first {IsTypeFunctionName} function.");
            }

            if (secondTypeCondition == null)
            {
                throw new ArgumentNullException($"Unable to find second {IsTypeFunctionName} function.");
            }

            ItemToUseId = Convert.ToUInt16(firstTypeCondition.Parameters[1]);
            ItemToUseOnId = Convert.ToUInt16(secondTypeCondition.Parameters[1]);
        }

        public override EventType Type => EventType.MultiUse;
    }
}