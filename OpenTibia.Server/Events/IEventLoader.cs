using System.Collections.Generic;
using OpenTibia.Data.Contracts;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Server.Events
{
    public interface IEventLoader
    {
        IDictionary<EventType, HashSet<IEvent>> Load(string moveUseFileName);
    }
}
