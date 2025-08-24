using Cs.Protocol;
using Protocol;

namespace ClientPacket.Defence;

[PacketId(ClientPacketId.kNKMPacket_DEFENCE_INFO_REQ)]
public sealed class NKMPacket_DEFENCE_INFO_REQ : ISerializable
{
	public int defenceTempletId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref defenceTempletId);
	}
}
