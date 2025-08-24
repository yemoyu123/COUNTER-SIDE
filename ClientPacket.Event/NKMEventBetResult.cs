using Cs.Protocol;

namespace ClientPacket.Event;

public sealed class NKMEventBetResult : ISerializable
{
	public int eventIndex;

	public EventBetTeam winTeam;

	public long totalBetCountTeamA;

	public long totalBetCountTeamB;

	public int userCountTeamA;

	public int userCountTeamB;

	public int winRateTeamA;

	public int winRateTeamB;

	public float dividentRateTeamA;

	public float dividentRateTeamB;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref eventIndex);
		stream.PutOrGetEnum(ref winTeam);
		stream.PutOrGet(ref totalBetCountTeamA);
		stream.PutOrGet(ref totalBetCountTeamB);
		stream.PutOrGet(ref userCountTeamA);
		stream.PutOrGet(ref userCountTeamB);
		stream.PutOrGet(ref winRateTeamA);
		stream.PutOrGet(ref winRateTeamB);
		stream.PutOrGet(ref dividentRateTeamA);
		stream.PutOrGet(ref dividentRateTeamB);
	}
}
