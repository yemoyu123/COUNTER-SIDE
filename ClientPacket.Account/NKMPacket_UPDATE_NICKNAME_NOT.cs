using Cs.Protocol;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_UPDATE_NICKNAME_NOT)]
public sealed class NKMPacket_UPDATE_NICKNAME_NOT : ISerializable
{
	public string nickname;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref nickname);
	}
}
