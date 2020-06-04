using System;
using BattleShip.Core;
using BattleShipShared.Packets.Game;
using BattleShipShared.Packets.Player;
using BattleShipShared.Packets.Rooms;
using BattleShipShared.Packets.Server;
using LiteNetLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vector2 = System.Numerics.Vector2;

namespace BattleShip.Net
{
    public class NetHub
    {
        private const string NETHUB_ACCESS_TOKEN = "BS79";

        private NetClient NetClient { get; }
        private readonly NetManager _connectionManager;
        private readonly RuntimeManager _runtimeManager;

        internal Action<ServerPacket> ServerCommandReceived;
        internal Action<RoomPacket> RoomCommandReceived;
        internal Action<PlayerPacket> PlayerCommandReceived;
        internal Action<RoomReadyPacket> RoomReadyReceived;
        internal Action<GamePacket> GamePacketReceived;
        internal Action<TurnPacket> TurnPacketReceived;
        
        public NetHub(RuntimeManager runtimeManager)
        {
            NetClient = new NetClient();
            _connectionManager = new NetManager(NetClient) { UnconnectedMessagesEnabled = true };
            _runtimeManager = runtimeManager;
            
            NetClient.NetHub = this;
            NetClient.Manager = _connectionManager;
            
            _connectionManager.Start();
            _connectionManager.Connect("localhost", 3000, NETHUB_ACCESS_TOKEN);

            ServerCommandReceived += OnServerCommandReceived;
            RoomCommandReceived += OnRoomCommandReceived;
            PlayerCommandReceived += OnPlayerCommandReceived;
            RoomReadyReceived += OnRoomReadyReceived;
            GamePacketReceived += OnGamePacketReceived;
            TurnPacketReceived += OnTurnPacketReceived;
        }

        private void OnTurnPacketReceived(TurnPacket obj)
        {
            Debug.Log("Turn packet Received");
            _runtimeManager.GameplayManager.CheckPlayerTurn(obj.PlayerId);
        }

