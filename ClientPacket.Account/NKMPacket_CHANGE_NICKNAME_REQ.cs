using Cs.Protocol;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_CHANGE_NICKNAME_REQ)]
public sealed class NKMPacket_CHANGE_NICKNAME_REQ : ISerializable
{
	public string nickname;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref nickname);
	}
}
