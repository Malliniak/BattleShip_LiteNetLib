using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using Vector2 = System.Numerics.Vector2;

namespace BattleShip.Game
{
    public class GameBoard : MonoBehaviour
    {
        private GameplayManager _gameplayManager;
        [SerializeField]
        private Dictionary<Vector2, PointRenderer> _points = new Dictionary<Vector2, PointRenderer>();
        private List<Ship> _ships = new List<Ship>();
    
        public Dictionary<Vector2, PointRenderer> Points => _points;

        private void Awake()
        {
            _gameplayManager = FindObjectOfType<GameplayManager>();

            if (_gameplayManager != null)
                _gameplayManager.GameBoard = this;
        }

        private void Start()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    PointRenderer pointRenderer = Instantiate(_gameplayManager.PointRendererPrefab, transform).GetComponent<PointRenderer>();
                    pointRenderer.Position = new Vector2(j, i);
                    _points.Add(pointRenderer.Position, pointRenderer);
                }
            }

            _ships = _gameplayManager._ships;
            PopulateBoard();
        }

        public void PopulateBoard()
        {
            foreach (var VARIABLE in _points)
            {
                VARIABLE.Value.Point = null;
                VARIABLE.Value.Image.color = Color.white;
            }
            Random random = new Random();
            for (int index = 0; index < _ships.Count; index++)
            {
                var ship = _ships[index];
                bool pointAquired = false;
                int randomX = 0;
                int randomY = 0;

                foreach (var VARIABLE in _points)
                {
                    int X = random.Next(0, 8);
                    int Y = random.Next(0, 8);


                    if (X >= ship.Size)
                        X -= ship.Size;

                    Debug.Log(X);
                    Debug.Log(Y);
                    for (int i = 0; i < ship.Size; i++)
                    {
                        if (_points[new Vector2(X + i, Y)].Point != null)
                        {
                            pointAquired = false;
                            continue;
                        }

                        pointAquired = true;
                    }

                    if (pointAquired)
                    {
                        randomX = X;
                        randomY = Y;
                        break;
                    }
                }

                for (int i = 0; i < ship.Size; i++)
                {
                    _points[new Vector2(randomX + i, randomY)].Point = ship._points[i];
                    _points[new Vector2(randomX + i, randomY)].Image.color = ship._points[i].Color;
                }
            }
        }
    }
}
