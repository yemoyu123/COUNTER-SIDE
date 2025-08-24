using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_UNIT_SKILL_UPGRADE_ACK)]
public sealed class NKMPacket_UNIT_SKILL_UPGRADE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long unitUID;

	public int skillID;

	public int skillLevel;

	public List<NKMItemMiscData> costItemDataList = new List<NKMItemMiscData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref unitUID);
		stream.PutOrGet(ref skillID);
		stream.PutOrGet(ref skillLevel);
		stream.PutOrGet(ref costItemDataList);
	}
}
