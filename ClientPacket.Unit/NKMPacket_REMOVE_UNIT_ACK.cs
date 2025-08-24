using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_REMOVE_UNIT_ACK)]
public sealed class NKMPacket_REMOVE_UNIT_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<long> removeUnitUIDList = new List<long>();

	public List<NKMItemMiscData> rewardItemDataList = new List<NKMItemMiscData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref removeUnitUIDList);
		stream.PutOrGet(ref rewardItemDataList);
	}
}
