namespace OpenTibia.Server.Data.Models.Structs
{
    public struct Outfit
    {
        public ushort LikeType { get; set; }
        public ushort Id { get; set; }
        public byte Head { get; set; }
        public byte Body { get; set; }
        public byte Legs { get; set; }
        public byte Feet { get; set; }
    }
}