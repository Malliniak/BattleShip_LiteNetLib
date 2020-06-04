using UnityEngine;

namespace BattleShip.Game
{
    public class ShipsColumn : MonoBehaviour
    {
        private GameplayManager _gameplayManager;

        private void Awake()
        {
            _gameplayManager = FindObjectOfType<GameplayManager>();
        }

        private void Start()
        {
            for (int i = 0; i < _gameplayManager._ships.Count; i++)
            {
                ShipRenderer ship = Instantiate(_gameplayManager.ShipRendererPrefab, transform).GetComponent<ShipRenderer>();
                ship.Ship = _gameplayManager._ships[i];
                ship.PopulateShipRender(ship.Ship._points.Count, _gameplayManager.PointRendererPrefab);
            }
        }
    }
}
