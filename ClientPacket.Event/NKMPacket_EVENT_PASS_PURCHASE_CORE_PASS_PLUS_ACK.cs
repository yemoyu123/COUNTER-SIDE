using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_EVENT_PASS_PURCHASE_CORE_PASS_PLUS_ACK)]
public sealed class NKMPacket_EVENT_PASS_PURCHASE_CORE_PASS_PLUS_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int totalExp;

	public List<NKMItemMiscData> costItemList = new List<NKMItemMiscData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref totalExp);
		stream.PutOrGet(ref costItemList);
	}
}
