using System;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

namespace BattleShip.Game
{
    [Serializable]
    public class Point
    {
        public Vector2 Position { get; set; } = new Vector2();
        public bool IsHit { get; set; } = false;
        public Color Color { get; set; }
    }
}