using Cs.Protocol;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_NEXON_PC_DATA_NOT)]
public sealed class NKMPacket_NEXON_PC_DATA_NOT : ISerializable
{
	public string npacode;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref npacode);
	}
}
