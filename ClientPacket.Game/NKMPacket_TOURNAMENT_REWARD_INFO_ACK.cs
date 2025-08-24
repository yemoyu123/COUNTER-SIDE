using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_TOURNAMENT_REWARD_INFO_ACK)]
public sealed class NKMPacket_TOURNAMENT_REWARD_INFO_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int tournamentid;

	public int hitCount;

	public NKMRewardData predictionRewardData;

	public NKMTournamentGroups groupIndex;

	public int winData;

	public NKMRewardData rankRewardData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref tournamentid);
		stream.PutOrGet(ref hitCount);
		stream.PutOrGet(ref predictionRewardData);
		stream.PutOrGetEnum(ref groupIndex);
		stream.PutOrGet(ref winData);
		stream.PutOrGet(ref rankRewardData);
	}
}
