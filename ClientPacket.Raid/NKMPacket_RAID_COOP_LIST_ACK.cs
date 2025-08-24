using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Raid;

[PacketId(ClientPacketId.kNKMPacket_RAID_COOP_LIST_ACK)]
public sealed class NKMPacket_RAID_COOP_LIST_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<NKMCoopRaidData> coopRaidDataList = new List<NKMCoopRaidData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref coopRaidDataList);
	}
}
