using Cs.Protocol;
using Protocol;

namespace ClientPacket.Contract;

[PacketId(ClientPacketId.kNKMPacket_MISC_CONTRACT_OPEN_REQ)]
public sealed class NKMPacket_MISC_CONTRACT_OPEN_REQ : ISerializable
{
	public int miscItemId;

	public int count;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref miscItemId);
		stream.PutOrGet(ref count);
	}
}
