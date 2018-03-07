using System.Timers;

namespace OpenTibia.Server.Data.Interfaces
{
    public interface IExpireable
    {
        Timer Timer { get; } 

        void OnExpire();

        void ExpirationSetup();
    }
}
