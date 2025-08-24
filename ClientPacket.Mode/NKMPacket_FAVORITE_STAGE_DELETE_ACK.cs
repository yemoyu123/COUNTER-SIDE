using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_FAVORITE_STAGE_DELETE_ACK)]
public sealed class NKMPacket_FAVORITE_STAGE_DELETE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public Dictionary<int, int> favoritesStage = new Dictionary<int, int>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref favoritesStage);
	}
}
