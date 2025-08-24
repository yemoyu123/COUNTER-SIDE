using Cs.Protocol;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_TRIM_RESTORE_REQ)]
public sealed class NKMPacket_TRIM_RESTORE_REQ : ISerializable
{
	public int trimIntervalId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref trimIntervalId);
	}
}
