using System.Collections.Generic;
using Cs.Protocol;

namespace ClientPacket.Mode;

public sealed class NKMPalaceData : ISerializable
{
	public int palaceId;

	public int currentDungeonId;

	public List<NKMPalaceDungeonData> dungeonDataList = new List<NKMPalaceDungeonData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref palaceId);
		stream.PutOrGet(ref currentDungeonId);
		stream.PutOrGet(ref dungeonDataList);
	}
}
