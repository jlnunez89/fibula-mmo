using System;
using System.Collections.Generic;

namespace OpenTibia.Communications.Packets
{
    internal interface IStatementInfo
    {
        uint Unknown { get; set; }
        uint StatementId { get; set; }
        ushort Count { get; set; }
        IList<Tuple<uint, uint, string, string>> Data { get; set; }
    }
}