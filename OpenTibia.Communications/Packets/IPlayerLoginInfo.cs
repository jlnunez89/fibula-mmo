namespace OpenTibia.Communications.Packets
{
    internal interface IPlayerLoginInfo
    {
        ushort Os { get; set; }
        ushort Version { get; set; }
        uint[] XteaKey { get; set; }
        bool IsGm { get; set; }
        uint AccountNumber { get; set; }
        string CharacterName { get; set; }
        string Password { get; set; }
    }
}