        private void OnGamePacketReceived(GamePacket obj)
        {
            Debug.Log($"Game Command: {obj.GameCommand} Received");

            switch (obj.GameCommand)
            {
                case GameCommand.SEND_HIT:
                    _runtimeManager.CheckEnemysHit(obj);
                    break;
                case GameCommand.ATTACK_RECEIVE_HIT_RESULT:
                    _runtimeManager.HandleReceivedHit(obj);
                    break;
                case GameCommand.PLAYER_READY_TO_PLAY:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        ~NetHub()
        {
            Debug.Log("Client Stop");
            _connectionManager.DisconnectAll();
            _connectionManager.Stop();
        }
        
        public void PollEvents()
        {
            _connectionManager.PollEvents();
        }

        private void OnServerCommandReceived(ServerPacket serverPacket)
        {
            Debug.Log($"Server Command: {serverPacket.ServerCommand} Received");
                
            switch (serverPacket.ServerCommand)
            {
                case ServerCommand.SIMPLE_MESSAGE:
                    Debug.Log($"Server Message: {serverPacket.Value}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        
        private void OnPlayerCommandReceived(PlayerPacket playerPacket)
        {
            Debug.Log($"Player Command: {playerPacket.PlayerCommand} Received");

            switch (playerPacket.PlayerCommand)
            {
                case PlayerCommand.CHANGE_PLAYER_NAME:
                    _runtimeManager.Player.ChangePlayerName(playerPacket.PlayerName, playerPacket.PlayerId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnRoomCommandReceived(RoomPacket roomPacket)
        {
            Debug.Log($"Room Command: {roomPacket.RoomCommand} Received");
            switch (roomPacket.RoomCommand)
            {
                case RoomCommand.CREATE_ROOM:
                    _runtimeManager.CreateRoom(roomPacket.RoomId);
                    _runtimeManager.DisplayRoom();
                    break;
                case RoomCommand.CREATE_ROOM_CONFIRMATION:
                    break;
                case RoomCommand.JOIN_ROOM:
                    JoinRoomRequest(roomPacket.RoomId);
                    break;
                case RoomCommand.JOIN_ROOM_CONFIRM:
                    _runtimeManager.CreateRoom(roomPacket.RoomId);
                    _runtimeManager.DisplayRoom();
                    break;
                case RoomCommand.ROOM_UPDATE:
                    _runtimeManager.FetchRoom(roomPacket.RoomName, roomPacket.RoomId, roomPacket.PlayersInRoomStruct.PlayersArray);
                    break;
                case RoomCommand.LOAD_GAME_SCENE:
                    _runtimeManager.LoadGameScene(roomPacket.RoomName, roomPacket.RoomId, roomPacket.PlayersInRoomStruct.PlayersArray);
                    break;
                case RoomCommand.LOAD_GAME_SCENE_CONFIRM:
                    break;
                default:
                    Debug.LogError($"UNHANDLED COMMAND: {roomPacket.RoomCommand}");
                    break;
            }
        }

        private void OnRoomReadyReceived(RoomReadyPacket roomReadyPacket)
        {
            _runtimeManager.Room.UpdatePlayersReady(roomReadyPacket.PlayerId, roomReadyPacket.ReadyState);
        }
        

        public void NameChangeRequest(string name)
        {
            NetClient.Send(new PlayerPacket
            {
                PlayerCommand = PlayerCommand.CHANGE_PLAYER_NAME,
                PlayerName =  name,
                PlayerId = _runtimeManager.Player.Id
            });
        }

        public void CreateRoomRequest()
        {
            NetClient.Send(new RoomPacket
            {
                RoomCommand = RoomCommand.CREATE_ROOM,
                RoomName = "TestRoom",
            });
        }

        public void JoinRoomRequest(string id)
        {
            NetClient.Send(new RoomPacket
            {
                RoomCommand = RoomCommand.JOIN_ROOM,
                RoomId = id
            });
        }

        public void SendRoomCreateConfirmation(string roomId)
        {
            NetClient.Send(new RoomPacket
            {
                RoomCommand = RoomCommand.CREATE_ROOM_CONFIRMATION,
                RoomId = roomId
            });
        }

        public void PlayerRoomStateRequest(string playerId, string roomId, bool readyState)
        {
            if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                NetClient.Send(new RoomReadyPacket
                {
                    PlayerId = playerId,
                    ReadyState = readyState,
                    RoomId = roomId
                });
            }

            if (SceneManager.GetActiveScene().buildIndex == 2)
            {
                NetClient.Send(new GamePacket
                {
                    GameCommand = GameCommand.PLAYER_READY_TO_PLAY,
                    PlayerId = _runtimeManager.Player.Id,
                    RoomId = roomId
                });
            }

        }

        public void SendCheckedHit(bool hitResult, string atacker, PointStruct pointStruct)
        {
            Debug.Log(hitResult);
            NetClient.Send(new GamePacket
            {
                GameCommand = GameCommand.ATTACK_RECEIVE_HIT_RESULT,
                IsPointHit = hitResult,
                PlayerId = atacker,
                RoomId = _runtimeManager.Room.RoomId,
                Point = pointStruct
            });
        }

        public void SendLoadGameSceneConfirmation()
        {
            NetClient.Send(new RoomPacket
            {
                RoomCommand = RoomCommand.LOAD_GAME_SCENE_CONFIRM,
                RoomId = _runtimeManager.Room.RoomId,
            });
        }

        public void SendHitRequest(UnityEngine.Vector2 position)
        {
            NetClient.Send(new GamePacket
            {
                GameCommand = GameCommand.SEND_HIT,
                PlayerId = _runtimeManager.Player.Id,
                Point = new PointStruct
                {
                    X = (int)position.x,
                    Y = (int)position.y
                },
                RoomId = _runtimeManager.Room.RoomId
            });
        }
    }
}
