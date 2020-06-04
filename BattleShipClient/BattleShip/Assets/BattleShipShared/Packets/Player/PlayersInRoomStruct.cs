using LiteNetLib.Utils;

namespace BattleShipShared.Packets.Player
{
    public struct PlayersInRoomStruct: INetSerializable
    {  
        public string[] PlayersArray { get; set; }
        
        public void Serialize(NetDataWriter writer)
        {
            writer.PutArray(PlayersArray);
        }

        public void Deserialize(NetDataReader reader)
        {
            PlayersArray = reader.GetStringArray();
        }
    }
}