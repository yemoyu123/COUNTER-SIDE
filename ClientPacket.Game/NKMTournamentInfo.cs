using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;

namespace ClientPacket.Game;

public sealed class NKMTournamentInfo : ISerializable
{
	public NKMTournamentGroups groupIndex;

	public Dictionary<long, NKMTournamentProfileData> userInfo = new Dictionary<long, NKMTournamentProfileData>();

	public List<long> slotUserUid = new List<long>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref groupIndex);
		stream.PutOrGet(ref userInfo);
		stream.PutOrGet(ref slotUserUid);
	}
}
