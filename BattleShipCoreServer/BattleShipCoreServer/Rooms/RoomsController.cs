using System;
using System.Collections.Generic;
using BattleShipCoreServer.Core;
using BattleShipShared.Packets.Game;
using BattleShipShared.Packets.Player;
using BattleShipShared.Packets.Rooms;
using LiteNetLib;
using Serilog;

namespace BattleShipCoreServer.Rooms
{
    public class RoomsController
    {
        public HashSet<Room> Rooms { get; } = new HashSet<Room>();

        private readonly Server _server;

        public RoomsController(Server server)
        {
            _server = server;
        }

        internal void AssignPlayerToRoom(Player playerSending, string roomId)
        {
            Room roomToJoin = Rooms.Find(x => x.RoomId == roomId);
            roomToJoin.Players.Add(playerSending);
            Log.Information("Assigning player {player} to room {roomName}", 
                playerSending.Name, 
                roomToJoin.RoomName
                );
            playerSending._Peer.Send(_server.Processor.Write(new RoomPacket
                {
                    RoomCommand = RoomCommand.JOIN_ROOM_CONFIRM,
                    RoomId = roomToJoin.RoomId,
                    RoomName = roomToJoin.RoomName,
                    PlayersInRoomStruct = new PlayersInRoomStruct
                    {
                        PlayersArray = roomToJoin.GetPlayerNames()
                    }
                }),
                DeliveryMethod.ReliableOrdered);
            UpdateRoom(roomToJoin);
        }

        internal void UpdateRoom(Room room)
        {
            PlayersInRoomStruct playersInRoomStruct = new PlayersInRoomStruct {PlayersArray = room.GetPlayerNames()};
            foreach (Player variable in room.Players)
            {
                variable._Peer.Send(_server.Processor.Write(new RoomPacket
                    {
                        RoomCommand = RoomCommand.ROOM_UPDATE,
                        RoomId = room.RoomId,
                        RoomName = room.RoomName,
                        PlayersInRoomStruct = playersInRoomStruct
                    }),
                    DeliveryMethod.ReliableOrdered);
            }
        }

        internal void CreateRoom(Player playerCreating, string roomName)
        {
            Room newRoom = new Room(roomName);
            Rooms.Add(newRoom);
            newRoom.Players.Add(playerCreating);
            playerCreating._Peer.Send(_server.Processor.Write(new RoomPacket
                {
                    RoomCommand = RoomCommand.CREATE_ROOM,
                    RoomId = newRoom.RoomId,
                    RoomName = newRoom.RoomName,
                    PlayersInRoomStruct = new PlayersInRoomStruct
                    {
                        PlayersArray = newRoom.GetPlayerNames()
                    }
                }),
                DeliveryMethod.ReliableOrdered);
        }

        public void SetPlayerReadyState(RoomReadyPacket roomPacket, Room find)
        {
            Player playerToChange = find.Players.Find(x => x.Id == roomPacket.PlayerId);
            if (playerToChange == null)
                return;
            playerToChange.IsReady = !playerToChange.IsReady;
            foreach (Player player in find.Players)
            {
                foreach (Player playerUpdate in find.Players)
                {
                    player._Peer.Send(_server.Processor.Write(new RoomReadyPacket
                    {
                        PlayerId =  playerUpdate.Id,
                        ReadyState = playerUpdate.IsReady,
                        RoomId = find.RoomId
                    }), DeliveryMethod.ReliableOrdered);
                }
            }

            CheckIfPlayersReady(find);
        }

        private void CheckIfPlayersReady(Room room)
        {
            foreach (Player player in room.Players)
            {
                if(!player.IsReady)
                    return;
            }

            foreach (Player player in room.Players)
            {
                player._Peer.Send(_server.Processor.Write(new RoomPacket
                {
                    RoomCommand = RoomCommand.LOAD_GAME_SCENE,
                    RoomId = room.RoomId,
                    RoomName = room.RoomName,
                    PlayersInRoomStruct = new PlayersInRoomStruct
                    {
                        PlayersArray = room.GetPlayerNames()
                    }
                }), DeliveryMethod.ReliableOrdered);
            }
        }

        public void CheckPlayerHit(GamePacket gamePacket)
        {
            Room room = Rooms.Find(x => x.RoomId == gamePacket.RoomId);
            Player playerSending = room.Players.Find(x => x.Id == gamePacket.PlayerId);

            if (playerSending == null)
                return;
            foreach (Player VARIABLE in room.Players)
            {
                if(VARIABLE == playerSending)
                    continue;
                VARIABLE._Peer.Send(_server.Processor.Write(new GamePacket
                {
                    GameCommand = GameCommand.SEND_HIT,
                    PlayerId = playerSending.Id,
                    RoomId = room.RoomId,
                    Point =  gamePacket.Point
                }), DeliveryMethod.ReliableOrdered);
            }
        }

        public void SendHitResultToPlayers(GamePacket gamePacket)
        {
            Room room = Rooms.Find(x => x.RoomId == gamePacket.RoomId);

            foreach (Player player in room.Players)
            {
                player._Peer.Send(_server.Processor.Write(new GamePacket
                {
                    GameCommand = GameCommand.ATTACK_RECEIVE_HIT_RESULT,
                    PlayerId = gamePacket.PlayerId,
                    Point =  gamePacket.Point,
                    IsPointHit = gamePacket.IsPointHit,
                    RoomId = gamePacket.RoomId
                }), DeliveryMethod.ReliableOrdered);
            }
        }

        public void SetupGame(string roomId)
        {
            Room room = Rooms.Find(x => x.RoomId == roomId);
            room.CurrentPlayerTurnId = new Random().Next(0, room.Players.Count-1);
            SetPlayerTurn(room);
        }

        public void SetPlayerTurn(Room room)
        {

            Log.Information("SetPlayerTurn");
            room.CurrentPlayerTurnId++;

            foreach (var VARIABLE in room.Players)
            {
                VARIABLE._Peer.Send(_server.Processor.Write(new TurnPacket
                {
                    PlayerId = room.Players[room.CurrentPlayerTurnId].Id,
                    RoomId = room.RoomId
                }), DeliveryMethod.ReliableOrdered);
            }
        }

        public void SetPlayerLoaded(Player playerSending, string roomPacketRoomId)
        {
            Room room = Rooms.Find(x => x.RoomId == roomPacketRoomId);
            Player player = room.Players.Find(x => x == playerSending);

            if (player == null)
                return;
            
            player.IsLoadedToScene = true;
        }

        public void SetPlayerReadyGameState(GamePacket gamePacket)
        {
            var room = Rooms.Find(x => x.RoomId == gamePacket.RoomId);
            var player = room.Players.Find(x => x.Id == gamePacket.PlayerId);
            player.IsReady = !player.IsReady;

            player._Peer.Send(_server.Processor.Write(new GamePacket
            {
                GameCommand = GameCommand.PLAYER_READY_TO_PLAY,
            }), DeliveryMethod.ReliableOrdered);

            bool roomReady = true;

            foreach (var VARIABLE in room.Players)
            {
                if (VARIABLE.IsReady == false)
                    roomReady = false;
            }
            
            if(roomReady)
                SetupGame(room.RoomId);
        }
    }
}