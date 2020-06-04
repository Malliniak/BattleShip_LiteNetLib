using BattleShipShared.Packets.Player;

namespace BattleShipShared.Packets.Rooms
{
    public class RoomPacket
    {
        public RoomCommand RoomCommand { get; set; }
        public string RoomName { get; set; }
        public string RoomId { get; set; }
        public PlayersInRoomStruct PlayersInRoomStruct { get; set; }
    }
}