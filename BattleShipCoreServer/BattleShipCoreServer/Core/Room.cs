using System;
using System.Collections.Generic;
using Serilog;

namespace BattleShipCoreServer.Core
{
    public class Room
    {
        public List<Player> Players { get; } = new List<Player>();
        public string RoomName { get; }
        public string RoomId { get; }

        private int _currentPlayerTurn;
        public int CurrentPlayerTurnId
        {
            get => _currentPlayerTurn;
            set => _currentPlayerTurn = value > Players.Count - 1 ? 0 : value;
        }

        public Room(string roomName)
        {
            RoomName = roomName;
            RoomId = GenerateRandomRoomId();
            Log.Information("Created room named {roomName}, with unique code: {code}", RoomName, RoomId );
        }

        public string[] GetPlayerNames()
        {
            string[] array = new string[Players.Count];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = Players[i].Name + "|" + Players[i].Id;
            }
            return array;
        } 
        
        private string GenerateRandomRoomId()
        {
            Random random = new Random();
            string code = null;
            for (int i = 0; i < 3; i++)
            {
                code += $"{random.Next(0, 9)}{(char)random.Next(65,90)}";
            }

            return code;
        }
    }
}