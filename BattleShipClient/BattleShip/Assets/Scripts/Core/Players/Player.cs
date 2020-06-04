using System;
using UnityEngine;

namespace BattleShip.Core.Players
{
    [Serializable]
    public abstract class Player
    {
        [SerializeField]
        private string _id;
        [SerializeField] 
        private string _name;

        [SerializeField] private bool _isReady;
        public string Id
        {
            get => _id;
            set => _id = value;
        }

        public string Name
        {
            get => _name;
            internal set => _name = value;
        }

        public bool IsReady
        {
            get => _isReady;
            set => _isReady = value;
        }

        internal Player(string name = "Unnamed PLayer")
        {
            Name = name;
        }
    }
}
