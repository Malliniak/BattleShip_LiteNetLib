using UnityEngine;

namespace BattleShip.Game
{
    public class ShipRenderer : MonoBehaviour
    {
        public Ship Ship { get; set; }
        public void PopulateShipRender(int pointsCount, GameObject prefab)
        {
            for (int i = 0; i < pointsCount; i++)
            {
                PointRenderer point = Instantiate(prefab, transform).GetComponent<PointRenderer>();
                point.Point = Ship._points[i];
                point.Image.color = Ship.Color;
            }
        }
    }
}
