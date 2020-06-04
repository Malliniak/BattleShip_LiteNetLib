using System;
using System.Collections.Generic;
using BattleShip.Core;
using BattleShipShared.Packets.Game;
using UnityEngine;
using Object = System.Object;
using Vector2 = System.Numerics.Vector2;

namespace BattleShip.Game
{
    public class GameplayManager : MonoBehaviour
    {
        public List<Ship> _ships = new List<Ship>();

        [SerializeField] private GameObject _shipRendererPrefab;
        [SerializeField] private GameObject _pointRendererPrefab;
        public GameObject ShipRendererPrefab => _shipRendererPrefab;
        public GameObject PointRendererPrefab => _pointRendererPrefab;
        public GameBoard GameBoard { get; set; }
        public ClickableGameBoard ClickableGameBoard { get; set; }

        public event Action<string> PlayerIdTurnUpdate;
        public event Action<Vector2, bool> EnemyShoot;

        private RuntimeManager _runtimeManager;
        private void Awake()
        {
            _ships.Add(new Ship(5, Color.blue));
            _ships.Add(new Ship(4, Color.green));
            _ships.Add(new Ship(3, Color.cyan));
            _ships.Add(new Ship(2,Color.magenta));
            _ships.Add(new Ship(2, Color.magenta));
            _ships.Add(new Ship(1, Color.yellow));
            _ships.Add(new Ship(1, Color.yellow));

            _runtimeManager = FindObjectOfType<RuntimeManager>();

            if (_runtimeManager != null)
                _runtimeManager.GameplayManager = this;
        }

        private void Start()
        {
            _runtimeManager.NetHub.PlayerRoomStateRequest(_runtimeManager.Player.Id, _runtimeManager.Room.RoomId, false);
        }

        public bool CheckHit(PointStruct gamePacketPoint)
        {
            return GameBoard.Points[new Vector2(gamePacketPoint.X, gamePacketPoint.Y)].Point != null;
        }

        public void HandleHitResult(GamePacket gamePacket)
        {
            ClickablePointRenderer clickablePointRenderer = ClickableGameBoard.Points[new UnityEngine.Vector2(gamePacket.Point.X, gamePacket.Point.Y)];
            if (gamePacket.IsPointHit)
            {
                clickablePointRenderer.Image.color = Color.green;
                clickablePointRenderer.Button.interactable = false;
                return;
            }
            clickablePointRenderer.Image.color = Color.gray;
            clickablePointRenderer.Button.interactable = false;
        }

        public void MarkEnemyHit(GamePacket gamePacket)
        {
            EnemyShoot?.Invoke(new Vector2(gamePacket.Point.X, gamePacket.Point.Y), gamePacket.IsPointHit);
        }

        public void CheckPlayerTurn(string objPlayerId)
        {
            if(objPlayerId == _runtimeManager.Player.Id)
                ClickableGameBoard.SetInteractable(true);
            else ClickableGameBoard.SetInteractable(false);
        }

        public void SendHit(UnityEngine.Vector2 position)
        {
            _runtimeManager.NetHub.SendHitRequest(position);
        }
    }
}