using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_ACCOUNT_BIRTHDAY_REWARD_NOT)]
public sealed class NKMPacket_ACCOUNT_BIRTHDAY_REWARD_NOT : ISerializable
{
	public NKMUserBirthDayData birthDayData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref birthDayData);
	}
}
