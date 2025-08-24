using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_FAVORITES_STAGE_UPDATE_REQ)]
public sealed class NKMPacket_FAVORITES_STAGE_UPDATE_REQ : ISerializable
{
	public Dictionary<int, int> favoritesStage = new Dictionary<int, int>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref favoritesStage);
	}
}
