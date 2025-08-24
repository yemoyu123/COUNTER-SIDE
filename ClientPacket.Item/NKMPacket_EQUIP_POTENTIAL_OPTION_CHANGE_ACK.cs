using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_POTENTIAL_OPTION_CHANGE_ACK)]
public sealed class NKMPacket_EQUIP_POTENTIAL_OPTION_CHANGE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<NKMItemMiscData> costItemDataList = new List<NKMItemMiscData>();

	public NKMPotentialOptionChangeCandidate potentialOptionCandidate = new NKMPotentialOptionChangeCandidate();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref costItemDataList);
		stream.PutOrGet(ref potentialOptionCandidate);
	}
}
