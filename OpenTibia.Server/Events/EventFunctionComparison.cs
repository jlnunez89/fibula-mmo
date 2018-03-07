using OpenTibia.Data.Contracts;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Server.Events
{
    internal class EventFunctionComparison : IEventFunction
    {
        public string FunctionName { get; }

        public object[] Parameters { get; }

        public FunctionComparisonType Type { get; }

        public string CompareToIdentifier { get; }

        public EventFunctionComparison(string name, FunctionComparisonType type, string compareIdentifier, object[] parameters)
        {
            FunctionName = name;
            Type = type;
            CompareToIdentifier = compareIdentifier;
            Parameters = parameters;
        }
    }
}