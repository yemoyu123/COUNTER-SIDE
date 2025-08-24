using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_CHANGE_NICKNAME_ACK)]
public sealed class NKMPacket_CHANGE_NICKNAME_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public string nickname;

	public NKMItemMiscData costItemData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref nickname);
		stream.PutOrGet(ref costItemData);
	}
}
