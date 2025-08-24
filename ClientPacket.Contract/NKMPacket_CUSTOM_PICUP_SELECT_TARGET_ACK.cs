using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Contract;

[PacketId(ClientPacketId.kNKMPacket_CUSTOM_PICUP_SELECT_TARGET_ACK)]
public sealed class NKMPacket_CUSTOM_PICUP_SELECT_TARGET_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMCustomPickupContract customPickupContractData = new NKMCustomPickupContract();

	public NKMContractBonusState contractBonusState = new NKMContractBonusState();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref customPickupContractData);
		stream.PutOrGet(ref contractBonusState);
	}
}
