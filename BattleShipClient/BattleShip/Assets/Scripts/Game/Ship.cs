using System.Collections.Generic;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

namespace BattleShip.Game
{
    public class Ship
    {
        public int Size { get; }
        public List<Point> _points = new List<Point>();
        public Color Color { get; set; }

        public Ship(int size, Color color)
        {
            Size = size;
            Color = color;
            for (int i = 0; i < Size; i++)
            {
                _points.Add(new Point {Color = Color});
            }
        }
        
        public void MarkHitPoint(Vector2 position)
        {
            var pointToMark = _points.Find(x => x.Position == position);
            if (pointToMark == null)
                return;
            pointToMark.IsHit = true;
        }
    }
}