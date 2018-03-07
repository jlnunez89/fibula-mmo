using OpenTibia.Data.Contracts;

namespace OpenTibia.Server.Data.Models.Structs
{
    public struct Speech
    {
        public SpeechType Type { get; set; }
        public string Receiver { get; set; }
        public string Message { get; set; }
        public ChatChannel ChannelId { get; set; }
        
    }
}