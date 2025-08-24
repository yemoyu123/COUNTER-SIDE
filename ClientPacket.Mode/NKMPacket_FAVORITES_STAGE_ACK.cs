using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_FAVORITES_STAGE_ACK)]
public sealed class NKMPacket_FAVORITES_STAGE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public Dictionary<int, int> favoritesStage = new Dictionary<int, int>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref favoritesStage);
	}
}
