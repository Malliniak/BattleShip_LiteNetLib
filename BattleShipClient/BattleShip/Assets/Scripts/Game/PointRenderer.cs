using System;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = System.Numerics.Vector2;

namespace BattleShip.Game
{
    [Serializable]
    public class PointRenderer : MonoBehaviour
    {
        public Point Point { get; set; }
        public Vector2 Position { get; set; }

        public Image Image { get; set; }

        private void Awake()
        {
            Image = GetComponent<Image>();
        }
    }
}
