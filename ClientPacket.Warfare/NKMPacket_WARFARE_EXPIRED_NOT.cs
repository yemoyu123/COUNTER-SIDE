using Cs.Protocol;
using Protocol;

namespace ClientPacket.Warfare;

[PacketId(ClientPacketId.kNKMPacket_WARFARE_EXPIRED_NOT)]
public sealed class NKMPacket_WARFARE_EXPIRED_NOT : ISerializable
{
	public int stageId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref stageId);
	}
}
