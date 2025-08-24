using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Contract;

[PacketId(ClientPacketId.kNKMPacket_CUSTOM_PICKUP_ACK)]
public sealed class NKMPacket_CUSTOM_PICKUP_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public ContractCostType costType;

	public List<NKMItemMiscData> costItems = new List<NKMItemMiscData>();

	public List<NKMUnitData> units = new List<NKMUnitData>();

	public List<NKMOperator> operators = new List<NKMOperator>();

	public NKMRewardData rewardData;

	public NKMCustomPickupContract customPickupContractData = new NKMCustomPickupContract();

	public int customPickupId;

	public int requestCount;

	public NKMContractBonusState contractBonusState = new NKMContractBonusState();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGetEnum(ref costType);
		stream.PutOrGet(ref costItems);
		stream.PutOrGet(ref units);
		stream.PutOrGet(ref operators);
		stream.PutOrGet(ref rewardData);
		stream.PutOrGet(ref customPickupContractData);
		stream.PutOrGet(ref customPickupId);
		stream.PutOrGet(ref requestCount);
		stream.PutOrGet(ref contractBonusState);
	}
}
