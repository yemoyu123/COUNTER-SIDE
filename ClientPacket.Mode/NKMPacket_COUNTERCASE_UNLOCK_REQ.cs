using Cs.Protocol;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_COUNTERCASE_UNLOCK_REQ)]
public sealed class NKMPacket_COUNTERCASE_UNLOCK_REQ : ISerializable
{
	public int dungeonID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref dungeonID);
	}
}
