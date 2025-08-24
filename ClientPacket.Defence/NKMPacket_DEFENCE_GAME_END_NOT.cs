using ClientPacket.Common;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Defence;

[PacketId(ClientPacketId.kNKMPacket_DEFENCE_GAME_END_NOT)]
public sealed class NKMPacket_DEFENCE_GAME_END_NOT : ISerializable
{
	public NKMGameEndData gameEndData = new NKMGameEndData();

	public NKMDefenceClearData defenceClearData = new NKMDefenceClearData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref gameEndData);
		stream.PutOrGet(ref defenceClearData);
	}
}
