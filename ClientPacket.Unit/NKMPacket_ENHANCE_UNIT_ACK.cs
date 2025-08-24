using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_ENHANCE_UNIT_ACK)]
public sealed class NKMPacket_ENHANCE_UNIT_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long unitUID;

	public List<int> statExpList = new List<int>();

	public List<long> consumeUnitUIDList = new List<long>();

	public NKMItemMiscData costItemData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref unitUID);
		stream.PutOrGet(ref statExpList);
		stream.PutOrGet(ref consumeUnitUIDList);
		stream.PutOrGet(ref costItemData);
	}
}
