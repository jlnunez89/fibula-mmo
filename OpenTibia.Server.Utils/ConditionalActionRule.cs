using System.Collections.Generic;
using System.Linq;

namespace OpenTibia.Utilities
{
    public class ConditionalActionRule
    {
        public IList<string> ConditionSet { get; }
        public IList<string> ActionSet { get; }

        public ConditionalActionRule(IEnumerable<string> conditions, IEnumerable<string> actions)
        {
            ConditionSet = conditions.ToList();
            ActionSet = actions.ToList();
        }
    }
}
