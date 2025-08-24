using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_STAGE_UNLOCK_ACK)]
public sealed class NKMPacket_STAGE_UNLOCK_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int stageId;

	public List<NKMItemMiscData> costItemDataList = new List<NKMItemMiscData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref stageId);
		stream.PutOrGet(ref costItemDataList);
	}
}
