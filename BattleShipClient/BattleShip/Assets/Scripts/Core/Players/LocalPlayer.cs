using System;
using UnityEngine;

namespace BattleShip.Core.Players
{
    [Serializable]
    public class LocalPlayer : Player 
    {
        public event Action PlayerNameChanged = delegate { Debug.LogWarning("Player Name Changed"); };

        internal void ChangePlayerName(string name, string id)
        {
            Id = id;
            Name = name;
            PlayerNameChanged();
        }
    }
}