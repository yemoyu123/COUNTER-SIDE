using Cs.Protocol;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_CONTRACT_PERMANENTLY_REQ)]
public sealed class NKMPacket_CONTRACT_PERMANENTLY_REQ : ISerializable
{
	public long unitUID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitUID);
	}
}
