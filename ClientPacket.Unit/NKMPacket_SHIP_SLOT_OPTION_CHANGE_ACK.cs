using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_SHIP_SLOT_OPTION_CHANGE_ACK)]
public sealed class NKMPacket_SHIP_SLOT_OPTION_CHANGE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMUnitData shipData;

	public NKMShipModuleCandidate candidateOption = new NKMShipModuleCandidate();

	public List<NKMItemMiscData> costItemDataList = new List<NKMItemMiscData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref shipData);
		stream.PutOrGet(ref candidateOption);
		stream.PutOrGet(ref costItemDataList);
	}
}
