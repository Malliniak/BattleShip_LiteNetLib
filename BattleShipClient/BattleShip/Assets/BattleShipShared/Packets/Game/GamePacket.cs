namespace BattleShipShared.Packets.Game
{
    public class GamePacket
    {
        public GameCommand GameCommand { get; set; }
        public string RoomId { get; set; }
        public string PlayerId { get; set; }
        public PointStruct Point { get; set; }
        public bool IsPointHit { get; set; }
    }
}