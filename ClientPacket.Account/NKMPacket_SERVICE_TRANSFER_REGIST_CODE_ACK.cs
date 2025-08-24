using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_SERVICE_TRANSFER_REGIST_CODE_ACK)]
public sealed class NKMPacket_SERVICE_TRANSFER_REGIST_CODE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public string code;

	public bool canReceiveReward;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref code);
		stream.PutOrGet(ref canReceiveReward);
	}
}
