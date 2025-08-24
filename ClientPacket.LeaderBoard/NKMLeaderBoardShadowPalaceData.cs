using System.Collections.Generic;
using Cs.Protocol;

namespace ClientPacket.LeaderBoard;

public sealed class NKMLeaderBoardShadowPalaceData : ISerializable
{
	public List<NKMShadowPalaceData> shadowPalaceData = new List<NKMShadowPalaceData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref shadowPalaceData);
	}
}
