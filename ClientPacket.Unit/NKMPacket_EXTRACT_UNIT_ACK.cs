using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_EXTRACT_UNIT_ACK)]
public sealed class NKMPacket_EXTRACT_UNIT_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<long> extractUnitUidList = new List<long>();

	public NKMRewardData rewardItems;

	public NKMRewardData synergyItems;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref extractUnitUidList);
		stream.PutOrGet(ref rewardItems);
		stream.PutOrGet(ref synergyItems);
	}
}
