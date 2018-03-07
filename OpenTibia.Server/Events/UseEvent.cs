using System;
using System.Collections.Generic;
using System.Linq;
using OpenTibia.Data.Contracts;

namespace OpenTibia.Server.Events
{
    internal class UseEvent : BaseEvent
    {
        public ushort ItemToUseId { get; }
        
        public UseEvent(IList<string> conditionSet, IList<string> actionSet)
            : base (conditionSet, actionSet)
        {
            // Look for a IsType condition. 

            var isTypeCondition = Conditions.FirstOrDefault(func => IsTypeFunctionName.Equals(func.FunctionName));

            if (isTypeCondition == null)
            {
                throw new ArgumentNullException($"Unable to find {IsTypeFunctionName} function.");
            }

            ItemToUseId = Convert.ToUInt16(isTypeCondition.Parameters[1]);
        }

        public override EventType Type => EventType.Use;
    }
}