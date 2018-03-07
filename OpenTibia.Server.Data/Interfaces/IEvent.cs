using System.Collections.Generic;
using OpenTibia.Data.Contracts;

namespace OpenTibia.Server.Data.Interfaces
{
    public interface IEvent
    {
        EventType Type { get; }

        IThing Obj1 { get; }
        IThing Obj2 { get; }
        IPlayer User { get; }

        bool CanBeExecuted { get; }

        IEnumerable<IEventFunction> Conditions { get; }

        IEnumerable<IEventFunction> Actions { get; }

        bool Setup(IThing obj1, IThing obj2 = null, IPlayer user = null);

        void Execute();
    }
}