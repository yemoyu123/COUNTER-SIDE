using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Contract;

[PacketId(ClientPacketId.kNKMPacket_SELECTABLE_CONTRACT_CONFIRM_ACK)]
public sealed class NKMPacket_SELECTABLE_CONTRACT_CONFIRM_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int contractId;

	public List<NKMItemMiscData> costItems = new List<NKMItemMiscData>();

	public List<NKMUnitData> units = new List<NKMUnitData>();

	public NKMSelectableContractState selectableContractState = new NKMSelectableContractState();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref contractId);
		stream.PutOrGet(ref costItems);
		stream.PutOrGet(ref units);
		stream.PutOrGet(ref selectableContractState);
	}
}
