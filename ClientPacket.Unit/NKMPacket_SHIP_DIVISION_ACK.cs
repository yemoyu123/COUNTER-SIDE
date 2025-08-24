using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_SHIP_DIVISION_ACK)]
public sealed class NKMPacket_SHIP_DIVISION_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<long> removeShipUIDList = new List<long>();

	public List<NKMItemMiscData> rewardItemDataList = new List<NKMItemMiscData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref removeShipUIDList);
		stream.PutOrGet(ref rewardItemDataList);
	}
}
