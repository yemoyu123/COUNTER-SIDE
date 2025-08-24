using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Contract;

[PacketId(ClientPacketId.kNKMPacket_INSTANT_CONTRACT_LIST_ACK)]
public sealed class NKMPacket_INSTANT_CONTRACT_LIST_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<NKMInstantContract> InstantContract = new List<NKMInstantContract>();

	public int customContractUnitId;

	public NKMContractBonusState customContractBonusState = new NKMContractBonusState();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref InstantContract);
		stream.PutOrGet(ref customContractUnitId);
		stream.PutOrGet(ref customContractBonusState);
	}
}
