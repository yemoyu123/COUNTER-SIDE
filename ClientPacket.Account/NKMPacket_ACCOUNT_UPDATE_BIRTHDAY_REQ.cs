using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_ACCOUNT_UPDATE_BIRTHDAY_REQ)]
public sealed class NKMPacket_ACCOUNT_UPDATE_BIRTHDAY_REQ : ISerializable
{
	public BirthDayDate birthDay;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref birthDay);
	}
}
