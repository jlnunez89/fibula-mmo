using System;

namespace OpenTibia.Server.Data.Models.Structs
{
    public struct Spawn
    {
        public ushort Id { get; set; }
        public Location Location { get; set; }
        public ushort Radius { get; set; }
        public byte Count { get; set; }
        public TimeSpan Regen { get; set; }
    }
}
