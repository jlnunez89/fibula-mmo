using System;
using OpenTibia.Communications;
using OpenTibia.Communications.Packets.Incoming;
using OpenTibia.Server.Data;
using OpenTibia.Server.Notifications;

namespace OpenTibia.Server.Handlers
{
    internal class SpeechHandler : IncomingPacketHandler
    {
        public override void HandlePacket(NetworkMessage message, Connection connection)
        {
            var speechPacket = new SpeechPacket(message);
            var player = Game.Instance.GetCreatureWithId(connection.PlayerId) as Player;

            if (player == null)
            {
                return;
            }

            // TODO: proper implementation.

            var msgStr = speechPacket.Speech.Message;

            if (msgStr.ToLower().StartsWith("test"))
            {
                Game.Instance.TestingViaCreatureSpeech(player, msgStr);
            }

            // TODO: implement all spells and speech related hooks.

            Game.Instance.NotifySpectatingPlayers(conn => new CreatureSpokeNotification(connection, player, speechPacket.Speech.Type, speechPacket.Speech.Message, speechPacket.Speech.ChannelId), player.Location);

            Console.WriteLine($"{player.Name}: {speechPacket.Speech.Message}");
        }
    }
}