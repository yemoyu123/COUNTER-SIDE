using Cs.Protocol;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_SERVICE_TRANSFER_CODE_VALIDATION_REQ)]
public sealed class NKMPacket_SERVICE_TRANSFER_CODE_VALIDATION_REQ : ISerializable
{
	public string code;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref code);
	}
}
