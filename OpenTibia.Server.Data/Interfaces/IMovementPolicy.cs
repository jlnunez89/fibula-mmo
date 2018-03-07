namespace OpenTibia.Server.Data.Interfaces
{
    public interface IMovementPolicy
    {
        string ErrorMessage { get; }

        bool Evaluate();
    }
}
