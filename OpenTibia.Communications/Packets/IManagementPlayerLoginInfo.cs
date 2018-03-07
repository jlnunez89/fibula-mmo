namespace OpenTibia.Communications.Packets
{
    internal interface IManagementPlayerLoginInfo
    {
        uint AccountNumber { get; set; }
        string CharacterName { get; set; }
        string Password { get; set; }
        string IpAddress { get; set; }
    }
}