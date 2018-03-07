namespace OpenTibia.Communications.Packets
{
    internal interface IPlayerLogoutInfo
    {
        int AccountId { get; set; }
        short Level { get; set; }
        string Vocation { get; set; }
        string Residence { get; set; }
        int LastLogin { get; set; }
    }
}