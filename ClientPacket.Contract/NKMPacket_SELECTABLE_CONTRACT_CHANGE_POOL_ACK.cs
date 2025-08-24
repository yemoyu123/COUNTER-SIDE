using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Contract;

[PacketId(ClientPacketId.kNKMPacket_SELECTABLE_CONTRACT_CHANGE_POOL_ACK)]
public sealed class NKMPacket_SELECTABLE_CONTRACT_CHANGE_POOL_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMSelectableContractState selectableContractState = new NKMSelectableContractState();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref selectableContractState);
	}
}
