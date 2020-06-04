using System;
using System.Net;
using System.Net.Sockets;
using BattleShipShared.Packets.Game;
using BattleShipShared.Packets.Player;
using BattleShipShared.Packets.Rooms;
using BattleShipShared.Packets.Server;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace BattleShip.Net
{
    internal class NetClient: INetEventListener
    {
        public NetManager Manager { get; set; }
        public NetHub NetHub { get; set; }
        private readonly NetPacketProcessor _processor;

        public NetClient()
        {
            _processor = new NetPacketProcessor();

            _processor.RegisterNestedType<PlayersInRoomStruct>();
            _processor.RegisterNestedType<PointStruct>();
            
            _processor.SubscribeReusable<ServerPacket, NetPeer>(HandleServerCommand);
            _processor.SubscribeReusable<RoomPacket, NetPeer>(HandleRoomCommand);
            _processor.SubscribeReusable<PlayerPacket, NetPeer>(HandlePlayerCommand);
            _processor.SubscribeReusable<RoomReadyPacket, NetPeer>(HandleRoomReadyPacket);
            _processor.SubscribeReusable<GamePacket, NetPeer>(HandleGamePacket);
            _processor.SubscribeReusable<TurnPacket, NetPeer>(HandleTurn);
        }

        private void HandleTurn(TurnPacket arg1, NetPeer arg2)
        {
            NetHub.TurnPacketReceived.Invoke(arg1);
        }

        private void HandleGamePacket(GamePacket arg1, NetPeer arg2)
        {
            NetHub.GamePacketReceived.Invoke(arg1);
        }

        private void HandleRoomReadyPacket(RoomReadyPacket roomReadyPacket, NetPeer player)
        {
            NetHub.RoomReadyReceived.Invoke(roomReadyPacket);
        }

        private void HandlePlayerCommand(PlayerPacket arg1, NetPeer arg2)
        {
            NetHub.PlayerCommandReceived.Invoke(arg1);
        }

        private void HandleRoomCommand(RoomPacket roomPacket, NetPeer sender)
        {
            NetHub.RoomCommandReceived.Invoke(roomPacket);
        }

        private void HandleServerCommand(ServerPacket serverPacket, NetPeer sender)
        {
            NetHub.ServerCommandReceived.Invoke(serverPacket);
        }

        public void OnPeerConnected(NetPeer peer)
        {
            Debug.Log($"Client {Manager} connected to: {peer.EndPoint.Address}:{peer.EndPoint.Port}");
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Debug.Log($"Disconnected: {disconnectInfo.Reason}");
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            Debug.LogError($"Network Socket Error: {socketError}");
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            Debug.Log(reader);
            _processor.ReadAllPackets(reader, peer);
            reader.Recycle();
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            string message = reader.GetString();
            Debug.Log($"Unconnected Message Received: {message} from {remoteEndPoint.Address} | {messageType}");
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            Debug.Log("Connection Latency Update");
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            Debug.Log("Connection Rejected");
        }
    
        public void Send<T>(T payload) where T : class, new()
        {
            Manager.FirstPeer.Send(_processor.Write(payload), DeliveryMethod.Unreliable);
        }
    }
}