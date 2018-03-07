namespace OpenTibia.Communications.Packets
{
    internal interface IAuthenticationInfo
    {
        byte Unknown { get; }
        string Password { get; }
        string WorldName { get; }
    }
}