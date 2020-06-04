using System.Collections.Generic;
using BattleShipCoreServer.Core;
using BattleShipShared.Packets.Player;
using LiteNetLib;
using Serilog;

namespace BattleShipCoreServer.Players
{
    public class PlayersManager
    {
        private HashSet<Player> Players { get; } = new HashSet<Player>();
        private readonly Server _server;

        public PlayersManager(Server server)
        {
            _server = server;
        }

        public void ChangePlayerConfig(string value, Player player)
        {
            Log.Information("Changing {player} name to {newName}", player.Name, value);
            player.Name = value;
            player._Peer.Send(_server.Processor.Write(new PlayerPacket
                {
                    PlayerCommand = PlayerCommand.CHANGE_PLAYER_NAME,
                    PlayerName = player.Name,
                    PlayerId = player.Id
                }),
                DeliveryMethod.ReliableOrdered);
        }

        public Player GetConnectedPlayer(NetPeer peer)
        {
            return Players.Find(x => x._Peer == peer);
        } 
        
        public void RegisterPlayer(NetPeer peer)
        {
            var player = new Player(peer, "UnnamedPlayer");
            Players.Add(player);
            player._Peer.Send(_server.Processor.Write(new PlayerPacket
                {
                    PlayerCommand = PlayerCommand.CHANGE_PLAYER_NAME,
                    PlayerName = player.Name,
                    PlayerId = player.Id
                }),
                DeliveryMethod.ReliableOrdered);
        }
    }
}