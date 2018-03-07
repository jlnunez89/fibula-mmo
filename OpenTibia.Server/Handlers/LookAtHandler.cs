using System;
using OpenTibia.Communications;
using OpenTibia.Communications.Packets.Incoming;
using OpenTibia.Communications.Packets.Outgoing;
using OpenTibia.Data.Contracts;
using OpenTibia.Server.Data;
using OpenTibia.Server.Data.Interfaces;

namespace OpenTibia.Server.Handlers
{
    internal class LookAtHandler : IncomingPacketHandler
    {
        public override void HandlePacket(NetworkMessage message, Connection connection)
        {
            var lookAtPacket = new LookAtPacket(message);
            IThing thing = null;
            var player = Game.Instance.GetCreatureWithId(connection.PlayerId) as Player;

            if (player == null)
            {
                return;
            }

            Console.WriteLine($"LookAt {lookAtPacket.ThingId}.");
            
            if (lookAtPacket.Location.Type != LocationType.Ground || player.CanSee(lookAtPacket.Location))
            {
                // Get thing at location
                switch (lookAtPacket.Location.Type)
                {
                    case LocationType.Ground:
                        thing = Game.Instance.GetTileAt(lookAtPacket.Location).GetThingAtStackPosition(lookAtPacket.StackPosition);
                        break;
                    case LocationType.Container:
                        // TODO: implement containers.
                        //Container container = player.Inventory.GetContainer(location.Container);
                        //if (container != null)
                        //{
                        //    return container.GetItem(location.ContainerPosition);
                        //}
                        break;
                    case LocationType.Slot:
                        thing = player.Inventory[(byte)lookAtPacket.Location.Slot];
                        break;
                }

                if (thing != null)
                {
                    ResponsePackets.Add(new TextMessagePacket
                    {
                        Type = MessageType.DescriptionGreen,
                        Message = $"You see {thing.InspectionText}."
                    });
                }
            }
        }
    }
}