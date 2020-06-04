namespace BattleShipShared.Packets.Game
{
    public class TurnPacket
    {
        public string PlayerId { get; set; }
        public string RoomId { get; set; }
    }
}