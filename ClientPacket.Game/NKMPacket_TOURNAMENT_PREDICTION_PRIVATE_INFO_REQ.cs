using Cs.Protocol;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_TOURNAMENT_PREDICTION_PRIVATE_INFO_REQ)]
public sealed class NKMPacket_TOURNAMENT_PREDICTION_PRIVATE_INFO_REQ : ISerializable
{
	public int templetId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref templetId);
	}
}
