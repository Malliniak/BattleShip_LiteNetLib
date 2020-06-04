using System.Collections.Generic;
using UnityEngine;

namespace BattleShip.Game
{
    public class ClickableGameBoard : MonoBehaviour
    {
        private GameplayManager _gameplayManager;
        [SerializeField]
        private Dictionary<Vector2, ClickablePointRenderer> _points = new Dictionary<Vector2, ClickablePointRenderer>();
    
        public Dictionary<Vector2, ClickablePointRenderer> Points => _points;

        public GameObject _clickablePointPrefab;

        private void Awake()
        {
            _gameplayManager = FindObjectOfType<GameplayManager>();

            if (_gameplayManager != null)
                _gameplayManager.ClickableGameBoard = this;
        }

        private void Start()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    ClickablePointRenderer pointRenderer = Instantiate(_clickablePointPrefab, transform).GetComponent<ClickablePointRenderer>();
                    pointRenderer.Position = new Vector2(j, i);
                    pointRenderer.ClickableGameBoard = this;
                    pointRenderer.SetInteractable(false);
                    _points.Add(pointRenderer.Position, pointRenderer);
                }
            }
        }

        public void SetInteractable(bool value)
        {
            foreach (var VARIABLE in _points)
            {
                VARIABLE.Value.SetInteractable(value);
            }
        }

        public void SendHit(Vector2 position)
        {
            _gameplayManager.SendHit(position);
        }
    }
}
