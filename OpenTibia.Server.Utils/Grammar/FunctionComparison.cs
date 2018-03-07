using OpenTibia.Data.Contracts;

namespace OpenTibia.Utilities.Grammar
{
    public class FunctionComparison
    {
        public string Name { get; }

        public string[] Parameters { get; }

        public FunctionComparisonType Type { get; }

        public string CompareToIdentifier { get; }

        public FunctionComparison(string name, string comparisonType, string comparingTo, params string[] parameters)
        {
            Name = name;
            Parameters = parameters;
            CompareToIdentifier = comparingTo;
            Parameters = parameters;

            if (comparisonType == ">=")
            {
                Type = FunctionComparisonType.GreaterThanOrEqual;
            }
            else if (comparisonType == "<=")
            {
                Type = FunctionComparisonType.LessThanOrEqual;
            }
            else if (comparisonType == ">")
            {
                Type = FunctionComparisonType.GreaterThan;
            }
            else if (comparisonType == "<")
            {
                Type = FunctionComparisonType.LessThan;
            }
            else if (comparisonType == "==")
            {
                Type = FunctionComparisonType.Equal;
            }
        }
    }
}