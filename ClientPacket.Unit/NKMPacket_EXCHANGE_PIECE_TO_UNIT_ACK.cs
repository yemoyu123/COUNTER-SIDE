using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_EXCHANGE_PIECE_TO_UNIT_ACK)]
public sealed class NKMPacket_EXCHANGE_PIECE_TO_UNIT_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<NKMUnitData> unitDataList = new List<NKMUnitData>();

	public NKMItemMiscData costItemData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref unitDataList);
		stream.PutOrGet(ref costItemData);
	}
}
