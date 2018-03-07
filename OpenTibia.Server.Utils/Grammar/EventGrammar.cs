using System;
using System.Linq;
using OpenTibia.Data.Contracts;
using Sprache;

namespace OpenTibia.Utilities.Grammar
{
    public class EventGrammar
    {
        public static readonly Parser<MoveUseEvent> Event =
            from rule in CipGrammar.ConditionalActionRule
            select new MoveUseEvent(rule);

        public class MoveUseEvent
        {
            public EventType Type { get; }
            public ConditionalActionRule Rule { get; }

            public MoveUseEvent(ConditionalActionRule rule)
            {
                if (rule == null)
                {
                    throw new ArgumentNullException(nameof(rule));
                }

                var firstCondition = rule.ConditionSet.FirstOrDefault();

                EventType eventType;

                if (!Enum.TryParse(firstCondition, out eventType))
                {
                    throw new ArgumentException("Invalid rule supplied.");
                }

                Type = eventType;
                Rule = rule;

                rule.ConditionSet.RemoveAt(0); // remove first.
            }
        }
    }
}
