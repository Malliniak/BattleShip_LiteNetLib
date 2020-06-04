using System;

namespace BattleShip.Core.Players
{
    [Serializable]
    public class RemotePlayer : Player
    {
        internal RemotePlayer(string player, string id)
        {
            Name = player;
            Id = id;
        }
    }
}