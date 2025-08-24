using Cs.Protocol;
using NKM;

namespace ClientPacket.Common;

public sealed class NKMDungeonRewardSet : ISerializable
{
	public NKMEpisodeCompleteData episodeCompleteData;

	public NKMDungeonClearData dungeonClearData = new NKMDungeonClearData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref episodeCompleteData);
		stream.PutOrGet(ref dungeonClearData);
	}
}
