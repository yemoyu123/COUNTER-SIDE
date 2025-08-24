using System.Collections.Generic;
using Cs.Protocol;

namespace ClientPacket.Common;

public sealed class NKMTournamentBanResult : ISerializable
{
	public HashSet<int> unitBanList = new HashSet<int>();

	public HashSet<int> shipBanList = new HashSet<int>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitBanList);
		stream.PutOrGet(ref shipBanList);
	}
}
