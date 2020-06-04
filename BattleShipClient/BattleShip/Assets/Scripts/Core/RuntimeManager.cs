using BattleShip.Core.Players;
using BattleShip.Game;
using UnityEngine;
using BattleShip.Net;
using BattleShipShared.Packets.Game;
using UnityEngine.SceneManagement;

namespace BattleShip.Core
{
    public class RuntimeManager : MonoBehaviour
    {
        private Room _room;
        private LocalPlayer _player;
        private NetHub _netHub;

        public GameplayManager GameplayManager { get; set; }
        
        public NetHub NetHub
        {
            get => _netHub;
            private set => _netHub = value;
        }

        public LocalPlayer Player
        {
            get => _player;
            private set => _player = value;
        }

        public Room Room
        {
            get => _room;
            private set => _room = value;
        }

        private void Awake()
        {
            DontDestroyOnLoad(this);
            
            NetHub = new NetHub(this);
            Player = new LocalPlayer();
        }

        private void Update()
        {
            NetHub.PollEvents();
        }

        public void CreateRoom(string roomId)
        {
            _room = new Room(roomId, this);
        }

        public void DisplayRoom()
        {
            SceneManager.LoadScene(1);
            NetHub.SendRoomCreateConfirmation(Room.RoomId);
        }

        public void FetchRoom(string roomName, string roomId, string[] players)
        {
            Room.UpdateRoom(roomName, roomId, players);
        }

        public void LoadGameScene(string roomName, string roomId, string[] players)
        {
            if (SceneManager.GetActiveScene().buildIndex != 2)
            {
                SceneManager.LoadScene(2);
                NetHub.SendLoadGameSceneConfirmation();
            }
        }

        public void CheckEnemysHit(GamePacket gamePacket)
        {
            if (gamePacket.PlayerId == _player.Id)
                return;

            bool hitResult = GameplayManager.CheckHit(gamePacket.Point);

            NetHub.SendCheckedHit(hitResult, gamePacket.PlayerId, gamePacket.Point);
        }

        public void HandleReceivedHit(GamePacket gamePacket)
        {
            if (gamePacket.PlayerId == _player.Id)
            {
                GameplayManager.HandleHitResult(gamePacket);
                return;
            }

            GameplayManager.MarkEnemyHit(gamePacket);
        }
    }
}
