
namespace OpenTibia.Communications.Packets
{
    public interface IAccountLoginInfo
    {
        uint AccountNumber { get; set; }
        string Password { get; set; }
        uint[] XteaKey { get; set; }
    }
}
