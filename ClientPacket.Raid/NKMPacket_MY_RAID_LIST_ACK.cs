using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Raid;

[PacketId(ClientPacketId.kNKMPacket_MY_RAID_LIST_ACK)]
public sealed class NKMPacket_MY_RAID_LIST_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<NKMMyRaidData> myRaidDataList = new List<NKMMyRaidData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref myRaidDataList);
	}
}
