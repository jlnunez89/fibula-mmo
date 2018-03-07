namespace OpenTibia.Utilities.Grammar
{
    public class ScriptFunction
    {
        public string Name { get; }

        public object[] Parameters { get; }

        public ScriptFunction(string name, params object[] parameters)
        {
            Name = name;
            Parameters = parameters;
        }
    }
}