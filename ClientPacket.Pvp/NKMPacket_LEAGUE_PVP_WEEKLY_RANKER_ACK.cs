using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_LEAGUE_PVP_WEEKLY_RANKER_ACK)]
public sealed class NKMPacket_LEAGUE_PVP_WEEKLY_RANKER_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<NKMUserProfileData> userProfileData = new List<NKMUserProfileData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref userProfileData);
	}
}
