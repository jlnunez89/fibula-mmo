using System.Collections.Generic;
using System.Linq;
using OpenTibia.Communications;
using OpenTibia.Communications.Packets.Incoming;
using OpenTibia.Communications.Packets.Outgoing;
using OpenTibia.Data;
using OpenTibia.Data.Models;
using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Server.Handlers.Management
{
    internal class CreatePlayerListHandler : IIncomingPacketHandler
    {
        public IList<IPacketOutgoing> ResponsePackets { get; private set; }

        public void HandlePacket(NetworkMessage message, Connection connection)
        {
            var createPlayerListPacket = new CreatePlayerListPacket(message);
            
            using (var otContext = new OpenTibiaDbContext())
            {
                var currentRecord = otContext.Stats.Select(s => s.RecordOnline).FirstOrDefault();
                var isNewRecord = createPlayerListPacket.PlayerList.Count > currentRecord;

                var currentRemove = new Dictionary<string, OnlinePlayer>();

                foreach(var player in otContext.Online.ToList())
                {
                    currentRemove.Add(player.Name, player);
                }
                
                foreach (var player in createPlayerListPacket.PlayerList)
                {
                    var dbRecord = otContext.Online.Where(o => o.Name.Equals(player.Name)).FirstOrDefault();

                    if(dbRecord != null)
                    {
                        dbRecord.Level = player.Level;
                        dbRecord.Vocation = player.Vocation;
                    }
                    else
                    {
                        otContext.Online.Add(new OnlinePlayer
                        {
                            Name = player.Name,
                            Level = player.Level,
                            Vocation = player.Vocation
                        });
                    }

                    if(currentRemove.ContainsKey(player.Name))
                    {
                        currentRemove.Remove(player.Name);
                    }
                }
                
                foreach (var player in currentRemove.Values)
                {
                    otContext.Online.Remove(player);
                }
                
                otContext.SaveChanges();

                ResponsePackets.Add(new CreatePlayerListResultPacket
                {
                    IsNewRecord = isNewRecord 
                });
            }
        }
    }
}