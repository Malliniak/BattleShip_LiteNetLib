using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using BattleShipCoreServer.Players;
using BattleShipCoreServer.Rooms;
using BattleShipShared.Packets.Game;
using BattleShipShared.Packets.Player;
using BattleShipShared.Packets.Rooms;
using BattleShipShared.Packets.Server;
using LiteNetLib;
using LiteNetLib.Utils;
using Serilog;

namespace BattleShipCoreServer.Core
{
    public class Server : INetEventListener
    {
        private const string NETHUB_ACCESS_TOKEN = "BS79";

        private RoomsController _roomsController;
        private PlayersManager _playersManager;
        public NetManager NetManager { get; set; }
        public NetPacketProcessor Processor { get; } = new NetPacketProcessor();

        public Server()
        {
            Processor.RegisterNestedType<PlayersInRoomStruct>();
            Processor.RegisterNestedType<PointStruct>();

            Processor.SubscribeReusable<ServerPacket, NetPeer>(ServerPacketReceived);
            Processor.SubscribeReusable<RoomPacket, NetPeer>(RoomPacketReceived);
            Processor.SubscribeReusable<PlayerPacket, NetPeer>(PlayerPacketReceived);
            Processor.SubscribeReusable<RoomReadyPacket, NetPeer>(SetPlayerReadyState);
            Processor.SubscribeReusable<GamePacket, NetPeer>(GamePacketReceived);
            Processor.SubscribeReusable<TurnPacket, NetPeer>(SetPlayerTurn);
            
            _playersManager = new PlayersManager(this);
            _roomsController = new RoomsController(this);
            
            Log.Information("Server Started");
        }

        private void SetPlayerTurn(TurnPacket arg1, NetPeer arg2)
        {
            _roomsController.SetPlayerTurn(_roomsController.Rooms.Find(x => x.RoomId == arg1.RoomId));
        }

        private void GamePacketReceived(GamePacket arg1, NetPeer peer)
        {
            switch (arg1.GameCommand)
            {
                case GameCommand.SEND_HIT:
                    _roomsController.CheckPlayerHit(arg1);
                    _roomsController.SetPlayerTurn(_roomsController.Rooms.Find(x=> x.RoomId ==arg1.RoomId));
                    break;
                case GameCommand.ATTACK_RECEIVE_HIT_RESULT:
                    _roomsController.SendHitResultToPlayers(arg1);
                    break;
                case GameCommand.PLAYER_READY_TO_PLAY:
                    Log.Information("PlayerReadyToPlay");
                    _roomsController.SetPlayerReadyGameState(arg1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SetPlayerReadyState(RoomReadyPacket roomPacket, NetPeer arg2)
        {
            _roomsController.SetPlayerReadyState(roomPacket, _roomsController.Rooms.Find(x => x.RoomId == roomPacket.RoomId));
        }

        private void PlayerPacketReceived(PlayerPacket playerPacket, NetPeer peer)
        {
            Player playerSending = _playersManager.GetConnectedPlayer(peer);
            
            if (playerSending == null) return;

            switch (playerPacket.PlayerCommand)
            {
                case PlayerCommand.CHANGE_PLAYER_NAME:
                    _playersManager.ChangePlayerConfig(playerPacket.PlayerName, playerSending);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private void RoomPacketReceived(RoomPacket roomPacket, NetPeer peer)
        {
            Player playerSending = _playersManager.GetConnectedPlayer(peer);
            
            if (playerSending == null) return;

            switch (roomPacket.RoomCommand)
            {
                case RoomCommand.CREATE_ROOM:
                    _roomsController.CreateRoom(playerSending, roomPacket.RoomName);
                    break;
                case RoomCommand.CREATE_ROOM_CONFIRMATION:
                    _roomsController.UpdateRoom(_roomsController.Rooms.Find(x => x.RoomId == roomPacket.RoomId));
                    break;
                case RoomCommand.JOIN_ROOM:
                    _roomsController.AssignPlayerToRoom(playerSending, roomPacket.RoomId);
                    break;
                case RoomCommand.JOIN_ROOM_CONFIRM:
                    break;
                case RoomCommand.ROOM_UPDATE:
                    break;
                case RoomCommand.LOAD_GAME_SCENE_CONFIRM:
                    _roomsController.SetPlayerLoaded(playerSending, roomPacket.RoomId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ServerPacketReceived(ServerPacket serverPacket, NetPeer peer)
        {            
            Player playerSending = _playersManager.GetConnectedPlayer(peer);

            if (playerSending == null) return;

            switch (serverPacket.ServerCommand)
            {
                case ServerCommand.SIMPLE_MESSAGE:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void OnPeerConnected(NetPeer peer)
        {
            Log.Information("Peer connected: {Peer}", peer.EndPoint);
            List<NetPeer> peers = NetManager.ConnectedPeerList;
            
            foreach  (NetPeer netPeer in peers)
            {
                Log.Information("ConnectedPeersList: id={Id}, ep={Peer}", netPeer.Id, netPeer.EndPoint);
            }
            
            _playersManager.RegisterPlayer(peer);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Log.Information("Client {Id} disconnected: {Reason}", peer.Id, disconnectInfo.Reason);
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            Log.Information("Error: {Error}", socketError);
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            Processor.ReadAllPackets(reader, peer);
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            string message = reader.GetString(100);
            
            Log.Information("Received unconnected message {MessageType} from {RemoteEndPoint}: {Data}",
                messageType,
                remoteEndPoint,
                message);
            
            NetDataWriter writer = new NetDataWriter();

            writer.Put(messageType == UnconnectedMessageType.DiscoveryRequest ? "ACK" : "NACK");

            NetManager.SendDiscoveryResponse(writer, remoteEndPoint);
            reader.Recycle();
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            if (request.AcceptIfKey(NETHUB_ACCESS_TOKEN) == null)
            {
                Log.Information("Connection denied!");
            }
        }
    }
}