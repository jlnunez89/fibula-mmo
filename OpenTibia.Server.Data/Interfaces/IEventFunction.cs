namespace OpenTibia.Server.Data.Interfaces
{
    public interface IEventFunction
    {
        string FunctionName { get; }
        
        object[] Parameters { get; }
    }
}