using Cs.Protocol;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_NEXON_NGS_DATA_NOT)]
public sealed class NKMPacket_NEXON_NGS_DATA_NOT : ISerializable
{
	public byte[] buffer;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref buffer);
	}
}
