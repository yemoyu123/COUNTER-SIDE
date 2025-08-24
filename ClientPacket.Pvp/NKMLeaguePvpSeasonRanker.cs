using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;

namespace ClientPacket.Pvp;

public sealed class NKMLeaguePvpSeasonRanker : ISerializable
{
	public int seasonId;

	public List<NKMUserProfileData> profiles = new List<NKMUserProfileData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref seasonId);
		stream.PutOrGet(ref profiles);
	}
}
