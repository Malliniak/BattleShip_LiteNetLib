﻿namespace BattleShipShared.Packets.Player
{
   public class PlayerPacket
    {
        public PlayerCommand PlayerCommand { get; set; }
        public string PlayerName { get; set; }
        public string PlayerId { get; set; }
    }
}