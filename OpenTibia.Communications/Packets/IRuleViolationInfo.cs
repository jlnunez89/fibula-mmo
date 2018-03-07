namespace OpenTibia.Communications.Packets
{
    internal interface IRuleViolationInfo
    {
        int GamemasterId { get; set; }
        string CharacterName { get; set; }
        string IpAddress { get; set; }
        string Reason { get; set; }
        string Comment { get; set; }
    }
}