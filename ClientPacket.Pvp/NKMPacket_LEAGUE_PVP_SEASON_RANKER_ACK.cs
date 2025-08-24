using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_LEAGUE_PVP_SEASON_RANKER_ACK)]
public sealed class NKMPacket_LEAGUE_PVP_SEASON_RANKER_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<NKMLeaguePvpSeasonRanker> rankerDatas = new List<NKMLeaguePvpSeasonRanker>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref rankerDatas);
	}
}
