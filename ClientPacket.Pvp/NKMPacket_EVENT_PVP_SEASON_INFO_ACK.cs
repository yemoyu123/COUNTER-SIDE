using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_EVENT_PVP_SEASON_INFO_ACK)]
public sealed class NKMPacket_EVENT_PVP_SEASON_INFO_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public PvpState eventPvpState;

	public List<EventPvpReward> eventPvpRewardInfos = new List<EventPvpReward>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref eventPvpState);
		stream.PutOrGet(ref eventPvpRewardInfos);
	}
}
