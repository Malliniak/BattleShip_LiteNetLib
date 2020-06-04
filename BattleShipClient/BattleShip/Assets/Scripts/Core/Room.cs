using System;
using System.Collections.Generic;
using BattleShip.Core.Players;
using UnityEngine;

namespace BattleShip.Core
{
    [Serializable]
    public class Room
    {
        private string _roomId;
        private string _roomName;
        private RuntimeManager _runtimeManager;
        private List<Player> _players = new List<Player>();

        public event Action RoomUpdated = delegate { Debug.Log("Room Updated"); };
        public event Action<Player> RoomReadyUpdated = delegate (Player x) { Debug.Log($"Ready State of {x.Name} Updated tp {x.IsReady}"); };

        public string RoomId
        {
            get => _roomId;
            private set => _roomId = value;
        }

        public string RoomName
        {
            get => _roomName;
            private set => _roomName = value;
        }

        public List<Player> Players
        {
            get => _players;
        }

        public int PlayerCount
        {
            get => _players.Count;
        }

        public Room(string roomId, RuntimeManager runtimeManager)
        {
            RoomId = roomId;
            _runtimeManager = runtimeManager;
        }

        public void UpdateRoom(string roomName, string roomId, string[] players)
        {
            Debug.Log("Room update");
            RoomId = roomId;
            RoomName = roomName;
            Debug.Log($"players in array: {players[0]}, player count {players.Length}");
            for (int i = 0; i<players.Length; i++)
            {
                string[] player = players[i].Split('|');
                Player found = _players.Find(x => x.Id == player[1]);
                if (found == null)
                {
                    Debug.Log("adding player xDDDD");
                    _players.Add(new RemotePlayer(player[0], player[1]));
                    continue;
                }
                Debug.Log($"Found player with id: {found.Id}");
            }

            RoomUpdated?.Invoke();
        }

        public void UpdatePlayersReady(string playerId, bool state)
        {
            Player playerToChange = _players.Find(x => x.Id == playerId);
            if (playerToChange == null)
                return;
            playerToChange.IsReady = state;
            RoomReadyUpdated?.Invoke(playerToChange);
        }
    }
}