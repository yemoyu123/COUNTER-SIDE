using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Contract;

[PacketId(ClientPacketId.kNKMPacket_CONTRACT_ACK)]
public sealed class NKMPacket_CONTRACT_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public ContractCostType costType;

	public List<NKMItemMiscData> costItems = new List<NKMItemMiscData>();

	public List<NKMUnitData> units = new List<NKMUnitData>();

	public List<NKMOperator> operators = new List<NKMOperator>();

	public NKMRewardData rewardData;

	public NKMContractState contractState = new NKMContractState();

	public NKMContractBonusState contractBonusState = new NKMContractBonusState();

	public int contractId;

	public int requestCount;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGetEnum(ref costType);
		stream.PutOrGet(ref costItems);
		stream.PutOrGet(ref units);
		stream.PutOrGet(ref operators);
		stream.PutOrGet(ref rewardData);
		stream.PutOrGet(ref contractState);
		stream.PutOrGet(ref contractBonusState);
		stream.PutOrGet(ref contractId);
		stream.PutOrGet(ref requestCount);
	}
}
