using System.Collections.Generic;
using OpenTibia.Data.Contracts;

namespace OpenTibia.Communications.Packets
{
    internal interface IPlayerListInfo
    {
        IList<IOnlinePlayer> PlayerList { get; set; }
    }
}