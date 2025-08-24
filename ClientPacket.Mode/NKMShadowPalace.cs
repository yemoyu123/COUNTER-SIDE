using System.Collections.Generic;
using Cs.Protocol;

namespace ClientPacket.Mode;

public sealed class NKMShadowPalace : ISerializable
{
	public int currentPalaceId;

	public int life;

	public List<NKMPalaceData> palaceDataList = new List<NKMPalaceData>();

	public int rewardMultiply = 1;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref currentPalaceId);
		stream.PutOrGet(ref life);
		stream.PutOrGet(ref palaceDataList);
		stream.PutOrGet(ref rewardMultiply);
	}
}
