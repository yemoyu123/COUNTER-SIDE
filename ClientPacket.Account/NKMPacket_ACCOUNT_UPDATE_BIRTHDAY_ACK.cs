using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_ACCOUNT_UPDATE_BIRTHDAY_ACK)]
public sealed class NKMPacket_ACCOUNT_UPDATE_BIRTHDAY_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public BirthDayDate birthDay;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref birthDay);
	}
}
