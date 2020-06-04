namespace BattleShipShared.Packets.Rooms
{
    public class RoomReadyPacket
    {
        public string RoomId { get; set; }
        public string PlayerId { get; set; }
        public bool ReadyState { get; set; }
    }
}