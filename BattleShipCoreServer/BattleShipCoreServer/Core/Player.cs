using System;
using LiteNetLib;

namespace BattleShipCoreServer.Core
{
    public class Player
    {
        public NetPeer _Peer;
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsReady { get; set; }
        public bool IsLoadedToScene { get; set; }

        public Player(NetPeer peer, string name)
        {
            _Peer = peer;
            Name = name;
            Id = GenerateUserId();
        }
        
                
        private string GenerateUserId()
        {
            Random _random = new Random();
            string code = "USER_";
            for (int i = 0; i < 3; i++)
            {
                code += $"{_random.Next(0, 9)}{(Char)_random.Next(65,90)}";
            }

            return code;
        }
    }
}