using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_TOURNAMENT_APPLY_ACK)]
public sealed class NKMPacket_TOURNAMENT_APPLY_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMAsyncDeckData deck = new NKMAsyncDeckData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref deck);
	}
}
