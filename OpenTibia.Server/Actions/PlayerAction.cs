using System;
using System.Collections.Generic;
using System.Linq;
using OpenTibia.Communications.Packets.Incoming;
using OpenTibia.Communications.Packets.Outgoing;
using OpenTibia.Server.Data.Interfaces;
using OpenTibia.Server.Data.Models.Structs;
using OpenTibia.Server.Notifications;

namespace OpenTibia.Server.Actions
{
    internal abstract class PlayerAction : IAction
    {
        public IPlayer Player { get; }
        public IPacketIncoming Packet { get; }
        public Location RetryLocation { get; }

        public IList<IPacketOutgoing> ResponsePackets { get; }

        protected PlayerAction(IPlayer player, PacketIncoming packet, Location retryLocation)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            if (packet == null)
            {
                throw new ArgumentNullException(nameof(packet));
            }
            
            Player = player;
            Packet = packet;
            RetryLocation = retryLocation;
            ResponsePackets = new List<IPacketOutgoing>();
        }

        public void Perform()
        {
            InternalPerform();
            
            if (ResponsePackets.Any())
            {
                Game.Instance.NotifySinglePlayer(Player, conn => new GenericNotification(conn, ResponsePackets.Cast<PacketOutgoing>().ToArray()));
            }
        }

        protected abstract void InternalPerform();
    }
}