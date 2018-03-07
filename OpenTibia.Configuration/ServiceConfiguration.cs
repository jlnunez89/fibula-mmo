using System.Net;
using OpenTibia.Data.Contracts;

namespace OpenTibia.Configuration
{
    public class ServiceConfiguration
    {
        public WorldType WorldType { get; set; }
        public IPAddress PublicGameIpAddress { get; set; }
        public ushort PublicGamePort { get; set; }
        public byte DailyResetHour { get; set; }
        public ushort MaximumTotalPlayers { get; set; }
        public ushort PremiumMainlandBuffer { get; set; }
        public ushort MaximumRookgardians { get; set; }
        public ushort PremiumRookgardiansBuffer { get; set; }
        public string QueryManagerPassword { get; set; }
        public string MessageOfTheDay { get; set; }
        public string LocationString { get; set; }
        public int GameVersionInt { get; set; }
        public string GameVersionString { get; set; }
        public int ClientVersionInt { get; set; }
        public string ClientVersionString { get; set; }
        public string WebsiteUrl { get; set; }
        public IPAddress PrivateGameIpAddress { get; set; }
        public ushort PrivateGamePort { get; set; }
        public bool UsingCipsoftRsaKeys { get; set; }

        private static readonly object ConfigLock = new object();
        private static ServiceConfiguration _config;

        public static ServiceConfiguration GetConfiguration()
        {
            if(_config == null)
            {
                lock(ConfigLock)
                {
                    if(_config == null)
                    {
                        _config = new ServiceConfiguration
                        {
                            UsingCipsoftRsaKeys = true,
                            GameVersionInt = 10,
                            GameVersionString = "0.1",
                            ClientVersionInt = 770,
                            ClientVersionString = "7.7",
                            WorldType = WorldType.Hardcore,
                            PublicGameIpAddress = IPAddress.Parse("104.210.149.211"),
                            PublicGamePort = 7172,
                            PrivateGameIpAddress = IPAddress.Parse("10.0.0.5"),
                            PrivateGamePort = 7170,
                            DailyResetHour = 10,
                            MaximumTotalPlayers = 1500,
                            PremiumMainlandBuffer = 900,
                            MaximumRookgardians = 500,
                            PremiumRookgardiansBuffer = 250,
                            QueryManagerPassword = "a6glaf0c",
                            LocationString = "United States",
                            MessageOfTheDay = "Welcome to OpenTibia.",
                            WebsiteUrl = @"http://www.randomot.com"
                        };
                    }
                }
            }

            return _config;
        }
    }
}
