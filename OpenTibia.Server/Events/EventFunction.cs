using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Server.Events
{
    internal class EventFunction : IEventFunction
    {
        public string FunctionName { get; }
        public object[] Parameters { get; }

        public EventFunction(string name, params object[] parameters)
        {
            FunctionName = name;
            Parameters = parameters;
        }
    }
}