using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPACKET_EVENT_BET_BETTING_ACK)]
public sealed class NKMPACKET_EVENT_BET_BETTING_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMEventBetPrivate eventBetPrivate = new NKMEventBetPrivate();

	public List<NKMItemMiscData> costItemList = new List<NKMItemMiscData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref eventBetPrivate);
		stream.PutOrGet(ref costItemList);
	}
}
